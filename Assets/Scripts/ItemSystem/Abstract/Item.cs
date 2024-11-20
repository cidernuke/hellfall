using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem.Abstract
{
    public abstract class Item
    {
        public string itemName { get; set; }    
        public Sprite itemSprite { get; set; }
        public bool isDestroyed { get; set; } 

        public void OnTriggerEnter2D(Collider2D collider)
        {
        }

    }

}

