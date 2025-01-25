using UnityEngine;
using ItemSystem.Abstract;

/// <summary>
/// Class for power up items in the game.
/// </summary>
namespace ItemSystem.Items
{
    public class PowerUpItem : ModifierItem
    {
        public PlayerController playerController;
        public PowerUpItem(string itemName, Sprite itemSprite)
        {
            this.itemName = itemName;
            this.itemSprite = itemSprite;
            // player controller
            playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        }
        public override bool use()
        {
            if(playerController != null)
            {
                StartCoroutine(upgradeStrength());
            }
            playerController.playerStats.IncrementVitality();
            Debug.Log("PowerUpItem was used");
            return true;
        }

        // TODO: Implement the rest of the PowerUpItem class
        // For example, get reference to StatSystem and increase the players stats
        private IEnumerator upgradeStrength()
        {
            
        }
    }
}

