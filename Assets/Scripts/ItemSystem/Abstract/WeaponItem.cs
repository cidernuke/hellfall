using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all weapon items in the game.
/// </summary>

namespace ItemSystem.Abstract
{
   public abstract class WeaponItem : Item
   {
      public int damage { get; set; }


   }
}
