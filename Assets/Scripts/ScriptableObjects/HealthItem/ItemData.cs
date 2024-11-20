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
    public bool isHealthItem;
    public bool isModifierItem;
    public bool isWeaponItem;    
    public Collider2D collider;
}
