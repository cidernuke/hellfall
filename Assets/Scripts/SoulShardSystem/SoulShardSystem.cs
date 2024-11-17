using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShardSystem : MonoBehaviour
{
    private List<SoulShardItem> soulShardItems = new List<SoulShardItem>();

    public void inreaseSoulShard(SoulShardItem soulShardItem, int amount)    
    {
        soulShardItems.Add(soulShardItem);
    }

    public void decreaseSoulShard(SoulShardItem soulShardItem, int amount)
    {
        soulShardItems.Remove(soulShardItem);
    }
}
