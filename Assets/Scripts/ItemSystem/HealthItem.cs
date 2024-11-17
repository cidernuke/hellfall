using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour, IItem
{
    public string itemName { get; set; }
    public Sprite itemSprite { get; set; }
    private float healthAmount;
    [SerializeField]private Potion potion;
    void Start()
    {
        itemName = potion.itemName;
        itemSprite = potion.itemSprite;
        healthAmount = potion.healthAmount;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            HealthSystem playerHealth = collider.GetComponent<HealthSystem>();
            if (playerHealth.currentHealth < playerHealth.startingHealth)
            {
                playerHealth.AddHealth(healthAmount);
                Destroy(gameObject);

            }
        }
    }
}

