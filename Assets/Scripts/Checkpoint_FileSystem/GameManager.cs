using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton-Pattern
    public static GameManager Instance { get; private set; }

    // Referenzen auf wichtige Komponenten
    public PlayerMovement playerMovement;
    public HealthSystem healthSystem;
    public SoulShardSystem soulShardSystem;
    public InventorySystem inventorySystem;

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

        // Aktualisiere die Soul Shard Anzeige
        // UIHandler.instance.UpdateSoulShardCount(soulShardSystem.GetSoulShardCount());

        // Empty inventory
        inventorySystem.ClearInventory();

        // Respawne enemies and items
        RespawnEnemiesAndItems();

        print("Health: " + healthSystem.currentHealth + " soulShards: " + soulShardSystem.GetSoulShardCount());
    }

    private int LoadSoulShardCountFromLastCheckpoint()
    {
        // Lade die gespeicherte Anzahl der Soul Shards aus dem letzten Checkpoint
        // Hier könntest du die Daten aus dem SaveManager oder einem separaten Speicher laden
        PlayerData data = SaveManager.Instance.LoadPlayerData();
        return data != null ? data.soulShardCount : 0;
    }

    private void RespawnEnemiesAndItems()
    {
        // Respawne Gegner
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            enemy.Respawn();
        }

        // Respawne Items
        foreach (var respawner in itemRespawners)
        {
            respawner.RespawnItem();
        }
    }

    private List<ItemRespawner> itemRespawners = new List<ItemRespawner>();

    public void RegisterItem(ItemRespawner respawner)
    {
        if (!itemRespawners.Contains(respawner))
            itemRespawners.Add(respawner);
    }
}
