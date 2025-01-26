using UnityEngine;
using ItemSystem.Abstract;

/// <summary>
/// Class for power up items in the game.
/// </summary>

public class PowerUpItem : Item
{
    public PlayerController playerController;
    public PowerUpItem(string itemName, Sprite itemSprite)
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        // player controller
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    public override bool use()
    {
        if (playerController == null)
        {
            Debug.Log("PowerUpItem couldn't be used");
            return false;
        }
        //needs to be called in player controller
        playerController.handlePowerUp();
        Debug.Log("PowerUpItem was used");
        return true;
    }

    // TODO: Implement the rest of the PowerUpItem class
    // For example, get reference to StatSystem and increase the players stats
    
}


