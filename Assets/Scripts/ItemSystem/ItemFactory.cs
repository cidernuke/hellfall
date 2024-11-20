using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using ItemSystem.Abstract;
using ItemSystem.Items;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{    
    [SerializeField] private ItemData itemData;
    
    private HealthItem healthItem;
    
    public void Awake()
    {
        createHealthItem();
    }

    private void createHealthItem()
    {
        if(itemData.isHealthItem)
        {
            healthItem = new HealthItem(itemData);
            healthItem.OnTriggerEnter2D();
            if(healthItem.isDestroyed)
            {
                Destroy(gameObject);
            }
            
        }
    }

    

    
}
