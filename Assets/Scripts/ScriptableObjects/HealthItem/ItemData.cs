using UnityEngine;

/// <summary>
/// Holds the data for an item in the game.
/// </summary>     
[CreateAssetMenu(fileName = "New Item Data", menuName = "Inventory System/Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public ItemType itemType;

    [Header("Health Item")]
    public float healthAmount;

    [Header("Modifier Item")]
    public float duration;
    public int amount;
    public string description;

    [Header("Weapon Item")]
    public bool isFire;
    public bool isIce;
    
}

public enum ItemType
{
    ModifierItem,
    HealthItem,
    WeaponItem,
    // More Item-types can be added here
    ShortRangeWeapon,

    LongRangeWeapon
}
