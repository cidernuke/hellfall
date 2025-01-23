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

    public Dictionary<String, float> characterStats = new Dictionary<string, float>();

    //Key-Logic
    public int keyCounter;

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
                collectedItemNames.Add(null); // Platzhalter für leere Slots
            }
        }

        CharacterStats stats = playerController.playerStats;

        characterStats.Add("vitalityBase", stats.vitality.GetBaseValue());
        characterStats.Add("strengthBase", stats.strength.GetBaseValue());
        characterStats.Add("intelligenceBase", stats.intelligence.GetModifier());

        characterStats.Add("maxHealthBase", stats.maxHealth.GetBaseValue());
        characterStats.Add("maxHealthMod", stats.maxHealth.GetModifier());

        characterStats.Add("closeDamageBase", stats.closeDamage.GetBaseValue());
        characterStats.Add("closeDamageMod", stats.closeDamage.GetModifier());

        characterStats.Add("rangedDamageBase", stats.rangedDamage.GetBaseValue());
        characterStats.Add("rangedDamageMod", stats.rangedDamage.GetModifier());
        characterStats.Add("rangedCooldownBase", stats.rangedCooldown.GetBaseValue());
        characterStats.Add("rangedCooldownMod", stats.rangedCooldown.GetModifier());
        characterStats.Add("rangedRangeBase", stats.rangedRange.GetBaseValue());
        characterStats.Add("rangedRangeMod", stats.rangedRange.GetModifier());

        





    }
}
