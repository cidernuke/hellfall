using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public InventorySystem playerInventory;
    private HealthSystem playerHealth;

    private bool isSwitching = false; // To prevent re-entry in switching mode

    // Store the current item the player can pick up
    private InventoryItem itemToPickUp;

    void Update()
    {
        // Logic for dropping items
        DropOnKeyPress();

        // Logic for using items
        UseOnKeyPress();

        // Logic for switching items
        SwitchItems();

        // Logic for picking up items
        PickupOnKeyPress();
    }

    void Start()
    {
        playerHealth = GetComponent<HealthSystem>();
        playerInventory = InventorySystem.Instance;
        //playerInventory.playerHealth = playerHealth;
        InventorySystem.Instance.playerHealth = playerHealth;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Item"
        if (other.CompareTag("Item"))
        {
            // Attempt to find an InventoryItem component on the object
            itemToPickUp = other.GetComponent<InventoryItem>();
            if (itemToPickUp != null)
            {
                Debug.Log($"Item in range to pick up: {itemToPickUp.item.itemName}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Clear the reference when the player leaves the item's trigger zone
        if (other.CompareTag("Item") && itemToPickUp != null && other.GetComponent<InventoryItem>() == itemToPickUp)
        {
            Debug.Log($"Item out of range: {itemToPickUp.item.itemName}");
            itemToPickUp = null;
        }
    }

    // private void PickupOnKeyPress()
    // {
    //     // Check if the player presses E and an item is available to pick up
    //     if (Input.GetKeyDown(KeyCode.E) && itemToPickUp != null)
    //     {
    //         Debug.Log($"Picking up item: {itemToPickUp.item.itemName}");
    //         if (playerInventory.AddItemToFirstAvailableSlot(itemToPickUp.item)) // Attempt to add the item to the inventory
    //         {
    //             Destroy(itemToPickUp.gameObject); // Remove the item from the scene
    //             itemToPickUp = null; // Clear the reference
    //         }
    //         else
    //         {
    //             Debug.Log("Failed to pick up item. Inventory is full!");
    //         }
    //     }
    // }

    private void PickupOnKeyPress()
    {
        // Check if the player presses E and an item is available to pick up
        if (Input.GetKeyDown(KeyCode.E) && itemToPickUp != null)
        {
            print($"Picking up item: {itemToPickUp.item.itemName}");
            if (playerInventory.AddItemToFirstAvailableSlot(itemToPickUp.item)) // Attempt to add the item to the inventory
            {
                ItemRespawner respawner = itemToPickUp.GetComponent<ItemRespawner>();
                if (respawner != null)
                {
                    respawner.CollectItem();
                }
                else
                {
                    // Falls kein respawner vorhanden ist, zerstören wie bisher
                    Destroy(itemToPickUp.gameObject);
                }

                itemToPickUp = null; // Clear the reference
            }
            else
            {
                print("Failed to pick up item. Inventory is full!");
            }
        }
    }

    /// <summary>
    /// Handles dropping items from the player's inventory.
    /// </summary>
    /// <remarks>
    /// This method checks for a key press to drop items from the player's inventory. It also calculates the player's position
    /// and an offset to drop the item from. It gives this and the corresponding slot number to the playerInventory to drop the item.
    /// </remarks>
    private void DropOnKeyPress()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            Vector3 playerPosition = transform.position;  //Player Position
            Vector3 dropOffset = new Vector3(1f, -0.5f, 0f); // Example offset to the right of the player

            //when Player presses 1,2,3
            //drop item from slot 1,2,3
            //pass player position and drop offset
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                //playerInventory.DropItemFromSlot(0, playerPosition, dropOffset); // Drop from slot 1
                InventorySystem.Instance.DropItemFromSlot(0, playerPosition, dropOffset); // Drop from slot 1
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //playerInventory.DropItemFromSlot(1, playerPosition, dropOffset); // Drop from slot 2
                InventorySystem.Instance.DropItemFromSlot(1, playerPosition, dropOffset); // Drop from slot 2
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //playerInventory.DropItemFromSlot(2, playerPosition, dropOffset); // Drop from slot 3
                InventorySystem.Instance.DropItemFromSlot(2, playerPosition, dropOffset); // Drop from slot 3
            }
        }
    }

    /// <summary>
    /// Handles using items from the player's inventory.
    /// </summary>
    /// <remarks>
    /// This method checks for a number key press to use items from the player's inventory. If the right mouse button is held, it
    /// exits to prevent using items while switching items. It then checks for a valid key press and calls the playerInventory to
    /// use the item from the corresponding slot.
    /// </remarks>
    private void UseOnKeyPress()
    {

        // Prevent using items when tab is held (switching mode)
        if (Input.GetKey(KeyCode.Tab)) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //playerInventory.UseItemFromSlot(0);
            InventorySystem.Instance.UseItemFromSlot(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //playerInventory.UseItemFromSlot(1);
            InventorySystem.Instance.UseItemFromSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //playerInventory.UseItemFromSlot(2);
            InventorySystem.Instance.UseItemFromSlot(2);
        }
    }

    /// <summary>
    /// Handles the switching of items.
    /// </summary>
    /// <remarks>
    /// This method checks for a right mouse button press and a subsequent number key press to switch items between slots.
    /// It waits for a short duration to debounce the initial input, then continuously checks for a valid key press to switch
    /// items between slots. If the same slot is selected, it logs a message and exits. After a successful switch, it waits for
    /// a short duration before allowing another switch.
    /// </remarks>
    private void SwitchItems()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            //the ? makes the integer nullable
            int? firstSlot = null;

            //just checking ig the player presses 1,2,3
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                firstSlot = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                firstSlot = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                firstSlot = 2;
            }

            if (firstSlot.HasValue && !isSwitching) // If a valid key was pressed and we are not already switching
            {
                isSwitching = true; // Block re-entry
                // Wait for the next key press
                StartCoroutine(WaitForSecondKeyPress(firstSlot.Value));
            }
        }
    }

    /// <summary>
    /// Waits for a second key press to switch items in the player's inventory.
    /// </summary>
    /// <param name="firstSlot">The slot number of the first item selected for switching.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    /// <remarks>
    /// This method waits for a short duration to debounce the initial input, then continuously checks for a valid key press
    /// to switch items between slots. If the same slot is selected, it logs a message and exits. After a successful switch,
    /// it waits for a short duration before allowing another switch.
    /// </remarks>
    private IEnumerator WaitForSecondKeyPress(int firstSlot)
    {
        Debug.Log($"Waiting for second key press to switch item from slot {firstSlot}");
        // Wait for a small duration to debounce the initial input
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            // Check if the right mouse button is still being held
            if (!Input.GetKey(KeyCode.Tab))
            {
                Debug.Log("Right mouse button released. Canceling item switch.");
                break;
            }
            // Check if the player pressed a valid key, then calling the the switchItems method
            // with the corresponding slot numbers
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (firstSlot == 0)
                {
                    Debug.Log("Cannot switch item with itself");
                    break;
                }
                playerInventory.SwitchItems(firstSlot, 0);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (firstSlot == 1)
                {
                    Debug.Log("Cannot switch item with itself");
                    break;
                }
                playerInventory.SwitchItems(firstSlot, 1);
                break;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (firstSlot == 2)
                {
                    Debug.Log("Cannot switch item with itself");
                    break;
                }
                playerInventory.SwitchItems(firstSlot, 2);
                break;
            }
            yield return null; // Wait for the next frame before rechecking
        }

        // Delay before allowing switching again
        yield return new WaitForSeconds(0.1f);
        isSwitching = false; // Allow switching again
    }



}
