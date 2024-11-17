using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShardItem : MonoBehaviour, IItem
{
    public string itemName { get; set; }
    public Sprite itemSprite { get; set; }

    public void OnTriggerEnter2D(Collider2D other)
    {
        throw new System.NotImplementedException();
    }
}

