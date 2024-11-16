using UnityEngine;
//Platzhalter for now


[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab; // Reference to the GameObject if needed
}
