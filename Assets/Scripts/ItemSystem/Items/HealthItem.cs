using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem.Abstract;

namespace ItemSystem.Items
{    
    public class HealthItem : Item
    {        
        private float healthAmount;
        private Collider2D collider;
            
        public HealthItem(ItemData itemData)
        {
            itemName = itemData.itemName;
            itemSprite = itemData.itemSprite;
            healthAmount = itemData.healthAmount;
            this.collider = itemData.collider;
        }

        public void OnTriggerEnter2D()
        {
            if (collider.tag == "Player")
            {
                HealthSystem playerHealth = collider.GetComponent<HealthSystem>();
                if (playerHealth.currentHealth < playerHealth.startingHealth)
                {
                    playerHealth.AddHealth(healthAmount);                    
                    isDestroyed = true;
                }
            }
            
        }
    }
}


