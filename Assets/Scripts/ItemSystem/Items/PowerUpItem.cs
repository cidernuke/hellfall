using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem.Abstract;

/// <summary>
/// Class for power up items in the game.
/// </summary>

namespace ItemSystem.Items
{
    public class PowerUpItem : ModifierItem
    {        
        public PowerUpItem(string itemName, Sprite itemSprite)
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        
    }
        public override bool use()
        {
            Debug.Log("PowerUpItem was used");
            return true;
        }

        // TODO: Implement the rest of the PowerUpItem class
        // For example, get reference to StatSystem and increase the players stats
    }
}

