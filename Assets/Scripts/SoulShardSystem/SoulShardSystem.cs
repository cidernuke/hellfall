using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for soul shard items in the game.
/// </summary>
public class SoulShardSystem : MonoBehaviour
{
    private List<SoulShardItem> soulShardItems;
    [SerializeField] Text counter;
    [SerializeField] int soulShardCount;

    private void Awake()
    {
        soulShardItems = new List<SoulShardItem>();
        SetSoulShardCount(soulShardCount);
    }

    //Now using UpdateUI() instead of Update
    // void Update()
    // {
    //     if (soulShardItems.Count > 0)
    //     {
    //         counter.text = soulShardItems.Count.ToString();
    //     }
    // }

    /// <summary>
    /// Method that increases the player's soul shard count by one.
    /// </summary>
    /// <param name="soulShardItem"></param>
    public void inreaseSoulShard(SoulShardItem soulShardItem, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            soulShardItems.Add(soulShardItem);
            Debug.Log("Soul Shard collected: " + soulShardItems.Count);
            UpdateUI();
            
        }
        
    }

    /// <summary>
    /// Method that decreases the player's soul shard count by the specified amount.
    /// Returns false if the player does not have enough soul shards.
    /// </summary>
    public void decreaseSoulShard(SoulShardItem soulShardItem, int amount)
    {
        //soulShardItems.Remove(soulShardItem);

        //More flexible like this, you can just say how many you want to remove
        if(amount > soulShardItems.Count)
        {
            Debug.Log("You dont have enough Soul Shards");
            
        }

        for (int i = 0; i < amount && soulShardItems.Count > 0; i++)
        {
            soulShardItems.RemoveAt(soulShardItems.Count - 1);
        }
        UpdateUI();
                  
    }

    public int GetSoulShardCount()
    {
        return soulShardItems.Count;
    }

    public void SetSoulShardCount(int count)
    {
        // To modify the list and being able to reduce the SoulShards in case of death
        //soulShardItems = new List<SoulShardItem>(count);

        //Create a list with the amount of soulshards
        soulShardItems = new List<SoulShardItem>(count);

        // Fill the lst with null, cause we just need the amount
        for (int i = 0; i < count; i++)
        {
            soulShardItems.Add(null);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (counter != null)
        {
            counter.text = soulShardItems.Count.ToString();
        }
    }
}

