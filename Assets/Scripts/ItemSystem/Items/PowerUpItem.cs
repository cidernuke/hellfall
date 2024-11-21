using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem.Abstract;

namespace ItemSystem.Items
{
    public class PowerUpItem : ModifierItem
    {        
        public override bool use()
        {
            Debug.Log("PowerUpItem used");
            return true;
        }

        // TODO: Implement the rest of the PowerUpItem class
        // For example, get reference to StatSystem and increase the players stats
    }
}

