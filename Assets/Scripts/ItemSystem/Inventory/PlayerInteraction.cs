using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InventorySystem playerInventory;

    //For Dropping Items
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerInventory.DropItemFromSlot(0); // Drop from slot 1
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerInventory.DropItemFromSlot(1); // Drop from slot 2
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                playerInventory.DropItemFromSlot(2); // Drop from slot 3
            }
        }
    }



    //For Pickung up Items
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Item"
        if (other.CompareTag("Item"))
        {
            // Attempt to find an Item component directly
            var pickup = other.GetComponent<InventoryItem>();

            if (pickup != null && playerInventory.AddItemToFirstAvailableSlot(pickup.item))
            {
                Destroy(other.gameObject); // Remove the item from the world
            }
            else
            {
                Debug.LogWarning("The object tagged 'Item' does not have a valid Item component!");
            }
        }
    }
}
