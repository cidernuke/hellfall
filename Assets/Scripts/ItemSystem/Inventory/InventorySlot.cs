using ItemSystem.Abstract;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventorySlot
{
    public Item storedItem;
    private VisualElement hotkey;

    public ItemData itemData;

    /// <summary>
    /// Initializes the inventory slot with a visual element.
    /// </summary>
    /// <param name="slotVisualElement">The visual element representing the slot.</param>
    public void Initialize(VisualElement slotVisualElement)
    {
        hotkey = slotVisualElement;
        if (hotkey == null)
        {
            Debug.LogError("Could not find VisualElement with name: " + slotVisualElement);
        }
        UpdateSlotVisual();
    }

    /// <summary>
    /// Stores an item and its data in the inventory slot.
    /// </summary>
    /// <param name="item">The item to store.</param>
    /// <param name="itemData">The data associated with the item.</param>
    public void StoreItem(Item item, ItemData itemData)
    {
        if (item == null || itemData == null)
        {
            Debug.Log("Item or itemData is null");
            UpdateSlotVisual();
            return;
        }
        else
        {
            this.storedItem = item;
            this.itemData = itemData;
            UpdateSlotVisual();
        }
    }

    /// <summary>
    /// Uses the stored item.
    /// </summary>
    public void useItem()
    {
        if (storedItem == null)
        {
            return;
        }
        storedItem.use();
        //after use, delete item
        //problem: instance of item is not really deleted
        storedItem = null;
        itemData = null;
        UpdateSlotVisual();
    }

    /// <summary>
    /// Drops the stored item at a specified position with an offset.
    /// </summary>
    /// <param name="playerPosition">The position of the player.</param>
    /// <param name="dropOffset">The offset from the player's position where the item will be dropped.</param>
    public void DropItem(Vector3 playerPosition, Vector3 dropOffset)
    {
        //get player position
        Vector3 dropPosition = playerPosition + dropOffset;
        // if there even is an item
        if (storedItem != null)
        {
            // switch based on the name of the item
            switch (itemData.itemType)
            {
                case ItemType.ModifierItem:
                    Debug.Log("PowerUpItem was dropped");
                    //Load resource
                    GameObject resource = Resources.Load<GameObject>("Prefabs/PowerUp");
                    // check if loading of resource worked
                    if (resource == null)
                    {
                        Debug.LogError("Failed to load PowerUp prefab");
                        return;
                    }

                    // create instance of the prefab
                    GameObject.Instantiate(resource, dropPosition, Quaternion.identity);
                    // clear the slot, so the visual representation is updated and the slot can't be dropped multiple times
                    ClearSlot();
                    break;

                case ItemType.HealthItem:
                    Debug.Log("HealthItem was dropped");
                    // load the resource
                    var resourceH = Resources.Load<GameObject>("Prefabs/HealthItem(new)");
                    if (resourceH == null)
                    {
                        Debug.LogError("Failed to load HealthItem(new) prefab");
                        return;
                    }
                    // create instance of the prefab
                    GameObject.Instantiate(resourceH, dropPosition, Quaternion.identity);
                    ClearSlot();
                    break;

                case ItemType.ShortRangeWeapon:
                    Debug.Log("WeaponItem was dropped");
                    GameObject resourceW = Resources.Load<GameObject>("Prefabs/Weapons/BasicSword");
                    if (resourceW == null)
                    {
                        Debug.LogError("Failed to load WeaponItem prefab");
                        return;
                    }
                    GameObject.Instantiate(resourceW, dropPosition, Quaternion.identity);
                    ClearSlot();
                    break;
                
                case ItemType.LongRangeWeapon:
                    Debug.Log("WeaponItem was dropped");
                    GameObject resourceLR = Resources.Load<GameObject>("Prefabs/Weapons/BasicLongRange");
                    if (resourceLR == null)
                    {
                        Debug.LogError("Failed to load WeaponItem prefab");
                        return;
                    }
                    GameObject.Instantiate(resourceLR, dropPosition, Quaternion.identity);
                    ClearSlot();
                    break;

                default:
                    Debug.LogError("Unknown ItemType: " + itemData.itemType);
                    break;
            }

            //UpdateSlotVisual();
        }
        else
        {
            Debug.Log("No item to drop");
        }
    }

    /// <summary>
    /// Updates the visual representation of the slot.
    /// </summary>
    private void UpdateSlotVisual()
    {
        if (hotkey != null)
        {
            if (storedItem != null)
            {
                // set image of hotkey to the image of the item
                hotkey.style.backgroundImage = new StyleBackground(storedItem.itemSprite);
            }
            else
            {
                hotkey.style.backgroundImage = null;
            }
        }
    }

    /// <summary>
    /// Clears the inventory slot, removing the stored item and its data.
    /// </summary>
    public void ClearSlot()
    {
        storedItem = null;
        itemData = null;
        UpdateSlotVisual();
    }

}
