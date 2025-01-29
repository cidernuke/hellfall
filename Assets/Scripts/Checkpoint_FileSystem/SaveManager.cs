using System;
using System.IO;
using UnityEngine;
using ItemSystem.Abstract;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    // Singleton pattern
    public static SaveManager Instance;

    // Path to the save file
    public string saveFilePath;
    public bool isLoadingFromSave = false;

    private void Awake()
    {
        // Singleton implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            print("SaveManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the path to the save file
    /// </summary>
    private void Start()
    {
        saveFilePath = Application.persistentDataPath + "/savegame.dat";
    }

    /// <summary>
    /// Save the game
    /// </summary>
    /// <param name="playerMovement"></param>
    /// <param name="healthSystem"></param>
    /// <param name="soulShardSystem"></param>
    /// <param name="inventorySystem"></param>
    /// <param name="keySystem"></param>
    /// <param name="playerController"></param>
    public void SaveGame(PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem, KeySystem keySystem, PlayerController playerController)
    {
        print("Saving Game");

        // Load existing GameData
        GameData existingData = LoadGameData();
        if (existingData == null)
            existingData = new GameData(); // in case there is none

        // 1) Get AltarGUI
        AltarGUI altarGUI = FindObjectOfType<AltarGUI>();

        // Create new PlayerData object
        PlayerData playerData = new PlayerData(
            healthSystem,
            playerMovement.GetLastCheckpointID(),
            soulShardSystem,
            inventorySystem,
            keySystem,
            playerController,
            altarGUI
        );

        // Merge existingData with PlayerData
        existingData.playerData = playerData;

        // Best times are saved in the TimerSystem
        existingData.currentLevelTimes = TimerSystem.Instance.GetAllCurrentTimes();

        // Serialize GameData to JSON
        string json = JsonUtility.ToJson(existingData);

        // Write JSON into file
        File.WriteAllText(saveFilePath, json);

        // Check if the file was written and the integrity
        if (File.Exists(saveFilePath))
        {
            string writtenContent = File.ReadAllText(saveFilePath);
            if (writtenContent == json)
            {
                print("The game was saved successfully and verified.");
            }
            else
            {
                print("The file content does not match the expected content.");
            }
        }
        else
        {
            print("The game was not saved.");
        }
    }

    /// <summary>
    /// This method is called by the TimerSystem or other classes to change PARTS of GameData (e.g., only best times).
    /// </summary>
    public void SaveGameData(GameData partialData)
    {
        // 1) Load existing data
        GameData existingData = LoadGameData();
        if (existingData == null)
            existingData = new GameData();

        // 2) Only overwrite fields that are not null in partialData
        // (This way, nothing is deleted that you don't want to update.)
        if (partialData.playerData != null)
            existingData.playerData = partialData.playerData;

        if (partialData.bestLevelTimes != null)
            existingData.bestLevelTimes = partialData.bestLevelTimes;

        if (partialData.currentLevelTimes != null)
            existingData.currentLevelTimes = partialData.currentLevelTimes;

        // Optional: Add more fields in GameData here if available.

        // 3) Save everything again
        string json = JsonUtility.ToJson(existingData);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("SaveGameData: Partial data adopted and saved under: " + saveFilePath);
    }

    /// <summary>
    /// Load the game
    /// </summary>
    /// <param name="playerMovement"></param>
    /// <param name="healthSystem"></param>
    /// <param name="soulShardSystem"></param>
    /// <param name="inventorySystem"></param>
    /// <param name="keySystem"></param>
    /// <param name="playerController"></param>
    public void LoadGame(PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem, KeySystem keySystem, PlayerController playerController)
    {
        GameData gameData = LoadGameData();
        if (gameData != null)
        {
            // Apply loaded data
            ApplyLoadedData(gameData, playerMovement, healthSystem, soulShardSystem, inventorySystem, keySystem, playerController);

            if (gameData.bestLevelTimes != null)
            {
                TimerSystem.Instance.SetAllBestTimes(gameData.bestLevelTimes);
            }
            if (gameData.currentLevelTimes != null)
            {
                TimerSystem.Instance.SetAllCurrentTimes(gameData.currentLevelTimes);
            }
            Debug.Log("Game loaded.");
        }
        else
        {
            Debug.LogWarning("No save file found.");
        }
    }

    /// <summary>
    /// Apply loaded data to the game
    /// </summary>
    /// <param name="gameData"></param>
    /// <param name="playerMovement"></param>
    /// <param name="healthSystem"></param>
    /// <param name="soulShardSystem"></param>
    /// <param name="inventorySystem"></param>
    /// <param name="keySystem"></param>
    /// <param name="playerController"></param>
    private void ApplyLoadedData(GameData gameData, PlayerMovement playerMovement, HealthSystem healthSystem, SoulShardSystem soulShardSystem, InventorySystem inventorySystem, KeySystem keySystem, PlayerController playerController)
    {
        PlayerData data = gameData.playerData;

        // Reset Health
        healthSystem.currentHealth = data.currentHealth;
        healthSystem.respawnHealth = data.respawnHealth;

        // Set SoulShards
        soulShardSystem.SetSoulShardCount(data.soulShardCount);

        // Set KeyCounter
        keySystem.SetKeyCount(data.keyCounter);

        // Set Items
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
                    Debug.LogWarning("ItemData for " + itemName + " not found.");
                    // Game keeps running but with empty slot
                    inventorySystem.slots[i].storedItem = null;
                }
            }
            else
            {
                // Ensure empty slot
                inventorySystem.slots[i].storedItem = null;
            }
        }

        // Find checkpoint with the saved ID and set the player position
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

        //apply the loaded PlayerData data to the controllers playerstats
        CharacterStats newPlayerStats = playerController.playerStats;

        newPlayerStats.vitality.SetBaseValue(data.vitalityBase);
        newPlayerStats.strength.SetBaseValue(data.strengthBase);
        newPlayerStats.intelligence.SetBaseValue(data.intelligenceBase);

        newPlayerStats.maxHealth.SetBaseValue(data.maxHealthBase);
        newPlayerStats.maxHealth.SetModifier(data.maxHealthModifier);

        newPlayerStats.closeDamage.SetBaseValue(data.closeDamageBase);
        newPlayerStats.closeDamage.SetModifier(data.closeDamageModifier);

        newPlayerStats.rangedDamage.SetBaseValue(data.rangedDamageBase);
        newPlayerStats.rangedDamage.SetModifier(data.rangedDamageModifier);
        newPlayerStats.rangedCooldown.SetBaseValue(data.rangedCooldownBase);
        newPlayerStats.rangedCooldown.SetModifier(data.rangedCooldownModifier);
        newPlayerStats.rangedRange.SetBaseValue(data.rangedRangeBase);
        newPlayerStats.rangedRange.SetModifier(data.rangedRangeModifier);
    }

    /// <summary>
    /// Returns only the PlayerData
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

    /// <summary>
    /// Load the game data
    /// </summary>
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
            Debug.LogWarning("No save file found.");
            return null;
        }
    }

    /// <summary>
    /// Load the scene from the save file
    /// </summary>
    public void LoadSceneFromSave()
    {
        PlayerData data = LoadPlayerData();
        if (data == null)
        {
            Debug.LogWarning("No save file found or data is null.");
            return;
        }

        // Read scene name from PlayerData
        string sceneName = data.lastSceneName;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("lastSceneName in the save is empty. Not loading any scene.");
            return;
        }

        isLoadingFromSave = true;

        // Load the saved scene
        SceneManager.LoadScene(sceneName);
    }
}