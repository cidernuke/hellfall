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

    private void Start()
    {
        InitializeReferences();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // After switching scenes, reset references
        InitializeReferences();

        // Delete old references
        itemRespawners.Clear();
        enemyRespawners.Clear();

        //Register Items
        ItemRespawner[] respawnersInScene = FindObjectsOfType<ItemRespawner>();
        foreach (var resp in respawnersInScene)
        {
            RegisterItem(resp);
        }

        //Register enemies
        EnemyController[] enemiesInScene = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemiesInScene)
        {
            RegisterEnemy(enemy);
        }
    }

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
        }

        // InventorySystem (Singleton)
        inventorySystem = InventorySystem.Instance;
        if (inventorySystem == null)
        {
            print("InventorySystem.Instance ist null.");
        }
    }

    public void SaveGame()
    {
        SaveManager.Instance.SaveGame(playerMovement, healthSystem, soulShardSystem, inventorySystem);
    }

    public void LoadGame()
    {
        SaveManager.Instance.LoadGame(playerMovement, healthSystem, soulShardSystem, inventorySystem);
    }

    public void RespawnPlayer()
    {
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

        print("Health: " + healthSystem.currentHealth + " soulShards: " + soulShardSystem.GetSoulShardCount());
    }

    private int LoadSoulShardCountFromLastCheckpoint()
    {
        //Load the amount of collected SoulShards from the last checkpoint
        // Hier könntest du die Daten aus dem SaveManager oder einem separaten Speicher laden
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
}
