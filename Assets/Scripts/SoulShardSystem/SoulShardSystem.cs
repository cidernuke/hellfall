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
        //soulShardItems.Remove(soulShardItem);

        //More flexible like this, you can just say how many you want to remove
        for (int i = 0; i < amount && soulShardItems.Count > 0; i++)
        {
            soulShardItems.RemoveAt(soulShardItems.Count - 1);
        }
    }

    public int GetSoulShardCount()
    {
        return soulShardItems.Count;
    }

    public void SetSoulShardCount(int count)
    {
        // To modify the list and being able to reduce the SoulShards in case of death
        soulShardItems = new List<SoulShardItem>(count);
    }

}

