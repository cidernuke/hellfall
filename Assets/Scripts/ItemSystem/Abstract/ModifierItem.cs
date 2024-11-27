using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all items that modify stats in the game.
/// </summary>

namespace ItemSystem.Abstract
{
    public abstract class ModifierItem : Item
    {
        public float duration { get; set; }
        public int amount { get; set; }
        public String description { get; set; }


        // TODO: has parameter of Type StatSystem ( StatSystem stats )  

    }
}
