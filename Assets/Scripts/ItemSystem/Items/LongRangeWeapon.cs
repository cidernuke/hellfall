using ItemSystem.Abstract;
using UnityEngine;

public class LongRangeWeapon : Item
{
    public HealthSystem playerHealth; // reference to healthsystem

    public WeaponStats weaponStats; // reference to weapon stats

    public LongRangeWeapon(string itemName, Sprite itemSprite, bool isFire, bool isIce) 
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.weaponStats = new WeaponStats();
        this.weaponStats.isCloseRangeWeapon = true;
        //change modifiers based on the type of weapon
        if(isFire)
        {
            //Example Values
            this.weaponStats.damageModifier = 0;
            this.weaponStats.cooldownModifier = 5;
        }
        if(isIce)
        {
            //Example Values
            this.weaponStats.damageModifier = 10;
            this.weaponStats.cooldownModifier = 0;
        }
    }
    public override bool use(){
                      
            Debug.Log("Item was used");
            return true;
    }       
        
}
