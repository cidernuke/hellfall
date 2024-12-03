using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    //public InventorySystem playerInventory;
    private HealthSystem playerHealth;

    void Update()
    {
        // Logic for dropping items
        DropOnKeyPress();

        // Logic for using items
        UseOnKeyPress();
    }

    void Start()
    {
        playerHealth = GetComponent<HealthSystem>();
        //playerInventory.playerHealth = playerHealth;
        InventorySystem.Instance.playerHealth = playerHealth;
        
    }

    //For Pickung up Items
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Item"
        if (other.CompareTag("Item"))
        {
            // Attempt to find an Item component directly
            var pickup = other.GetComponent<InventoryItem>();

            // call AddItemToFirstAvailableSlot. If true, the object was added
            //if (pickup != null && playerInventory.AddItemToFirstAvailableSlot(pickup.item)) //übergibt nur ItemData
            if (pickup != null && InventorySystem.Instance.AddItemToFirstAvailableSlot(pickup.item)) //übergibt nur ItemData
            {
                Destroy(other.gameObject); // Remove the item from the world
            }
            else
            {
                Debug.LogWarning("The object tagged 'Item' does not have a valid Item component!");
            }
        }
    }

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

    private void UseOnKeyPress()
    {
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
}
