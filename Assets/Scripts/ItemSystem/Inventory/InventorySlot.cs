using System.Collections;
using ItemSystem.Abstract;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventorySlot
{
    public Item storedItem;
    private VisualElement hotkey;

    public ItemData itemData;

    public void Initialize(VisualElement slotVisualElement)
    {
        hotkey = slotVisualElement;
        if (hotkey == null)
        {
            //Debug.LogError("Could not find VisualElement with name Hotkey");
        }
        UpdateSlotVisual();
    }

    public void StoreItem(Item item, ItemData itemData)
    {
        this.storedItem = item;
        this.itemData = itemData;
        UpdateSlotVisual();
    }

    public void useItem()
    {
        if(storedItem == null)
        {
            //Debug.Log("No item to use");
            return;
        }
        storedItem.use();
        //after use, delete item
        //problem: instance of item is not really deleted
        storedItem = null;
        itemData = null;
        UpdateSlotVisual();
    }
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
                    //resource Laden
                    GameObject resource = Resources.Load<GameObject>("Prefabs/PowerUp");
                    // check if loading of resource worked
                    if (resource == null)
                    {
                        Debug.LogError("Failed to load HealthItem(new) prefab");
                        return;
                    }

                    // create instance of the prefab
                    GameObject.Instantiate(resource, dropPosition, Quaternion.identity);
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
                    break;

                default:
                    Debug.LogError("Unknown ItemType: " + itemData.itemType);
                    break;
            }

            //Debug.Log($"Dropping item: {storedItem.itemName}");

            UpdateSlotVisual();
        }
    }

    private void UpdateSlotVisual()
    {
        if (hotkey != null)
        {
            if (storedItem != null)
            {
                // set image of hotkey to the image of the item
                hotkey.style.backgroundImage = new StyleBackground(storedItem.itemSprite);
                //Debug.Log("InvSlot: Item displayed");
            }
            else
            {
                hotkey.style.backgroundImage = null;
                //Debug.Log("InvSlot: No item to display");
            }
        }
    }

    public void ClearSlot()
{
    storedItem = null;
    itemData = null;
    UpdateSlotVisual();
}

}
