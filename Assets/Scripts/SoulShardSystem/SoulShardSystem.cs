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

    /// <summary>
    /// Method that increases the player's soul shard count by the specified amount.
    /// </summary>
    /// <param name="soulShardItem">The soul shard item to add.</param>
    /// <param name="amount">The amount of soul shards to add.</param>
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
    /// <param name="soulShardItem">The soul shard item to remove.</param>
    /// <param name="amount">The amount of soul shards to remove.</param>
    public void decreaseSoulShard(SoulShardItem soulShardItem, int amount)
    {
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

    /// <summary>
    /// Gets the current count of soul shards.
    /// </summary>
    /// <returns>The current count of soul shards.</returns>
    public int GetSoulShardCount()
    {
        return soulShardItems.Count;
    }

    /// <summary>
    /// Sets the soul shard count to the specified value.
    /// </summary>
    /// <param name="count">The new soul shard count.</param>
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

    /// <summary>
    /// Updates the UI to reflect the current soul shard count.
    /// </summary>
    private void UpdateUI()
    {
        if (counter != null)
        {
            counter.text = soulShardItems.Count.ToString();
        }
    }
}

