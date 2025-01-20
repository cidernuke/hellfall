using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using ItemSystem.Abstract;
using ItemSystem.Items;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    // Singleton-Pattern
    public static SaveManager Instance;

    // Path to the save file
    private string saveFilePath;

    public bool isLoadingFromSave = false;

    private void Awake()
    {
        // Singleton-Implementierung
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            print("SaveManager initialisiert");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        saveFilePath = Application.persistentDataPath + "/savegame.dat";
    }

    // Method to save the game
    public void SaveGame(PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
    {
        print("Saving Game");

        // //Load existing GameData
        GameData existingData = LoadGameData();
        if (existingData == null)
            existingData = new GameData(); // in case there is none

        // Create new PlayerData-Object
        PlayerData playerData = new PlayerData(
            healthSystem,
            playerMovement.GetLastCheckpointID(),
            soulShardSystem,
            inventorySystem
        );

        //Merge existingData with PlayerData
        existingData.playerData = playerData;

        // // 4) Bestzeiten + Zwischenzeiten vom TimerSystem (nur wenn du sie dort verwaltest)
        // existingData.bestLevelTimes = TimerSystem.Instance.GetAllBestTimes();
        existingData.currentLevelTimes = TimerSystem.Instance.GetAllCurrentTimes();

        // Creates GameData and adds PlayerData
        // GameData gameData = new GameData
        // {
        //     playerData = playerData
        // };

        // Serialize GameData to JSON
        //string json = JsonUtility.ToJson(gameData);
        string json = JsonUtility.ToJson(existingData);

        // Write JSON into file
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

    /// <summary>
    /// Diese Methode wird vom TimerSystem oder anderen Klassen aufgerufen,
    /// um TEIL-Bereiche von GameData zu ändern (z.B. nur Bestzeiten).
    /// </summary>
    public void SaveGameData(GameData partialData)
    {
        // 1) Vorhandene Daten laden
        GameData existingData = LoadGameData();
        if (existingData == null)
            existingData = new GameData();

        // 2) Nur die Felder überschreiben, die in partialData != null sind
        // (So wird nichts gelöscht, was du nicht aktualisieren willst.)
        if (partialData.playerData != null)
            existingData.playerData = partialData.playerData;

        if (partialData.bestLevelTimes != null)
            existingData.bestLevelTimes = partialData.bestLevelTimes;

        if (partialData.currentLevelTimes != null)
            existingData.currentLevelTimes = partialData.currentLevelTimes;

        // Optional: Weitere Felder in GameData hier ergänzen, falls vorhanden.

        // 3) Jetzt alles wieder speichern
        string json = JsonUtility.ToJson(existingData);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("SaveGameData: Teil-Daten übernommen und gespeichert unter: " + saveFilePath);
    }

    // Methoad to load old game
    public void LoadGame(PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
    {
        GameData gameData = LoadGameData();
        if (gameData != null)
        {
            // Anwenden der geladenen Daten
            ApplyLoadedData(gameData, playerMovement, healthSystem, soulShardSystem, inventorySystem);

            if (gameData.bestLevelTimes != null)
            {
                TimerSystem.Instance.SetAllBestTimes(gameData.bestLevelTimes);
            }
            if (gameData.currentLevelTimes != null)
            {
                TimerSystem.Instance.SetAllCurrentTimes(gameData.currentLevelTimes);
            }
            Debug.Log("Spiel geladen.");
        }
        else
        {
            Debug.LogWarning("Keine Speicherdatei gefunden.");
        }
    }

    private void ApplyLoadedData(GameData gameData, PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
    {
        PlayerData data = gameData.playerData;

        // Reset Health
        healthSystem.currentHealth = data.currentHealth;

        healthSystem.respawnHealth = data.respawnHealth;

        // Set SoulShards
        soulShardSystem.SetSoulShardCount(data.soulShardCount);


        //Set Items
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

        // Findes checkpoint with the saved ID und set the player position
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

    /// <summary>
    /// Liefert lediglich die PlayerData zurück
    /// </summary>
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

    public GameData LoadGameData()
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

    /// <summary>
    /// Laden der GameData als Objekt
    /// </summary>
    public void LoadSceneFromSave()
    {
        PlayerData data = LoadPlayerData();
        if (data == null)
        {
            Debug.LogWarning("Keine Speicherdatei gefunden oder Daten null.");
            return;
        }

        // SzeneName aus PlayerData auslesen
        string sceneName = data.lastSceneName;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("lastSceneName im Save ist leer. Lade keine Szene.");
            return;
        }

        isLoadingFromSave = true;

        // Lade die gespeicherte Szene
        SceneManager.LoadScene(sceneName);
    }
}




// // Method to save the game
// public void SaveGame(PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
// {
//     print("Saving Game");

//     // //Load existing GameData
//     // GameData existingData = LoadGameData();
//     // if (existingData == null)
//     //     existingData = new GameData(); // in case there is none

//     // Create new PlayerData-Object
//     PlayerData playerData = new PlayerData(
//         healthSystem,
//         playerMovement.GetLastCheckpointID(),
//         soulShardSystem,
//         inventorySystem
//     );

//     //Merge existingData with PlayerData
//     // existingData.playerData = playerData;

//     // // 4) Bestzeiten + Zwischenzeiten vom TimerSystem (nur wenn du sie dort verwaltest)
//     // existingData.bestLevelTimes = TimerSystem.Instance.GetAllBestTimes();
//     // existingData.currentLevelTimes = TimerSystem.Instance.GetAllCurrentTimes();

//     // Creates GameData and adds PlayerData
//     GameData gameData = new GameData
//     {
//         playerData = playerData
//     };

//     // Serialize GameData to JSON
//     string json = JsonUtility.ToJson(gameData);

//     // Write JSON into file
//     File.WriteAllText(saveFilePath, json);

//     //Check if the file was written and the integrity
//     if (File.Exists(saveFilePath))
//     {
//         string writtenContent = File.ReadAllText(saveFilePath);
//         if (writtenContent == json)
//         {
//             print("Das Spiel wurde erfolgreich gespeichert und überprüft.");
//         }
//         else
//         {
//             print("Der Dateiinhalt stimmt nicht mit dem erwarteten Inhalt überein.");
//         }
//     }
//     else
//     {
//         print("Das Spiel wurde nicht gespeichert.");
//     }
// }