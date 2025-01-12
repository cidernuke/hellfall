using System.Collections;
using System.Collections.Generic;
using ItemSystem.Abstract;
using UnityEngine;

/// <summary>
/// Class for health items in the game.
/// </summary>

public class ShortRangeWeapon : Item
{
    public HealthSystem playerHealth; // reference to healthsystem

    public ShortRangeWeapon(string itemName, Sprite itemSprite) 
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
    }
    public override bool use(){
                      
            Debug.Log("Item was used");
            return true;
    }       
        
}
