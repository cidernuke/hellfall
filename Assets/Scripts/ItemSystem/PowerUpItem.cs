using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpItem : MonoBehaviour, IModifierItem
{
    public string itemName { get; set; }
    public Sprite itemSprite { get; set; }

    public float duration { get; set; }
    public int amount { get; set; }
    public string description { get; set; }

    public void OnTriggerEnter2D(Collider2D other)
    {
        throw new System.NotImplementedException();
    }

     public void modifyStats()
    {
    }



    
}

