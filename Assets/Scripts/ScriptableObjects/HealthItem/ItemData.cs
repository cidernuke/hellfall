using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory System/Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public float healthAmount;
    public ItemType itemType;       
    
}

public enum ItemType
{
    ModifierItem,
    HealthItem,
    WeaponItem
    // Weitere Item-Typen können hier hinzugefügt werden
}
