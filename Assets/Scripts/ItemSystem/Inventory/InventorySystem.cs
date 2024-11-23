using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ItemSystem.Abstract;

public class InventorySystem : MonoBehaviour
{
    public InventorySlot[] slots;
    [SerializeField] protected UIDocument uiDocument;
    protected VisualElement root;


    void Start()
    {
        root = uiDocument.rootVisualElement;
        if (uiDocument == null)
        {
            Debug.LogError("Could not find UIDocument component on GameObject");
            return;
        }
        //Create 3 InventorySlots 
        slots = new InventorySlot[3];
        //For the 3 Itemslots "Hotkey1", "Hotkey2", "Hotkey3"
        for (int i = 0; i < slots.Length; i++)
        {
            //Regex to find the VisualElement with the name "Hotkey" + i
            VisualElement hotkey = root.Q<VisualElement>($"Hotkey{i + 1}");
            if (hotkey == null)
            {
                Debug.LogError("Could not find VisualElement with name Hotkey" + i + 1);
            }
            // Initialize a new InventorySlot and assign it to the slots array
            slots[i] = new InventorySlot();
            slots[i].Initialize(hotkey);
        }
    }

    public bool AddItemToFirstAvailableSlot(ItemSystem.Abstract.Item item)
    {
        if (item == null)
        {
            Debug.LogError("Attempting to add a null item to the inventory.");
            return false;
        }

        foreach (var slot in slots)
        {
            if (slot.storedItem == null)
            {
                Debug.Log($"Adding item {item.itemName} to the inventory.");
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
