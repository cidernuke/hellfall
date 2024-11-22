using System.Collections;
using System.Collections.Generic;
using ItemSystem.Abstract;
using UnityEngine;

/// <summary>
/// Class for health items in the game.
/// </summary>

public class HealthItem : Item
{
    private float healthAmount;

    public HealthItem(string itemName, Sprite itemSprite, float healthAmount)
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.healthAmount = healthAmount;

    }

    /// <summary>
    /// Method to use the health item. 
    /// If the player's health is less than the starting health, the player's health is increased by the health amount of the item.
    /// </summary>
    /// <param name="playerHealth"></param>
    /// <returns>
    /// True if the player's health is less than the starting health, false otherwise.
    /// </returns>
    public override bool use(HealthSystem playerHealth)
    {
        if (playerHealth.currentHealth < playerHealth.startingHealth)
        {
            playerHealth.AddHealth(healthAmount);
            return true;

        }
        return false;
    }

}
