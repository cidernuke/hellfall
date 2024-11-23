using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot
{
    public ItemData storedItem;
    private VisualElement hotkey;

    public void Initialize(VisualElement slotVisualElement)
    {
        hotkey = slotVisualElement;
        UpdateSlotVisual();
    }

    public void StoreItem(ItemData itemData)
    {
        storedItem = itemData;
        UpdateSlotVisual();
    }

    public void DropItem()
    {
        if (storedItem != null)
        {
            Debug.Log($"Dropping item: {storedItem.itemName}");
            storedItem = null;
        }
        UpdateSlotVisual();
    }

    private void UpdateSlotVisual()
    {
        if (hotkey != null)
        {
            if (storedItem != null)
            {
                hotkey.style.backgroundImage = new StyleBackground(storedItem.itemSprite);
            }
            else
            {
                hotkey.style.backgroundImage = null;
            }
        }
    }
}
