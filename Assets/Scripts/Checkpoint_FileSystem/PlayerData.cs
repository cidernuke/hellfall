using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // ID of the latest Checkpoints
    public int lastCheckpointID;

    // Current health of the player
    public float currentHealth;
    public float respawnHealth;

    // Collected Items as List with the item-names
    //public List<string> collectedItems;

    // Current amount of soulShards
    //public int soulShardCount;

    //public PlayerData(HealthSystem healthSystem, List<ItemSystem.Abstract.Item> playerItems, SoulShardSystem soulShardSystem, int lastCheckpointID)
    public PlayerData(HealthSystem healthSystem, int lastCheckpointID)
    {
        // Save latest checkpointID
        this.lastCheckpointID = lastCheckpointID;

        // Save health
        currentHealth = healthSystem.currentHealth;

        // Save respawnHealth
        respawnHealth = healthSystem.respawnHealth;

        // // Safe collected items
        // collectedItems = new List<string>();
        // foreach (var item in playerItems)
        // {
        //     collectedItems.Add(item.itemName);
        // }

        // // Save current amount of soulShards
        // soulShardCount = soulShardSystem.GetSoulShardCount();
    }
}
