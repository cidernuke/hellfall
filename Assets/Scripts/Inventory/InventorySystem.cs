using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class InventorySystem : MonoBehaviour
{
    public InventorySlot[] slots;

    public UIDocument uiDocument;

    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if(uiDocument == null){
            Debug.LogError("Could not find UIDocument component on GameObject");
            return;
        }
        //For the 3 Itemslots "Hotkey1", "Hotkey2", "Hotkey3"
        for (int i = 0; i < slots.Length; i++)
        {
            //Regex to find the VisualElement with the name "Hotkey" + i
            VisualElement hotkey = uiDocument.rootVisualElement.Q<VisualElement>($"Hotkey{i + 1}");
            slots[i].Initialize(hotkey);
            if(hotkey == null){
                Debug.LogError("Could not find VisualElement with name Hotkey" + i + 1);
            }
        }
    }

    public bool AddItemToFirstAvailableSlot(Item item)
    {
        foreach (var slot in slots)
        {
            if (slot.storedItem == null)
            {
                slot.StoreItem(item);
                return true;
            }
        }
        Debug.Log("Inventory full!");
        return false;
    }

    public void DropItemFromSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            slots[slotIndex].DropItem();
        }
    }
}
