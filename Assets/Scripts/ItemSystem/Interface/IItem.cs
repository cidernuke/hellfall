using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem 
{
    String itemName { get; set; }
    Sprite itemSprite { get; set; }

    public void OnTriggerEnter2D(Collider2D other)
    {
    }
        
}
