using ItemSystem.Abstract;
using UnityEngine;

public class LongRangeWeapon : Item
{
    public HealthSystem playerHealth; // reference to healthsystem

    public LongRangeWeapon(string itemName, Sprite itemSprite) 
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
    }
    public override bool use(){
                      
            Debug.Log("Item was used");
            return true;
    }       
        
}
