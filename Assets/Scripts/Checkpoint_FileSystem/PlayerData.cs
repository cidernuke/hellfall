using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerData
{
    // ID of the latest Checkpoints
    public int lastCheckpointID;

    // Current health of the player
    public float currentHealth;
    public float respawnHealth;

    // Collected Items as List with the item-names
    public List<string> collectedItemNames;

    // Current amount of soulShards
    public int soulShardCount;

    public string lastSceneName;

    //public PlayerData(HealthSystem healthSystem, List<ItemSystem.Abstract.Item> playerItems, SoulShardSystem soulShardSystem, int lastCheckpointID)
    public PlayerData(HealthSystem healthSystem, int lastCheckpointID, SoulShardSystem soulShardSystem, InventorySystem inventorySystem)
    {
        // Save latest checkpointID
        this.lastCheckpointID = lastCheckpointID;

        lastSceneName = SceneManager.GetActiveScene().name;

        // Save health
        currentHealth = healthSystem.currentHealth;

        // Save respawnHealth
        respawnHealth = healthSystem.respawnHealth;

        //Save the current amout of Soul Shards
        soulShardCount = soulShardSystem.GetSoulShardCount();

        collectedItemNames = new List<string>();
        foreach (var slot in inventorySystem.slots)
        {
            if (slot.storedItem != null)
            {
                collectedItemNames.Add(slot.storedItem.itemName);
            }
            else
            {
                collectedItemNames.Add(null); // Platzhalter für leere Slots
            }
        }
    }
}
