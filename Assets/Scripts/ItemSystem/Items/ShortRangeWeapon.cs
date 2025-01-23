using ItemSystem.Abstract;
using UnityEngine;

/// <summary>
/// Class for health items in the game.
/// </summary>
public class ShortRangeWeapon : Item
{
    public HealthSystem playerHealth; // reference to healthsystem

    /// <summary>
    /// Constructor for ShortRangeWeapon.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <param name="itemSprite">Sprite of the item.</param>
    public ShortRangeWeapon(string itemName, Sprite itemSprite) 
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
    }

    /// <summary>
    /// Method to use the item.
    /// </summary>
    /// <returns>Returns true if the item was used successfully.</returns>
    public override bool use(){
        Debug.Log("Item was used");
        return true;
    }       
}
