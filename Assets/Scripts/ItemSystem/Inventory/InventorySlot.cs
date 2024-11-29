using System.Collections;
using ItemSystem.Abstract;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot
{
    public Item storedItem;
    private VisualElement hotkey;

    public ItemData itemData;

    public void Initialize(VisualElement slotVisualElement)
    {
        hotkey = slotVisualElement;
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
        storedItem.use();
        //after use, delete item
        //problem: instance of item is not really deleted
        storedItem = null;
        UpdateSlotVisual();
    }
    public void DropItem(Vector3 playerPosition, Vector3 dropOffset)
    {
        //get player position
        Vector3 dropPosition = playerPosition + dropOffset;
        //falls überhaupt ein Item da ist
        if (storedItem != null)
        {
            //switch basierend auf dem Namen des Items
            switch (itemData.itemType)
            {
                case ItemType.ModifierItem:
                    Debug.Log("PowerUpItem was dropped");
                    //Lade die Resource
                    GameObject resource = null;
                    //resource Laden
                    resource = Resources.Load<GameObject>("Prefabs/PowerUp");
                    //prüfen ob das Laden funktioniert hat
                    if (resource == null)
                    {
                        Debug.LogError("Failed to load HealthItem(new) prefab");
                        return;
                    }

                    //erstelle Instanz des Prefabs
                    GameObject.Instantiate(resource, dropPosition, Quaternion.identity);
                    break;

                case ItemType.HealthItem:
                    Debug.Log("HealthItem was dropped");
                    //Lade die Resource
                    var resourceH = Resources.Load<GameObject>("Prefabs/HealthItem(new)");
                    if (resourceH == null)
                    {
                        Debug.LogError("Failed to load HealthItem(new) prefab");
                        return;
                    }
                    //erstelle Instanz des Prefabs
                    GameObject.Instantiate(resourceH, dropPosition, Quaternion.identity);
                    break;

                default:
                    Debug.LogError("Unknown ItemType: " + itemData.itemType);
                    break;
            }

            Debug.Log($"Dropping item: {storedItem.itemName}");

            UpdateSlotVisual();
        }
    }

    private void UpdateSlotVisual()
    {
        if (hotkey != null)
        {
            if (storedItem != null)
            {
                //setze das Bild des Hotkeys auf das Bild des Items
                hotkey.style.backgroundImage = new StyleBackground(storedItem.itemSprite);
            }
            else
            {
                hotkey.style.backgroundImage = null;
            }
        }
    }
}
