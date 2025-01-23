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
            Debug.LogError("Could not find VisualElement with name: " + slotVisualElement);
        }
        UpdateSlotVisual();
    }

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
            //Debug.Log($"Item stored: {storedItem.itemName}");
            UpdateSlotVisual();
        }
    }

    public void useItem()
    {
        if (storedItem == null)
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
                    if (itemData.isFire)
                    {
                        Debug.Log("WeaponItem was dropped");
                        GameObject resourcefW = Resources.Load<GameObject>("Prefabs/Weapons/FireSword");
                        if (resourcefW == null)
                        {
                            Debug.LogError("Failed to load WeaponItem prefab");
                            return;
                        }
                        GameObject.Instantiate(resourcefW, dropPosition, Quaternion.identity);
                        ClearSlot();
                    }
                    else if (itemData.isIce)
                    {
                        Debug.Log("WeaponItem was dropped");
                        GameObject resourceiW = Resources.Load<GameObject>("Prefabs/Weapons/IceSword");
                        if (resourceiW == null)
                        {
                            Debug.LogError("Failed to load WeaponItem prefab");
                            return;
                        }
                        GameObject.Instantiate(resourceiW, dropPosition, Quaternion.identity);
                        ClearSlot();
                    }
                    else
                    {
                        Debug.Log("WeaponItem was dropped");
                        GameObject resourceW = Resources.Load<GameObject>("Prefabs/Weapons/BasicSword");
                        if (resourceW == null)
                        {
                            Debug.LogError("Failed to load WeaponItem prefab");
                            return;
                        }
                        GameObject.Instantiate(resourceW, dropPosition, Quaternion.identity);
                        ClearSlot();
                    }
                    break;

                case ItemType.LongRangeWeapon:
                    if (itemData.isFire)
                    {
                        Debug.Log("WeaponItem was dropped");
                        GameObject resourcefR = Resources.Load<GameObject>("Prefabs/Weapons/FireLongRange");
                        if (resourcefR == null)
                        {
                            Debug.LogError("Failed to load WeaponItem prefab");
                            return;
                        }
                        GameObject.Instantiate(resourcefR, dropPosition, Quaternion.identity);
                        ClearSlot();
                    }
                    else if (itemData.isIce)
                    {
                        Debug.Log("WeaponItem was dropped");
                        GameObject resourceiR = Resources.Load<GameObject>("Prefabs/Weapons/IceLongRange");
                        if (resourceiR == null)
                        {
                            Debug.LogError("Failed to load WeaponItem prefab");
                            return;
                        }
                        GameObject.Instantiate(resourceiR, dropPosition, Quaternion.identity);
                        ClearSlot();
                    }
                    else
                    {
                        Debug.Log("WeaponItem was dropped");
                        GameObject resourceLR = Resources.Load<GameObject>("Prefabs/Weapons/BasicLongRange");
                        if (resourceLR == null)
                        {
                            Debug.LogError("Failed to load WeaponItem prefab");
                            return;
                        }
                        GameObject.Instantiate(resourceLR, dropPosition, Quaternion.identity);
                        ClearSlot();
                    }
                    break;

                default:
                    Debug.LogError("Unknown ItemType: " + itemData.itemType);
                    break;
            }

            //Debug.Log($"Dropping item: {storedItem.itemName}");

            //UpdateSlotVisual();
        }
        else
        {
            Debug.Log("No item to drop");
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
