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

    public WeaponStats weaponStats; // reference to weapon stats

    public ShortRangeWeapon(string itemName, Sprite itemSprite, bool isFire, bool isIce) 
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.weaponStats = new WeaponStats(true);
        //change modifiers based on the type of weapon
        if(isFire)
        {
            //Example Values
            this.weaponStats.SetDamageModifier(0);
            this.weaponStats.SetCooldownModifier(5);
        }
        if(isIce)
        {
            //Example Values
            this.weaponStats.SetDamageModifier(10);
            this.weaponStats.SetCooldownModifier(0);
        }
    }
    public override bool use(){
                      
            Debug.Log("Item was used");
            return true;
    }       
        
}
