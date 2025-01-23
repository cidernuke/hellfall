using ItemSystem.Abstract;
using ItemSystem.Items;
using UnityEngine;

/// <summary>
/// Factory class for creating items in the game.
/// </summary>
public class ItemFactory : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    Item powerUpItem;
    Item weaponItem;
    Item healthItem;

    Item shortRangeWeapon;
    Item longRangeWeapon;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    public void Awake()
    {
        createItem();
    }

    /// <summary>
    /// Creates an item based on the item type specified in itemData.
    /// </summary>
    private void createItem()
    {
        switch (itemData.itemType)
        {
            case ItemType.ModifierItem:
                powerUpItem = new PowerUpItem(itemData.itemName, itemData.itemSprite);
                break;

            case ItemType.HealthItem:
                healthItem = new HealthItem(itemData.itemName, itemData.itemSprite, itemData.healthAmount);
                break;
            
            case ItemType.ShortRangeWeapon:
                shortRangeWeapon = new ShortRangeWeapon(itemData.itemName, itemData.itemSprite, itemData.isFire, itemData.isIce);
                break;

            case ItemType.LongRangeWeapon:
                longRangeWeapon = new LongRangeWeapon(itemData.itemName, itemData.itemSprite, itemData.isFire, itemData.isIce);
                break;

            // Additional cases for other item types can be added here
            default:
                Debug.LogError("Unknown ItemType: " + itemData.itemType);
                break;
        }
    }
}
