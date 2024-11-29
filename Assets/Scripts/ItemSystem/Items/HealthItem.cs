using System.Collections;
using System.Collections.Generic;
using ItemSystem.Abstract;
using UnityEngine;

public class HealthItem : Item
{
    private float healthAmount;  
    public HealthSystem playerHealth;

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
            Debug.Log("Item was used");
            return false;
        }
    
}
