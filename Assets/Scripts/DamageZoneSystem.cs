using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    /// <summary>
    /// Called when another collider enters the trigger collider attached to the object where this script is attached.
    /// </summary>
    /// <param name="collider">The Collider2D that enters the trigger.</param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        HealthSystem playerHealthSys = collider.GetComponent<HealthSystem>();
        if(playerHealthSys != null)
        {
            playerHealthSys.TakeDamage(10);
        }
    }
}
