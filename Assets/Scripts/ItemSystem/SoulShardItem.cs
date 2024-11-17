using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShardItem : MonoBehaviour, IItem
{
    public string itemName { get; set; }
    public Sprite itemSprite { get; set; }   
    

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player"){
            SoulShardSystem soulShardSystem = collider.GetComponent<SoulShardSystem>();
            if (soulShardSystem != null)
            {
                soulShardSystem.inreaseSoulShard(this);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("SoulShardSystem component not found on the player.");
            }
        }
    }
}

