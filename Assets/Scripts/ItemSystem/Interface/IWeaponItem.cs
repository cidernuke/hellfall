using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponItem : IItem
{
   int damage { get; set; }
   
}
