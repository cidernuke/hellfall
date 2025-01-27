using ItemSystem.Abstract;
using UnityEngine;

/// <summary>
/// Class for health items in the game.
/// </summary>
public class HealthItem : Item
{
    private float healthAmount; // how much does an item heal
    public HealthSystem playerHealth; // reference to
    private AudioManager audioManager; 
    
    /// <summary>
    /// Constructor for the HealthItem class.
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="itemSprite"></param>
    /// <param name="healthAmount"></param>
    public HealthItem(string itemName, Sprite itemSprite, float healthAmount) 
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.healthAmount = healthAmount;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    /// <summary>
    /// Uses the health item.
    /// </summary>
    /// <returns></returns>
    public override bool use()        {
            
            if (playerHealth.currentHealth < playerHealth.startingHealth)
            {
                playerHealth.AddHealth(healthAmount);
                audioManager.PlaySFX(audioManager.healthitem);                            
                return true;             
            }            
            return false;
        }
}
