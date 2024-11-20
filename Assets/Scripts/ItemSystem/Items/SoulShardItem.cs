using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem.Abstract;


namespace ItemSystem.Items
{
    public class SoulShardItem : Item
    {
        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == "Player")
            {
                SoulShardSystem soulShardSystem = collider.GetComponent<SoulShardSystem>();
                if (soulShardSystem != null)
                {
                    soulShardSystem.inreaseSoulShard(this);                    
                }
                else
                {
                    Debug.LogError("SoulShardSystem component not found on the player.");
                }
            }
        }
    }
}

