using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Coroutine damageCoroutine;

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object.
    /// </summary>
    /// <param name="collider">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        HealthSystem playerHealthSys = collider.GetComponent<HealthSystem>();
        playerMovement = collider.GetComponent<PlayerMovement>();
        if (playerHealthSys != null)
        {
            // Start damaging the player
            damageCoroutine = StartCoroutine(DealDamage(playerHealthSys));
        }
    }

    /// <summary>
    /// Called when another collider exits the trigger collider attached to this object.
    /// </summary>
    /// <param name="collider">The collider that exited the trigger.</param>
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (damageCoroutine != null)
        {
            // Stop damaging the player
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine to apply damage to the player over time.
    /// </summary>
    /// <param name="playerHealthSys">The player's health system.</param>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator DealDamage(HealthSystem playerHealthSys)
    {
        while (true)
        {
            // Apply damage to the player
            yield return new WaitForSeconds(1f);
            playerHealthSys.TakeDamage(10, playerMovement);
            Debug.Log("Player took damage from DamageZone");
        }
    }
}
