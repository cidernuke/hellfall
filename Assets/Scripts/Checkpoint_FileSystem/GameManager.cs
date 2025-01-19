using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton-Pattern
    public static GameManager Instance { get; private set; }

    // Referenzen auf wichtige Komponenten
    public PlayerMovement playerMovement;
    public HealthSystem healthSystem;
    public SoulShardSystem soulShardSystem;
    public InventorySystem inventorySystem;
    private List<ItemRespawner> itemRespawners = new List<ItemRespawner>();
    private List<EnemyController> enemyRespawners = new List<EnemyController>();

    public Vector3 setPlayerCoordinates;

    private void Awake()
    {
        // Singleton-Implementierung
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persistiert über Szenen hinweg
        }
        else
        {
            Destroy(gameObject);
            print("Mehrere Instanzen von GameManager erkannt.");
        }
    }

    private void OnEnable()
    {
        // Bei jedem Szenenwechsel OnSceneLoaded aufrufen
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Abmelden, damit kein Memory Leak entsteht
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        InitializeReferences();
        //SceneManager.sceneLoaded += OnSceneLoaded;
        RegisterSceneObjects();
    }

    /// <summary>
    /// Wird aufgerufen, sobald eine neue Szene geladen wurde.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // After switching scenes, reset references
        InitializeReferences();

        // Delete old references
        itemRespawners.Clear();
        enemyRespawners.Clear();

        RegisterSceneObjects();

        //Sets the spawn location after switching scene
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (setPlayerCoordinates != new Vector3(0, 0, 0))
        {
            playerMovement.transform.position = setPlayerCoordinates;
        }
        else if (spawnPoint != null)
        {
            playerMovement.transform.position = spawnPoint.transform.position;
        }

        if (scene.name == "Menu")
        {
            //Hide UI in Menu
            InventorySystem.Instance.HideInventoryUI();
        }
        else
        {
            //Show UI in game
            InventorySystem.Instance.ShowInventoryUI();
        }

        if (SaveManager.Instance.isLoadingFromSave)
        {
            // LoadGame();

            // // Danach nicht mehr laden
            // SaveManager.Instance.isLoadingFromSave = false;

            //SaveManager.Instance.isLoadingFromSave = false;
            StartCoroutine(LoadGameAfterUIIsReady());
        }
    }

    /// <summary>
    /// Sucht dynamisch Player, InventorySystem und verknüpft sie.
    /// </summary>
    private void InitializeReferences()
    {
        // Player references
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            healthSystem = player.GetComponent<HealthSystem>();
            soulShardSystem = player.GetComponent<SoulShardSystem>();
        }
        else
        {
            print("Player not found.");
            playerMovement = null;
            healthSystem = null;
            soulShardSystem = null;
        }

        // InventorySystem (Singleton)
        inventorySystem = InventorySystem.Instance;
        if (inventorySystem == null)
        {
            print("InventorySystem.Instance is null.");
        }
        else
        {
            if (healthSystem != null)
                inventorySystem.playerHealth = healthSystem;
        }
    }

    /// <summary>
    /// Sucht alle relevanten Objekte (ItemRespawner, EnemyController) in der aktuellen Szene und registriert sie.
    /// </summary>
    private void RegisterSceneObjects()
    {
        // Items in der Szene finden
        ItemRespawner[] respawnersInScene = FindObjectsOfType<ItemRespawner>();
        foreach (var resp in respawnersInScene)
        {
            RegisterItem(resp);
        }

        // Enemies in der Szene finden
        EnemyController[] enemiesInScene = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemiesInScene)
        {
            RegisterEnemy(enemy);
        }
    }

    public void SaveGame()
    {
        if (SaveManager.Instance)
        {
            SaveManager.Instance.SaveGame(playerMovement, healthSystem, soulShardSystem, inventorySystem);
        }
        else
        {
            Debug.LogWarning("SaveManager.Instance is null. Cannot save.");
        }
    }

    public void LoadGame()
    {
        if (SaveManager.Instance)
        {
            SaveManager.Instance.LoadGame(playerMovement, healthSystem, soulShardSystem, inventorySystem);
        }
        else
        {
            Debug.LogWarning("SaveManager.Instance is null. Cannot load.");
        }
    }

    public void StartNewGame()
    {
        //Check player references
        if (playerMovement == null || healthSystem == null || soulShardSystem == null)
        {
            Debug.LogWarning("StartNewGame: Player references are missing trying to reInitialize references.");
            InitializeReferences();
            if (playerMovement == null || healthSystem == null || soulShardSystem == null)
            {
                Debug.LogWarning("StartNewGame: Player references are missing, cannot start new Game");
                return;
            }
        }

        //Reset Health
        healthSystem.currentHealth = healthSystem.startingHealth;
        if (UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(1.0f);
        }

        //SoulShards to 0
        soulShardSystem.SetSoulShardCount(0);

        //Empty Inventory
        inventorySystem.ClearInventory();
        inventorySystem.SetupReferences();

        //Rest Checkpoint-Status
        //Need more logic, not done yet

        Debug.Log("StartNewGame: Values reset to default.");
    }

    public void RespawnPlayer()
    {
        if (playerMovement == null || healthSystem == null)
        {
            Debug.LogWarning("Cannot respawn player because references are missing.");
            return;
        }
        // Reset the position of the player to the last checkpoint
        playerMovement.Respawn();

        // Reset health to max value
        healthSystem.currentHealth = healthSystem.startingHealth;

        // Reset healthbar animation
        UIHandler.instance.SetHealthValue(healthSystem.currentHealth / healthSystem.startingHealth);

        // Reset shoulShards to the value while reaching the last checkpoint
        soulShardSystem.SetSoulShardCount(LoadSoulShardCountFromLastCheckpoint());

        // Empty inventory
        inventorySystem.ClearInventory();

        // Respawne enemies and items
        RespawnEnemiesAndItems();

        print("RespawnPlayer wurde aufgerufen");
        print("Health: " + healthSystem.currentHealth + " soulShards: " + soulShardSystem.GetSoulShardCount());
    }

    private int LoadSoulShardCountFromLastCheckpoint()
    {
        //Load the amount of collected SoulShards from the last checkpoint
        PlayerData data = SaveManager.Instance.LoadPlayerData();
        return data != null ? data.soulShardCount : 0;
    }

    private void RespawnEnemiesAndItems()
    {
        // Respawn enemies
        foreach (var enemy in enemyRespawners)
        {
            //enemy.Respawn();
            if (enemy != null)
            {
                HealthSystem hs = enemy.GetComponent<HealthSystem>();
                if (hs != null)
                {
                    //Get inital position of the enemy
                    Vector3 initialPos = enemy.GetInitialPosition();
                    //Set inital position of the enemy
                    hs.RespawnEnemy(initialPos);
                }
            }
        }

        // Respawn Items
        foreach (var respawner in itemRespawners)
        {
            respawner.RespawnItem();
        }
    }

    public void RegisterItem(ItemRespawner respawner)
    {
        if (!itemRespawners.Contains(respawner))
            itemRespawners.Add(respawner);
    }

    public void RegisterEnemy(EnemyController enemy)
    {
        if (!enemyRespawners.Contains(enemy))
            enemyRespawners.Add(enemy);
    }

    private IEnumerator LoadGameAfterUIIsReady()
    {
        //Wait until UIHandler.instance != null 
        yield return new WaitUntil(() => UIHandler.instance != null);

        LoadGame();
        healthSystem?.UpdateHealthUI();

        SaveManager.Instance.isLoadingFromSave = false;
        var pmc = GameObject.Find("PauseMenuController");
        pmc.GetComponent<PauseMenuController>().DeactivateMenu();
    }
}
