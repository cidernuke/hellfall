using System;
using System.Collections.Generic;
using UnityEngine;
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

    //Altar Costs
     public int vitalityCost;
    public int strengthCost;
    public int intelligenceCost;

    public int vitalityMultiplierCount;
    public int strengthMultiplierCount;
    public int intelligenceMultiplierCount;

    //Array for the current saved costs
    public int[] vitalityCosts;
    public int[] strengthCosts;
    public int[] intelligenceCosts;

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
    public PlayerData(HealthSystem healthSystem, int lastCheckpointID, SoulShardSystem soulShardSystem, InventorySystem inventorySystem, KeySystem keySystem, PlayerController playerController, AltarGUI altarGUI)
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


        //Altar Costs
        if (altarGUI != null)
        {
            this.vitalityCost = altarGUI.vitalityCost;
            this.strengthCost = altarGUI.strengthCost;
            this.intelligenceCost = altarGUI.intelligenceCost;

            this.vitalityMultiplierCount = altarGUI.vitalityMultiplierCount;
            this.strengthMultiplierCount = altarGUI.strengthMultiplierCount;
            this.intelligenceMultiplierCount = altarGUI.intelligenceMultiplierCount;

            // Arrays kopieren
            this.vitalityCosts = (int[])altarGUI.vitalityCosts.Clone();
            this.strengthCosts = (int[])altarGUI.strengthCosts.Clone();
            this.intelligenceCosts = (int[])altarGUI.intelligenceCosts.Clone();
        }
        else
        {
            Debug.LogWarning("AltarGUI not found, no altar cost data saved!");
        }
    }
}
