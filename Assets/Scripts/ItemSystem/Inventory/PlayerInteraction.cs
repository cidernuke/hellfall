using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public InventorySystem playerInventory;

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
