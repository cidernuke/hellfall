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

    //CharacterStats einzeln, da das JSON sonst wahrscheinlich nicht funktioniert
    public float vitalityBase;
    public float strengthBase;
    public float intelligenceBase;
    public float maxHealthBase;
    public float maxHealthModifier;
    public float closeDamageBase;
    public float closeDamageModifier;
    public float rangedDamageBase;
    public float rangedDamageModifier;
    public float rangedCooldownBase;
    public float rangedCooldownModifier;
    public float rangedRangeBase;
    public float rangedRangeModifier;

    //Key-Logic
    public int keyCounter;

    /// <summary>
    /// Constructor for the PlayerData-Class
    /// </summary>
    /// <param name="healthSystem"></param>
    /// <param name="lastCheckpointID"></param>
    /// <param name="soulShardSystem"></param>
    /// <param name="inventorySystem"></param>
    /// <param name="keySystem"></param>
    /// <param name="playerController"></param>
    public PlayerData(HealthSystem healthSystem, int lastCheckpointID, SoulShardSystem soulShardSystem, InventorySystem inventorySystem, KeySystem keySystem, PlayerController playerController)
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

        //Save keyCounter
        keyCounter = keySystem.GetKeyCount();

        //Save Items
        collectedItemNames = new List<string>();
        foreach (var slot in inventorySystem.slots)
        {
            if (slot.storedItem != null)
            {
                collectedItemNames.Add(slot.storedItem.itemName);
            }
            else
            {
                collectedItemNames.Add(null); // Placeholder for empty slots
            }
        }

        //Save Character Stats
        CharacterStats stats = playerController.playerStats;

        //in einzelne floats machen
        vitalityBase = stats.vitality.GetBaseValue();
        strengthBase = stats.strength.GetBaseValue();
        intelligenceBase = stats.intelligence.GetBaseValue();

        maxHealthBase = stats.maxHealth.GetBaseValue();
        maxHealthModifier = stats.maxHealth.GetModifier();

        closeDamageBase = stats.closeDamage.GetBaseValue();
        closeDamageModifier = stats.closeDamage.GetModifier();

        rangedDamageBase = stats.rangedDamage.GetBaseValue();
        rangedDamageModifier = stats.rangedDamage.GetModifier();
        rangedCooldownBase = stats.rangedCooldown.GetBaseValue();
        rangedCooldownModifier = stats.rangedCooldown.GetModifier();
        rangedRangeBase = stats.rangedRange.GetBaseValue();
        rangedRangeModifier = stats.rangedRange.GetModifier();
    }
}
