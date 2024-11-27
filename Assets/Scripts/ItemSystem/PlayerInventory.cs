using System.Collections.Generic;
using ItemSystem.Abstract;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //TODO: when player picks up an item its added to the Inventory
    public List<ItemSystem.Abstract.Item> collectedItems = new List<ItemSystem.Abstract.Item>();

    public void AddItem(ItemSystem.Abstract.Item item)
    {
        collectedItems.Add(item);
    }

    public void RemoveItem(ItemSystem.Abstract.Item item)
    {
        collectedItems.Remove(item);
    }
}
