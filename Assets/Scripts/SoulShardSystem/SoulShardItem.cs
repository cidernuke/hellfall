using UnityEngine;

/// <summary>
/// Class for soul shard items in the game.
/// </summary>
public class SoulShardItem : MonoBehaviour
{
    /// <summary>
    /// Method that is called when the player collides with the soul shard.
    /// The player's soul shard count is increased by one and the soul shard is destroyed.
    /// </summary>
    /// <param name="collider"></param>
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
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


