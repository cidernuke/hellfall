using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using ItemSystem.Abstract;

public class GameManager : MonoBehaviour
{
    // Singleton-Pattern
    public static GameManager Instance { get; private set; }

    //References to important components
    public PlayerMovement playerMovement;
    public HealthSystem healthSystem;
    public SoulShardSystem soulShardSystem;
    public KeySystem keySystem;
    public InventorySystem inventorySystem;
    public PlayerController playerController;
    private List<ItemRespawner> itemRespawners = new List<ItemRespawner>();
    private List<EnemyController> enemyRespawners = new List<EnemyController>();

    public Vector3 setPlayerCoordinates;
    private readonly Vector3 playerSpawnPointEndBoss = new(0, 0.5f, 0);
    public bool isMainBoss = false;

    private void Awake()
    {
        // Singleton-implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Persistence across scenes
        }
        else
        {
            Destroy(gameObject);
            print("Found multiple instances of GameManager.");
        }
    }

    private void OnEnable()
    {
        //On every scene change call OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        //log out to prevent memory leak
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Called when the game starts.
    /// </summary>
    private void Start()
    {
        InitializeReferences();
        RegisterSceneObjects();
        TimerSystem.Instance.StartTimer("Scene_01");
    }

    /// <summary>
    /// Getting called when a new scene is loaded.
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
        if (setPlayerCoordinates != new Vector3(0, 0, 0) && !isMainBoss)
        {
            playerMovement.transform.position = setPlayerCoordinates;
        }
        else if (spawnPoint != null)
        {
            // if (isMainBoss)
            // {
            //     EndBossMain mainBoss = new(); 
            //     mainBoss.gameObject.SetActive(true);
            // }
            print("spawning at spawn point");
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
            StartCoroutine(LoadGameAfterUIIsReady());
        }
    }

    public void SetIsMainBossTrue()
    {
        isMainBoss = true;
    }

    /// <summary>
    /// Search dynamically for Player, InventorySystem and link them.
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
            keySystem = player.GetComponent<KeySystem>();
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            print("Player not found.");
            playerMovement = null;
            healthSystem = null;
            soulShardSystem = null;
            keySystem = null;
            playerController = null;
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
    /// Search for all relevant objects (ItemRespawner, EnemyController) in the current scene and register them.
    /// </summary>
    private void RegisterSceneObjects()
    {
        //Find items in the scene
        ItemRespawner[] respawnersInScene = FindObjectsOfType<ItemRespawner>();
        foreach (var resp in respawnersInScene)
        {
            RegisterItem(resp);
        }

        //Find enemies in the scene
        EnemyController[] enemiesInScene = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemiesInScene)
        {
            RegisterEnemy(enemy);
        }
    }

    /// <summary>
    /// Save the current game state.
    /// </summary>
    public void SaveGame()
    {
        if (SaveManager.Instance)
        {
            SaveManager.Instance.SaveGame(playerMovement, healthSystem, soulShardSystem, inventorySystem, keySystem, playerController);
        }
        else
        {
            Debug.LogWarning("SaveManager.Instance is null. Cannot save.");
        }
    }

    /// <summary>
    /// Load the last saved game state.
    /// </summary>
    public void LoadGame()
    {
        if (SaveManager.Instance)
        {
            SaveManager.Instance.LoadGame(playerMovement, healthSystem, soulShardSystem, inventorySystem, keySystem, playerController);
        }
        else
        {
            Debug.LogWarning("SaveManager.Instance is null. Cannot load.");
        }
    }

    /// <summary>
    /// Start a new game.
    /// </summary>
    public void StartNewGame()
    {
        //Check player references
        if (playerMovement == null || healthSystem == null || soulShardSystem == null || keySystem == null || playerController == null)
        {
            Debug.LogWarning("StartNewGame: Player references are missing trying to reInitialize references.");
            InitializeReferences();
            if (playerMovement == null || healthSystem == null || soulShardSystem == null || keySystem == null || playerController == null)
            {
                Debug.LogWarning("StartNewGame: Player references are missing, cannot start new Game");
                return;
            }
        }

        //Reset Health
        healthSystem.currentHealth = healthSystem.startingHealth;
        //update healthbar and text
        healthSystem.UpdateHealthUI();

        //SoulShards to 0
        soulShardSystem.SetSoulShardCount(0);

        //KeyCount to 0
        keySystem.SetKeyCount(0);

        //Empty Inventory
        inventorySystem.ClearInventory();
        inventorySystem.SetupReferences();

        playerController.SetUpReferences();

        //Delete old save file to prevent loading the old game state
        if (SaveManager.Instance.saveFilePath != null)
        {
            string path = SaveManager.Instance.saveFilePath;
            if (File.Exists(path))
            {
                File.Delete(path);
                print("Old save file deleted.");
            }
        }
        else
        {
            print("No save file found.");
        }

        Debug.Log("StartNewGame: Values reset to default.");
    }

    /// <summary>
    /// Respawn the player at the last checkpoint.
    /// </summary>
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

        // Reset healthbar and healthtext
        healthSystem.UpdateHealthUI();

        PlayerData data = SaveManager.Instance.LoadPlayerData();

        // Reset shoulShards to the value while reaching the last checkpoint
        soulShardSystem.SetSoulShardCount(LoadSoulShardCountFromLastCheckpoint(data));

        // Reset keyCounter to the value while reaching the last checkpoint
        keySystem.SetKeyCount(LoadKeyCountFromLastCheckpoint(data));

        //inventorySystem.ClearInventory();
        LoadInventoryFromLastCheckpoint(data);

        //Not working atm because JsonUtility does not support Dictionaries...
        //LoadCharacterStatsFromLastCheckpoint(data);

        // Respawne enemies and items
        RespawnEnemiesAndItems();

        print("RespawnPlayer got called");
        print("Health: " + healthSystem.currentHealth + " soulShards: " + soulShardSystem.GetSoulShardCount());
    }

    /// <summary>
    /// Load the amount of collected SoulShards from the last checkpoint.
    /// </summary>
    /// <returns>SoulShardCount from the last checkpoint</returns>
    private int LoadSoulShardCountFromLastCheckpoint(PlayerData data)
    {
        //Load the amount of collected SoulShards from the last checkpoint
        return data != null ? data.soulShardCount : 0;
    }

    /// <summary>
    /// Load the amount of collected keys from the last checkpoint.
    /// </summary>
    /// <returns>KeyCounter from the last checkpoint</returns>
    private int LoadKeyCountFromLastCheckpoint(PlayerData data)
    {
        //Load the amount of collected keys from the last checkpoint
        return data != null ? data.keyCounter : 0;
    }

    /// <summary>
    /// Load the inventory from the last checkpoint.
    /// <paramref name="data"/> The PlayerData from the last checkpoint.
    /// </summary>
    private void LoadInventoryFromLastCheckpoint(PlayerData data)
    {
        if (data == null)
        {
            Debug.LogWarning("No checkpoint data found. Inventory will not be restored.");
            inventorySystem.ClearInventory();
            return;
        }

        //Empty inventory before filling it with checkpoint data
        inventorySystem.ClearInventory();

        for (int i = 0; i < inventorySystem.slots.Length; i++)
        {
            string itemName = data.collectedItemNames[i];

            if (!string.IsNullOrEmpty(itemName))
            {
                ItemData itemData = Resources.Load<ItemData>("ItemData/" + itemName);
                if (itemData != null)
                {
                    Item newItem = inventorySystem.CreateItemInstance(itemData);
                    inventorySystem.slots[i].StoreItem(newItem, itemData);
                }
                else
                {
                    Debug.LogWarning($"ItemData for {itemName} not found.");
                }
            }
        }
        Debug.Log("Inventory restored to last checkpoint state.");
    }

    /// <summary>
    /// Not working atm because JsonUtility does not support Dictionaries...
    /// Load the character stats from the last checkpoint.
    /// <paramref name="data"/> The PlayerData from the last checkpoint.
    /// </summary>
    private void LoadCharacterStatsFromLastCheckpoint(PlayerData data)
    {
        if (data != null && data.characterStats != null)
        {
            print("Current CharacterStats"+ playerController.playerStats);
            print("Loaded CharacterStats: "+ data.characterStats);

            var loadedStats = data.characterStats;
            var playerStats = playerController.playerStats;

            playerStats.vitality.SetBaseValue(loadedStats["vitalityBase"]);
            playerStats.strength.SetBaseValue(loadedStats["strengthBase"]);
            playerStats.intelligence.SetBaseValue(loadedStats["intelligenceBase"]);

            playerStats.maxHealth.SetBaseValue(loadedStats["maxHealthBase"]);
            playerStats.maxHealth.SetModifier(loadedStats["maxHealthMod"]);

            playerStats.closeDamage.SetBaseValue(loadedStats["closeDamageBase"]);
            playerStats.closeDamage.SetModifier(loadedStats["closeDamageMod"]);

            playerStats.rangedDamage.SetBaseValue(loadedStats["rangedDamageBase"]);
            playerStats.rangedDamage.SetModifier(loadedStats["rangedDamageMod"]);
            playerStats.rangedCooldown.SetBaseValue(loadedStats["rangedCooldownBase"]);
            playerStats.rangedCooldown.SetModifier(loadedStats["rangedCooldownMod"]);
            playerStats.rangedRange.SetBaseValue(loadedStats["rangedRangeBase"]);
            playerStats.rangedRange.SetModifier(loadedStats["rangedRangeMod"]);

            print("Loaded playercontroller.CharacterStats: "+ playerController.playerStats);
            print("Loaded CharacterStats: "+ playerStats);
        }
        else
        {
            print("No CharacterStats found in the last checkpoint.");
        }
    }

    /// <summary>
    /// Respawn all enemies and items in the scene.
    /// </summary>
    private void RespawnEnemiesAndItems()
    {
        // Respawn enemies
        foreach (var enemy in enemyRespawners)
        {
            if (enemy != null && !enemy.isBoss)
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
            // So that the keys dont respawn on death and can be duplicated
            if (respawner.gameObject.name != "KeyItem(Clone)")
            {
                respawner.RespawnItem();
            }
        }
    }

    /// <summary>
    /// Register an item to the GameManager.
    /// </summary>
    /// <param name="respawner"></param>
    public void RegisterItem(ItemRespawner respawner)
    {
        if (!itemRespawners.Contains(respawner))
            itemRespawners.Add(respawner);
    }

    /// <summary>
    /// Register an enemy to the GameManager.
    /// </summary>
    /// <param name="enemy"></param>
    public void RegisterEnemy(EnemyController enemy)
    {
        if (!enemyRespawners.Contains(enemy))
            enemyRespawners.Add(enemy);
    }

    /// <summary>
    /// Load the game after the UI is ready.
    /// </summary>
    /// <returns></returns>
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
