using UnityEngine;

/// <summary>
/// Base class for all items in the game.
/// </summary>
namespace ItemSystem.Abstract
{
    [System.Serializable]
    public abstract class Item
    {
        public string itemName { get; set; }
        public Sprite itemSprite { get; set; }
        public GameObject prefab;
        
        public virtual bool use()
        {
            return false;
        }
        public virtual bool use(HealthSystem playerHealth)
        {
            return false;
        }
    }
}
