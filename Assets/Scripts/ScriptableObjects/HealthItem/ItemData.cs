using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public float healthAmount;
}
