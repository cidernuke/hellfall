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
     //[SerializeField] private TMP_Text deathMessage;

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
        // Initialisiere die Referenzen
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        // Spieler-Referenzen
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

    // Methoden zum Speichern und Laden
    public void SaveGame()
    {
        SaveManager.Instance.SaveGame(playerMovement, healthSystem, soulShardSystem, inventorySystem);
    }

    public void LoadGame()
    {
        SaveManager.Instance.LoadGame(playerMovement, healthSystem, soulShardSystem, inventorySystem);
    }

    // Methoden für Respawn-Logik
    public void RespawnPlayer()
    {
        // Setze die Position des Spielers auf den letzten Checkpoint
        //<playerMovement.transform.position = playerMovement.RespawnPosition;
        playerMovement.Respawn();

        // Setze die Gesundheit auf volle Gesundheit
        healthSystem.currentHealth = healthSystem.startingHealth;

        // Aktualisiere die Gesundheitsanzeige
        UIHandler.instance.SetHealthValue(healthSystem.currentHealth / healthSystem.startingHealth);

        // Setze die Anzahl der Soul Shards auf den Wert vom letzten Checkpoint
        soulShardSystem.SetSoulShardCount(LoadSoulShardCountFromLastCheckpoint());

        // Aktualisiere die Soul Shard Anzeige
        // UIHandler.instance.UpdateSoulShardCount(soulShardSystem.GetSoulShardCount());

        // Leere das Inventar
        inventorySystem.ClearInventory();

        // Respawne Gegner und Items
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
        // ItemRespawner[] itemSpawners = FindObjectsOfType<ItemRespawner>();
        // foreach (var spawner in itemSpawners)
        // {
        //     spawner.RespawnItem();
        // }
        // Items aus der Liste respawnen
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

    //     public void ShowDeathMessage()
    // {
    //     if (deathMessage != null)
    //     {
    //         deathMessage.text = "Rise again, or be forgotten among the damned.";
    //         print("Setting death message active");
    //         deathMessage.gameObject.SetActive(true);

    //         StartCoroutine(HideMessageAfterDelay(3f));
    //     }
    // }

//     public void ShowDeathMessage(Vector3 playerPosition)
// {
//     if (deathMessage != null)
//     {
//         // Spieler-Weltposition in Bildschirmkoordinaten umrechnen
//         Vector3 screenPos = Camera.main.WorldToScreenPoint(playerPosition);

//         // Position des Textes anpassen (RectTransform erforderlich)
//         RectTransform rt = deathMessage.GetComponent<RectTransform>();
//         rt.position = screenPos;
        
//         // Nachricht setzen und einblenden
//         deathMessage.text = "Rise again, or be forgotten among the damned.";
//         deathMessage.gameObject.SetActive(true);

//         StartCoroutine(HideMessageAfterDelay(3f));
//     }
// }

//     private IEnumerator HideMessageAfterDelay(float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         deathMessage.gameObject.SetActive(false);
//     }
}
