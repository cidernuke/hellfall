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


    public void Awake()
    {
        createItem();
    }

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

            // Weitere Fälle für andere Item-Typen können hier hinzugefügt werden
            default:
                Debug.LogError("Unknown ItemType: " + itemData.itemType);
                break;
        }
    }
}
