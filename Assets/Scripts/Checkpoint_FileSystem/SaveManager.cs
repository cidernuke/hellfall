using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using ItemSystem.Abstract;
using ItemSystem.Items;

public class SaveManager : MonoBehaviour
{
    // Singleton-Pattern, damit es nur eine Instanz des SaveManagers gibt
    public static SaveManager Instance;

    private void Awake()
    {
        // Singleton-Implementierung
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Damit der SaveManager über Szenen hinweg erhalten bleibt
            print("SaveManager initialisiert");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Pfad zur Speicherdatei
    private string saveFilePath;

    private void Start()
    {
        saveFilePath = Application.persistentDataPath + "/savegame.dat";
    }

    // Methode zum Speichern des Spiels
    //public void SaveGame(PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
    public void SaveGame(PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
    {
        print("Saving Game");
        // Erstelle ein neues PlayerData-Objekt
        PlayerData playerData = new PlayerData(
            healthSystem,
            playerMovement.GetLastCheckpointID(),
            soulShardSystem,
            inventorySystem
        );

        // Erstelle GameData und füge PlayerData hinzu
        GameData gameData = new GameData
        {
            playerData = playerData
            // enviromentData = ... // später, wenn  EnviromentData hinzugefügt wird
        };

        // Serialisiere GameData zu JSON
        string json = JsonUtility.ToJson(gameData);

        // Schreibe JSON in Datei
        File.WriteAllText(saveFilePath, json);

        //Check if the file was written and the integrity
        if (File.Exists(saveFilePath))
        {
            string writtenContent = File.ReadAllText(saveFilePath);
            if (writtenContent == json)
            {
                print("Das Spiel wurde erfolgreich gespeichert und überprüft.");
            }
            else
            {
                print("Der Dateiinhalt stimmt nicht mit dem erwarteten Inhalt überein.");
            }
        }
        else
        {
            print("Das Spiel wurde nicht gespeichert.");
        }
    }

    // Methode zum Laden des Spiels
    public void LoadGame(PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
    {
        GameData gameData = LoadGameData();
        if (gameData != null)
        {
            // Wende die geladenen Daten an
            ApplyLoadedData(gameData, playerMovement, healthSystem, soulShardSystem, inventorySystem);
            Debug.Log("Spiel geladen.");
        }
        else
        {
            Debug.LogWarning("Keine Speicherdatei gefunden.");
        }

        //Before Refactoring:
        //TODO: Stay with Dry -> Use LoadPlayerData
        // if (File.Exists(saveFilePath))
        // {
        //     string json = File.ReadAllText(saveFilePath);

        //     // Deserialisiere JSON zu GameData
        //     GameData gameData = JsonUtility.FromJson<GameData>(json);

        //     // Wende die geladenen Daten an
        //     //ApplyLoadedData(gameData, playerMovement, healthSystem, playerInventory, soulShardSystem);
        //     ApplyLoadedData(gameData, playerMovement, healthSystem, soulShardSystem, inventorySystem);
        //     Debug.Log("Spiel geladen.");
        // }
        // else
        // {
        //     Debug.LogWarning("Keine Speicherdatei gefunden.");
        // }
    }

    //private void ApplyLoadedData(GameData gameData, PlayerMovement playerMovement, HealthSystem healthSystem, PlayerInventory playerInventory, SoulShardSystem soulShardSystem)
    private void ApplyLoadedData(GameData gameData, PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
    {
        PlayerData data = gameData.playerData;

        // Setzt die Gesundheit
        healthSystem.currentHealth = data.currentHealth;

        // Setzt respawnHealth
        healthSystem.respawnHealth = data.respawnHealth;

        // Setzt die Anzahl der Soul Shards
        soulShardSystem.SetSoulShardCount(data.soulShardCount);


        //Setzt die Items
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
                    Debug.LogWarning("ItemData für " + itemName + " nicht gefunden.");
                    //Game keeps running but with empty slot
                    inventorySystem.slots[i].storedItem = null;
                }
            }
            else
            {
                // Leeren Slot sicherstellen
                inventorySystem.slots[i].storedItem = null;
            }
        }

        // Findet den Checkpoint mit der gespeicherten ID und setze die Position
        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.checkpointID == data.lastCheckpointID)
            {
                playerMovement.transform.position = checkpoint.transform.position;
                playerMovement.UpdateRespawnPoint(checkpoint.transform.position, checkpoint.checkpointID);
                break;
            }
        }
    }

    public PlayerData LoadPlayerData()
    {
        GameData gameData = LoadGameData();
        if (gameData != null)
        {
            return gameData.playerData;
        }
        else
        {
            return null;
        }
    }

    private GameData LoadGameData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            return gameData;
        }
        else
        {
            Debug.LogWarning("Keine Speicherdatei gefunden.");
            return null;
        }
    }

}
