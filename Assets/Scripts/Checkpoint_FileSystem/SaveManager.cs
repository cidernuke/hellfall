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
            DontDestroyOnLoad(gameObject); // Optional, falls der SaveManager über Szenen hinweg erhalten bleiben soll
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
    //public void SaveGame(PlayerMovement playerMovement, HealthSystem healthSystem, PlayerInventory playerInventory, SoulShardSystem soulShardSystem)
    public void SaveGame(PlayerMovement playerMovement, HealthSystem healthSystem)
    {
        // Erstelle ein neues PlayerData-Objekt
        PlayerData playerData = new PlayerData(
            healthSystem,
            //playerInventory.collectedItems,
            //soulShardSystem,
            playerMovement.GetLastCheckpointID()
        );

        // Erstelle GameData und füge PlayerData hinzu
        GameData gameData = new GameData
        {
            playerData = playerData
            // enviromentData = ... // später, wenn  EnviromentData hinzugefügt wird
        };

        // Serialisiere GameData und schreibe es in die Datei
        // BinaryFormatter gilt scheinbar als veraltet und unsicher

        // BinaryFormatter bf = new BinaryFormatter();
        // FileStream file = File.Create(saveFilePath);
        // bf.Serialize(file, gameData);
        // file.Close();

        // Serialisiere GameData zu JSON
        string json = JsonUtility.ToJson(gameData);

        // Schreibe JSON in Datei
        File.WriteAllText(saveFilePath, json);

        Debug.Log("Spiel gespeichert.");
    }

    // Methode zum Laden des Spiels
    public void LoadGame(PlayerMovement playerMovement, HealthSystem healthSystem, PlayerInventory playerInventory, SoulShardSystem soulShardSystem)
    {
        if (File.Exists(saveFilePath))
        {
            // Lese die Datei und deserialisiere GameData
            // BinaryFormatter bf = new BinaryFormatter();
            // FileStream file = File.Open(saveFilePath, FileMode.Open);
            // GameData gameData = (GameData)bf.Deserialize(file);
            // file.Close();

            string json = File.ReadAllText(saveFilePath);

            // Deserialisiere JSON zu GameData
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            // Wende die geladenen Daten an
            //ApplyLoadedData(gameData, playerMovement, healthSystem, playerInventory, soulShardSystem);
            ApplyLoadedData(gameData, playerMovement, healthSystem);
            Debug.Log("Spiel geladen.");
        }
        else
        {
            Debug.LogWarning("Keine Speicherdatei gefunden.");
        }
    }

    //private void ApplyLoadedData(GameData gameData, PlayerMovement playerMovement, HealthSystem healthSystem, PlayerInventory playerInventory, SoulShardSystem soulShardSystem)
    private void ApplyLoadedData(GameData gameData, PlayerMovement playerMovement, HealthSystem healthSystem)
    {
        PlayerData data = gameData.playerData;

        // Setze die Gesundheit
        healthSystem.currentHealth = data.currentHealth;

        // Setze respawnHealth
        healthSystem.respawnHealth = data.respawnHealth;

        // Finde den Checkpoint mit der gespeicherten ID und setze die Position
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

        // Setze die gesammelten Items
        // playerInventory.collectedItems.Clear();
        // foreach (var itemName in data.collectedItems)
        // {
        //     Item item = CreateItemByName(itemName);
        //     //Item item = ItemFactory.createItem();
        //     if (item != null)
        //     {
        //         playerInventory.collectedItems.Add(item);
        //     }
        // }

        // // Setze die Anzahl der Soul Shards
        // soulShardSystem.SetSoulShardCount(data.soulShardCount);
    }

    // private Item CreateItemByName(string itemName)
    // {
    //     // Implementiert eine Logik, um ein Item anhand seines Namens zu erstellen
    //     // Beispiel mit einer einfachen Factory-Methode:

    //     switch (itemName)
    //     {
    //         case "HealthItem":
    //             return new HealthItem(itemName, null, 50f); // Passe die Parameter an
    //         case "PowerUpItem":
    //             return new PowerUpItem();
    //         // Füge weitere Fälle für andere Item-Typen hinzu
    //         default:
    //             Debug.LogWarning("Unbekanntes Item: " + itemName);
    //             return null;
    //     }
    // }

}
