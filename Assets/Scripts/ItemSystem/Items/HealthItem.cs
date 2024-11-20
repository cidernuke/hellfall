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
        public HealthItem(ItemData itemData)
        {
            itemName = itemData.itemName;
            itemSprite = itemData.itemSprite;
            healthAmount = itemData.healthAmount;
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == "Player")
            {
                HealthSystem playerHealth = collider.GetComponent<HealthSystem>();
                if (playerHealth.currentHealth < playerHealth.startingHealth)
                {
                    playerHealth.AddHealth(healthAmount);                    

                }
            }
        }
    }
}


