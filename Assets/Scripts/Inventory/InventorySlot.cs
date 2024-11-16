using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : MonoBehaviour
{
    public Item storedItem;
    private VisualElement hotkey;

    public void Initialize(VisualElement slotVisualElement)
    {
        hotkey = slotVisualElement;
        UpdateSlotVisual();
    }

    public void StoreItem(Item item)
    {
        storedItem = item;
        UpdateSlotVisual();
    }

    public void DropItem()
    {
        if (storedItem != null && storedItem.prefab != null)
        {
            Instantiate(storedItem.prefab, transform.position + transform.forward, Quaternion.identity);
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
                hotkey.style.backgroundImage = new StyleBackground(storedItem.icon);
            }
            else
            {
                hotkey.style.backgroundImage = null;
            }
        }
    }
}
