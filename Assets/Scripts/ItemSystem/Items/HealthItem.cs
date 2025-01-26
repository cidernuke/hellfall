using System.Collections;
using System.Collections.Generic;
using ItemSystem.Abstract;
using UnityEngine;

/// <summary>
/// Class for health items in the game.
/// </summary>

public class HealthItem : Item
{
    private float healthAmount; // how much does an item heal
    public HealthSystem playerHealth; // reference to 
    
    public HealthItem(string itemName, Sprite itemSprite, float healthAmount) 
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.healthAmount = healthAmount;
    }
    public override bool use()        {
            
            if (playerHealth.currentHealth < playerHealth.startingHealth)
            {
                playerHealth.AddHealth(healthAmount);                            
                return true;             
            }            
            return false;
        }
}
