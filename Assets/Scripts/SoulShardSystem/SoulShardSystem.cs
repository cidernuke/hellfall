using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemSystem.Items;

public class SoulShardSystem : MonoBehaviour
{
    private List<SoulShardItem> soulShardItems;

    private void Awake()
    {
        soulShardItems = new List<SoulShardItem>();
    }
    public void inreaseSoulShard(SoulShardItem soulShardItem)
    {
        soulShardItems.Add(soulShardItem);
        Debug.Log("Soul Shard collected: " + soulShardItems.Count);
    }

    public void decreaseSoulShard(SoulShardItem soulShardItem, int amount)
    {
        soulShardItems.Remove(soulShardItem);
    }
}

