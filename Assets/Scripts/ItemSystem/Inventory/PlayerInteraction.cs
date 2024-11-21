using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InventorySystem playerInventory; // Reference to the Inventory script

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is tagged as "Item"
        if (other.CompareTag("Item"))
        {
            // Get the ItemPickup component
            InventoryItem pickup = other.GetComponent<InventoryItem>();

            // Ensure the item and inventory are valid
            if (pickup != null && playerInventory.AddItemToFirstAvailableSlot(pickup.item))
            {
                Destroy(other.gameObject); // Remove the item from the world
            }
        }
    }
}
