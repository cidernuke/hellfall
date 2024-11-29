using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ItemSystem.Abstract;
using ItemSystem.Items; // Add this line to include the namespace where PowerUpItem is defined

public class InventorySystem : MonoBehaviour
{
    public InventorySlot[] slots;
    [SerializeField] protected UIDocument uiDocument;
    protected VisualElement root;

    // needed for the HealthItem
    public HealthSystem playerHealth;


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

    public bool AddItemToFirstAvailableSlot(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("Attempting to add a null item to the inventory.");
            return false;
        }

        foreach (var slot in slots)
        {
            if (slot.storedItem == null)
            {
                Debug.Log($"Adding item {itemData.itemName} to the inventory.");
                //rufe methode auf, um Instanz zu erstellen
                Item newItem = CreateItemInstance(itemData);
                //Falls Itemtyp nicht bekannt, ist newItem null
                if (newItem == null)
                {
                    Debug.LogError("Failed to create item instance.");
                    return false;
                }
                //übergebe die Iteminstanz und die Itemdata an den Slot
                slot.StoreItem(newItem, itemData);
                return true;
            }
        }
        Debug.Log("Inventory full!");
        return false;
    }

    public void DropItemFromSlot(int slotIndex, Vector3 playerPosition, Vector3 dropOffset)
    {
        //droppt Item aus dem jeweiligen Slot
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            slots[slotIndex].DropItem(playerPosition, dropOffset);
        }
    }

    private Item CreateItemInstance(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.ModifierItem:
                return new PowerUpItem(itemData.itemName, itemData.itemSprite);
        	
            //in this case, we need to pass the playerHealth to the HealthItem
            //for the use method
            case ItemType.HealthItem:
                var it = new HealthItem(itemData.itemName, itemData.itemSprite, itemData.healthAmount);
                it.playerHealth = playerHealth;    
                return it;
            // Weitere Fälle für andere Item-Typen können hier hinzugefügt werden
            default:
                Debug.LogError("Unknown ItemType: " + itemData.itemType);
                return null;
        }
    }

    public void UseItemFromSlot(int SlotNumber)
    {
        //Rufe useItem Methode des Items im zugehörigen Slot auf
        switch(SlotNumber)
        {
            case 0:
                slots[0].useItem();
                break;
            case 1:
                slots[1].useItem();
                break;
            case 2:
                slots[2].useItem();
                break;
            default:
                Debug.LogError("Unknown SlotNumber: " + SlotNumber);
                break;
        }
    }
}
