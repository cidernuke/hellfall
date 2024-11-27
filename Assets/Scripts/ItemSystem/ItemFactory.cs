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

    /// <summary>
    /// Called when the player collides with the item.
    /// If the player collides with an item, the item is used and destroyed.
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (itemData.itemType)
        {
            case ItemType.ModifierItem:
                if (powerUpItem.use())
                {
                    Destroy(gameObject);
                }
                break;

            case ItemType.HealthItem:
                HealthSystem playerHealth = collider.GetComponent<HealthSystem>();
                if (healthItem.use(playerHealth))
                {
                    Destroy(gameObject);
                }
                break;
            // Weitere Fälle für andere Item-Typen können hier hinzugefügt werden
            default:
                Debug.LogError("Unknown ItemType: " + itemData.itemType);
                break;
        }
    }


    /// <summary>
    /// Creates an item based on the item type.
    /// </summary>
    private void createItem()
    {
        switch (itemData.itemType)
        {
            case ItemType.ModifierItem:
                powerUpItem = new PowerUpItem();
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
