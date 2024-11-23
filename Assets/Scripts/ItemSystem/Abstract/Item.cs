using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ItemSystem.Abstract
{
    [System.Serializable]
    public abstract class Item 
    {
        public string itemName { get; set; }
        public Sprite itemSprite { get; set; }
        

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

