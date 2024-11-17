using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory System/Items/HealthItem/Potion")]
public class Potion : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public float healthAmount;
}
