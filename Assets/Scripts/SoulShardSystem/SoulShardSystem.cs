using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for soul shard items in the game.
/// </summary>
public class SoulShardSystem : MonoBehaviour
{
    private List<SoulShardItem> soulShardItems;

    private void Awake()
    {
        soulShardItems = new List<SoulShardItem>();
    }

    /// <summary>
    /// Method that increases the player's soul shard count by one.
    /// </summary>
    /// <param name="soulShardItem"></param>
    public void inreaseSoulShard(SoulShardItem soulShardItem)
    {
        soulShardItems.Add(soulShardItem);
        Debug.Log("Soul Shard collected: " + soulShardItems.Count);
    }

    /// <summary>
    /// Method that decreases the player's soul shard count by the specified amount.
    /// </summary>
    public void decreaseSoulShard(SoulShardItem soulShardItem, int amount)
    {
        soulShardItems.Remove(soulShardItem);
    }
}

