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
            var itemComponent = other.GetComponent<ItemSystem.Abstract.Item>();

            if (itemComponent != null && playerInventory.AddItemToFirstAvailableSlot(itemComponent))
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
