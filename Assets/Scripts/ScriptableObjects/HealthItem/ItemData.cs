using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the data for an item in the game.
/// </summary>     

[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory System/Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public ItemType itemType;

    [Header("Health Item")]
    public float healthAmount;

    [Header("Modifier Item")]
    public float duration;
    public int amount;
    public string description;

    [Header("Weapon Item")]
    public WeaponStats weaponStats;

}

public enum ItemType
{
    ModifierItem,
    HealthItem,
    WeaponItem
    // Weitere Item-Typen können hier hinzugefügt werden
}
