using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class InventorySlot
{
    public ItemSystem.Abstract.Item storedItem;
    private VisualElement hotkey;

    public void Initialize(VisualElement slotVisualElement)
    {
        hotkey = slotVisualElement;
        UpdateSlotVisual();
    }

    public void StoreItem(ItemSystem.Abstract.Item item)
    {
        storedItem = item;
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
