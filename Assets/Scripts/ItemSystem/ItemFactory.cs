using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Principal;
using ItemSystem.Abstract;
using ItemSystem.Items;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    Item powerUpItem;
    Item weaponItem;
    Item healthItem;


    public void Awake()
    {
        createItem();
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (itemData.itemType)
        {
            case ItemType.ModifierItem:
                if (powerUpItem.use())
                {
                    Destroy(gameObject);
                }
                break;

            case ItemType.HealthItem:
                HealthSystem playerHealth = collider.GetComponent<HealthSystem>();
                if (healthItem.use(playerHealth))
                {
                    Destroy(gameObject);
                }
                break;
            // Weitere Fälle für andere Item-Typen können hier hinzugefügt werden
            default:
                Debug.LogError("Unknown ItemType: " + itemData.itemType);
                break;
        }
    }



    private void createItem()
    {
        switch (itemData.itemType)
        {
            case ItemType.ModifierItem:
                powerUpItem = new PowerUpItem(itemData.itemName, itemData.itemSprite);
                break;

            case ItemType.HealthItem:
                healthItem = new HealthItem(itemData.itemName, itemData.itemSprite, itemData.healthAmount);
                break;

            // Weitere Fälle für andere Item-Typen können hier hinzugefügt werden
            default:
                Debug.LogError("Unknown ItemType: " + itemData.itemType);
                break;



        }
    }




}
