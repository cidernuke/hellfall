using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using ItemSystem.Abstract;
using ItemSystem.Items;
using UnityEngine.SceneManagement; // Add this line to include the namespace where PowerUpItem is defined

public class InventorySystem : MonoBehaviour
{
    // Singleton instance
    public static InventorySystem Instance { get; private set; }
    public InventorySlot[] slots;
    //[SerializeField] protected UIDocument uiDocument;
    protected VisualElement root;

    // Dynamic fields
    private UIDocument uiDocument;
    private GameObject player;
    private PlayerMovement playerMov;
    public HealthSystem playerHealth;  // public so that GameManager can override it
    private GameObject playerSpeech;
    private SpriteRenderer playerSpeechSpriteRenderer;

    // Flag
    private bool isLoaded = false;

    // New Singleton implementation
    private void Awake()
    {
        // Temporary solution: Destroy duplicate check
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Duplicate InventorySystem found on {gameObject.name}. Destroying it.");
            Destroy(gameObject);
            return; // Important: Abort here so that the rest of Awake is not executed.
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Call "OnSceneLoaded" on every scene change
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        Debug.Log("Start");
        // Search for references in the current scene
        SetupReferences();
    }

    /// <summary>
    /// Called every time a new scene is loaded.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        isLoaded = true;
        // Search for references again
        //SetupReferences();
    }

    /// <summary>
    /// Dynamically searches for the player, HealthSystem, UI document, etc. in the current scene.
    /// </summary>
    public void SetupReferences()
    {
        // 1) Search for UI
        uiDocument = FindObjectOfType<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogWarning("No UIDocument found in this scene. InventoryUI might not be available.");
            return;
        }

        // Get root
        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("UIDocument rootVisualElement is null!");
            return;
        }

        // 2) Create/initialize slots
        // Warning: If you only want to do this ONCE, you should first check
        // if slots are already created. Or you delete them and create them again.

        // slots = new InventorySlot[3];
        // for (int i = 0; i < slots.Length; i++)
        // {
        //     VisualElement hotkey = root.Q<VisualElement>($"Hotkey{i + 1}Container");
        //     if (hotkey == null)
        //     {
        //         Debug.LogError("Could not find VisualElement with name Hotkey" + (i + 1));
        //     }
        //     slots[i] = new InventorySlot();
        //     slots[i].Initialize(hotkey);
        // }

        // Do not reassign slots, only the UI elements
        if (slots == null || slots.Length == 0)
        {
            slots = new InventorySlot[5];
            // For the 5 item slots "Hotkey1", "Hotkey2", "Hotkey3", Weapon1, Weapon2
            for (int i = 0; i < 5; i++)
            {
                slots[i] = new InventorySlot();
            }
        }

        // Now only search and bind the UI elements of the hotkeys
        for (int i = 0; i < 3; i++)
        {
            var hotkey = root.Q<VisualElement>($"Hotkey{i + 1}Container");
            slots[i].Initialize(hotkey);
        }

        // 3) Search for player
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj;
            playerMov = player.GetComponent<PlayerMovement>();
            playerHealth = player.GetComponent<HealthSystem>();
        }
        else
        {
            Debug.LogWarning("No Player found in this scene!");
        }

        // 4) Speech Bubble
        playerSpeech = GameObject.Find("Player_Speech_Bubble");
        if (playerSpeech != null)
        {
            playerSpeechSpriteRenderer = playerSpeech.GetComponent<SpriteRenderer>();
        }

        // 5) Initialize weapon slots and add default weapons

        // For slots 4 and 5 which are weapon slots
        // Get the containers!
        VisualElement shortRangeWeaponHotkey = root.Q<VisualElement>("Weapon1Container");
        if (shortRangeWeaponHotkey == null)
        {
            Debug.LogError("Could not find VisualElement with name Weapon1Container");
        }
        VisualElement longRangeWeaponHotkey = root.Q<VisualElement>("Weapon2Container");
        if (longRangeWeaponHotkey == null)
        {
            Debug.LogError("Could not find VisualElement with name Weapon2Container");
        }

        // Initialize
        slots[3] = new InventorySlot();
        slots[3].Initialize(shortRangeWeaponHotkey);

        slots[4] = new InventorySlot();
        slots[4].Initialize(longRangeWeaponHotkey);

        // Let player start with short (and to-do: long) range weapon 
        try
        {
            ItemData basicSword = Resources.Load<ItemData>("ItemData/BasicSword");
            //Debug.Log("Trying to create item instance: " + basicData.itemName + " " + basicData.itemType);
            AddItemToFirstAvailableSlot(basicSword);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not load ItemData/BasicSword");
        }

        try
        {
            ItemData basicLongRange = Resources.Load<ItemData>("ItemData/BasicLongRange");
            //Debug.Log("Trying to create item instance: " + basicData.itemName + " " + basicData.itemType);
            AddItemToFirstAvailableSlot(basicLongRange);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not load ItemData/BasicLongRange");
        }
    }

    void Update()
    {
        if (playerMov == null || playerSpeechSpriteRenderer == null)
            return;

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
        // Check from Diego
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

        // Special case for weapons
        if (itemData.itemType == ItemType.ShortRangeWeapon)
        {
            // Check if the slot is empty, else return false so weapons don't get added to other slots
            if (slots[3].storedItem == null)
            {
                //Debug.Log("Weapon added to inventory");
                Item newItem = CreateItemInstance(itemData);
                slots[3].StoreItem(newItem, itemData);
                return true;
            }
            else
            {
                return false;
            }
        }

        if (itemData.itemType == ItemType.LongRangeWeapon)
        {
            // Check if the slot is empty, else return false so weapons don't get added to other slots
            if (slots[4].storedItem == null)
            {
                Item newItem = CreateItemInstance(itemData);
                slots[4].StoreItem(newItem, itemData);
                return true;
            }
            else
            {
                return false;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (slots[i].storedItem == null)
            {
                // Calling method to set up instance
                Item newItem = CreateItemInstance(itemData);
                // In case itemType isn't known, newItem is null
                if (newItem == null)
                {
                    //Debug.LogError("Failed to create item instance.");
                    return false;
                }
                // Pass the item instance and itemData to the slot
                slots[i].StoreItem(newItem, itemData);
                return true;
            }
        }
        StartCoroutine(InvenotryFull(1));
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
        // Drops item from the respective slot
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
    /// - For <see cref="ItemType.WeaponItem"/>, it creates a <see cref="WeaponItem"/>.
    /// Additional item types can be added by extending the switch statement.
    /// </remarks>
    //private Item CreateItemInstance(ItemData itemData)
    //Diego changed to public, because i need to use it
    public Item CreateItemInstance(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("ItemData is null.");
            return null;
        }
        switch (itemData.itemType)
        {
            case ItemType.ModifierItem:
                return new PowerUpItem(itemData.itemName, itemData.itemSprite);

            // In this case, we need to pass the playerHealth to the HealthItem
            // for the use method
            case ItemType.HealthItem:
                var it = new HealthItem(itemData.itemName, itemData.itemSprite, itemData.healthAmount);
                it.playerHealth = playerHealth;
                return it;
            // Additional cases for other item types can be added here

            // Short Range Weapon
            case ItemType.ShortRangeWeapon:
                return new ShortRangeWeapon(itemData.itemName, itemData.itemSprite, itemData.isFire, itemData.isIce);

            // Long Range Weapon
            case ItemType.LongRangeWeapon:
                return new LongRangeWeapon(itemData.itemName, itemData.itemSprite, itemData.isFire, itemData.isIce);
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
        // Call useItem from Items on the respective slot
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
        //Debug.Log($"First slot: {firstSlot}, second slot: {secondSlot}");
        //Debug.Log($"BEFORE SWAP: Item in first slot: {slots[firstSlot].itemData?.itemName}, Item in second slot: {slots[secondSlot].itemData?.itemName}");
        // Retrieve item data from the first slot
        var firstSlotItemData = slots[firstSlot].itemData;
        // Allow switching even if the first slot is empty
        Item firstSlotNewItem = firstSlotItemData != null ? CreateItemInstance(firstSlotItemData) : null;

        // Retrieve item data from the second slot
        var secondSlotItemData = slots[secondSlot].itemData;
        // Allow switching even if the second slot is empty
        Item secondSlotNewItem = secondSlotItemData != null ? CreateItemInstance(secondSlotItemData) : null;

        // Clear InventorySlots
        slots[firstSlot].ClearSlot();
        slots[secondSlot].ClearSlot();

        // Store the new item instances in the opposite slots
        slots[secondSlot].StoreItem(firstSlotNewItem, firstSlotItemData);
        slots[firstSlot].StoreItem(secondSlotNewItem, secondSlotItemData);

        //Debug.Log($"AFTER SWAP: Item in first slot: {slots[firstSlot].itemData?.itemName}, Item in second slot: {slots[secondSlot].itemData?.itemName}");
    }

    /// <summary>
    /// Coroutine to display an "Inventory Full" message for a set duration or until "Q" is pressed.
    /// </summary>
    public IEnumerator InvenotryFull(int messageCase)
    {
        var time = 1.5f;
        Sprite speechBubble = null; 
        switch(messageCase)
        {
            case 1:
                speechBubble = Resources.Load<Sprite>("Sprites/Level_One/Speech_Bubbles/Inventory_full_bubble");
            break;
            case 2:
                //to-do:
            break;
            default:
                //nix
            break;
        }
        // Load the sprite for the speech bubble
        //Sprite speechBubble = Resources.Load<Sprite>("Sprites/Level_One/Speech_Bubbles/Inventory_full_bubble");
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

    /// <summary>
    /// Clears the inventory by removing all items from the slots.
    /// </summary>
    public void ClearInventory()
    {
        for (int i = 0; i < 3; i++)
        {
            slots[i].ClearSlot();
        }
    }

    /// <summary>
    /// Hides the inventory UI.
    /// </summary>
    public void HideInventoryUI()
    {
        if (root != null)
            root.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Shows the inventory UI.
    /// </summary>
    public void ShowInventoryUI()
    {
        if (root != null)
            root.style.display = DisplayStyle.Flex;
    }

}
