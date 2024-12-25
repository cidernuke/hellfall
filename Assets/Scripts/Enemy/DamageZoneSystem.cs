using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private PlayerMovement playerMovement;
    /// <summary>
    /// Called when another collider stays in the trigger collider attached to the object 
    /// where this script is attached. Waits for 0.5 seconds before dealing damage to the player.
    /// </summary>
    /// <param name="collider">The Collider2D that enters the trigger.</param>
    IEnumerator OnTriggerStay2D(Collider2D collider)
    {
        HealthSystem playerHealthSys = collider.GetComponent<HealthSystem>();
        playerMovement = collider.GetComponent<PlayerMovement>();
        if(playerHealthSys != null)
        {
            yield return new WaitForSeconds(0.5f);
            playerHealthSys.TakeDamage(10,playerMovement);
            Debug.Log("Player took damage from DamageZone");
        }

    }
}
