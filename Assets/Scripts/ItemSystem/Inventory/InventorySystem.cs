using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ItemSystem.Abstract;
using ItemSystem.Items; // Add this line to include the namespace where PowerUpItem is defined

public class InventorySystem : MonoBehaviour
{
    // Singleton-Instance
    public static InventorySystem Instance { get; private set; }
    public InventorySlot[] slots;
    [SerializeField] protected UIDocument uiDocument;
    protected VisualElement root;

    // needed for the HealthItem
    public HealthSystem playerHealth;
    private GameObject playerSpeech;

    private SpriteRenderer playerSpeechSpriteRenderer;
    
    private GameObject player;

    private PlayerMovement playerMov;

    void Update()
    {
        // Flip the speech bubble sprite if the player is facing left
        if (playerMov.isFacingRight == false)
        {
            playerSpeechSpriteRenderer.flipX = true;
        }
        else
        {
            playerSpeechSpriteRenderer.flipX = false;
        }
    }


    //Singleton-implementation, to make global accessable
    private void Awake()
    {
        // Implement the Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Inventory persist across scenes
        }
        else
        {
            Destroy(gameObject);
            Debug.LogError("Multiple InventorySystem instances detected.");
        }
    }
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
            VisualElement hotkey = root.Q<VisualElement>($"Hotkey{i + 1}Container");
            if (hotkey == null)
            {
                Debug.LogError("Could not find VisualElement with name Hotkey" + i + 1);
            }
            // Initialize a new InventorySlot and assign it to the slots array
            slots[i] = new InventorySlot();
            slots[i].Initialize(hotkey);
        }

        playerSpeech = GameObject.Find("Player_Speech_Bubble");
        playerSpeechSpriteRenderer = playerSpeech.GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
        playerMov = player.GetComponent<PlayerMovement>();

    }

    /// <summary>
    /// Adds an item to the first available slot in the inventory.
    /// </summary>
    /// <param name="itemData">The data of the item to be added.</param>
    /// <returns>
    /// True if the item was successfully added to an available slot; 
    /// false if the item could not be added (e.g., inventory is full or itemData is null).
    /// </returns>
    /// <remarks>
    /// This method logs an error if the itemData is null or if the item instance could not be created.
    /// It also logs a message when an item is successfully added or if the inventory is full.
    /// </remarks>
    public bool AddItemToFirstAvailableSlot(ItemData itemData)
    {
        //Check from Diego
        if (slots == null)
        {
            //Debug.LogError("Slots array is null.");
            return false;
        }
        if (slots.Length == 0)
        {
            //Debug.LogError("Slots array is empty.");
            return false;
        }

        if (itemData == null)
        {
            //Debug.LogError("Attempting to add a null item to the inventory.");
            return false;
        }

        foreach (var slot in slots)
        {
            if (slot.storedItem == null)
            {
                //Debug.Log($"Adding item {itemData.itemName} to the inventory.");
                // calling method to set up instance
                Item newItem = CreateItemInstance(itemData);
                // incase itemType isnt known, newItem is null
                if (newItem == null)
                {
                    //Debug.LogError("Failed to create item instance.");
                    return false;
                }
                // pass the item-instance and itemData to the slot
                slot.StoreItem(newItem, itemData);
                return true;
            }
        }
        StartCoroutine(InvenotryFull());
        return false;
    }


    /// <summary>
    /// Drops the item from the specified slot at the given player position with an offset.
    /// </summary>
    /// <param name="slotIndex">The index of the slot from which to drop the item.</param>
    /// <param name="playerPosition">The position of the player where the item will be dropped.</param>
    /// <param name="dropOffset">The offset from the player's position where the item will be dropped.</param>
    public void DropItemFromSlot(int slotIndex, Vector3 playerPosition, Vector3 dropOffset)
    {
        // dropps item from the respective slot
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            slots[slotIndex].DropItem(playerPosition, dropOffset);
        }
    }

    /// <summary>
    /// Creates an instance of an item based on the provided item data.
    /// </summary>
    /// <param name="itemData">The data used to create the item instance.</param>
    /// <returns>
    /// An instance of an item corresponding to the item type specified in the item data.
    /// Returns null if the item type is unknown.
    /// </returns>
    /// <remarks>
    /// This method handles different item types by using a switch statement:
    /// - For <see cref="ItemType.ModifierItem"/>, it creates a <see cref="PowerUpItem"/>.
    /// - For <see cref="ItemType.HealthItem"/>, it creates a <see cref="HealthItem"/> and assigns the player's health to it.
    /// Additional item types can be added by extending the switch statement.
    /// </remarks>
    //private Item CreateItemInstance(ItemData itemData)
    //Diego changed to public, because i need to use it
    public Item CreateItemInstance(ItemData itemData)
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
            // Additional cases for other item types can be added here
            default:
                //Debug.LogError("Unknown ItemType: " + itemData.itemType);
                return null;
        }
    }


    /// <summary>
    /// Uses the item from the specified slot number.
    /// </summary>
    /// <param name="SlotNumber">The slot number from which to use the item. Valid values are 0, 1, and 2.</param>
    /// <remarks>
    /// This method calls the <c>useItem</c> method on the item in the specified slot.
    /// If the slot number is not valid, an error message is logged.
    /// </remarks>
    public void UseItemFromSlot(int SlotNumber)
    {
        // call useItem from Items on the respective slot
        switch (SlotNumber)
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
                //Debug.LogError("Unknown SlotNumber: " + SlotNumber);
                break;
        }
    }

    /// <summary>
    /// Switches the items between two specified inventory slots.
    /// </summary>
    /// <param name="firstSlot">The index of the first slot.</param>
    /// <param name="secondSlot">The index of the second slot.</param>
    /// <remarks>
    /// This method allows switching items even if one or both of the slots are empty.
    /// It logs the state of the slots before and after the swap for debugging purposes.
    /// </remarks>
    public void SwitchItems(int firstSlot, int secondSlot)
    {

        Debug.Log($"First slot: {firstSlot}, second slot: {secondSlot}");
        Debug.Log($"BEFORE SWAP: Item in first slot: {slots[firstSlot].itemData?.itemName}, Item in second slot: {slots[secondSlot].itemData?.itemName}");
        // Retrieve item data from the first slot
        var firstSlotItemData = slots[firstSlot].itemData;
        // Allow switching even if the first slot is empty
        Item firstSlotNewItem = firstSlotItemData != null ? CreateItemInstance(firstSlotItemData) : null;


        // Retrieve item data from the second slot
        var secondSlotItemData = slots[secondSlot].itemData;
        // Allow switching even if the second slot is empty
        Item secondSlotNewItem = secondSlotItemData != null ? CreateItemInstance(secondSlotItemData) : null;


        // Store the new item instances in the opposite slots
        slots[secondSlot].StoreItem(firstSlotNewItem, firstSlotItemData);
        slots[firstSlot].StoreItem(secondSlotNewItem, secondSlotItemData);

        //Debug.Log($"AFTER SWAP: Item in first slot: {slots[firstSlot].itemData?.itemName}, Item in second slot: {slots[secondSlot].itemData?.itemName}");

    }

    public IEnumerator InvenotryFull()
    {
        var time = 1.5f;
        // Load the sprite for the speech bubble
        Sprite speechBubble = Resources.Load<Sprite>("Sprites/Level_One/Speech_Bubbles/Inventory_full_bubble");
        // Set the sprite and position of the speech bubble
        playerSpeechSpriteRenderer.sprite = speechBubble;

        // Move the speech bubble a little bit to the right
        playerSpeechSpriteRenderer.transform.position = new Vector2(player.transform.position.x + 1.7f, playerSpeechSpriteRenderer.transform.position.y);

        float elapsedTime = 0f;

        // Wait for either the full time or until "Q" is pressed to skip
        while (elapsedTime < time)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // Hide the speech bubble and exit early if "Q" is pressed
                playerSpeechSpriteRenderer.sprite = null;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        // Hide the speech bubble after the time has elapsed
        playerSpeechSpriteRenderer.sprite = null;
    }

    public void ClearInventory()
    {
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
    }

}
