using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthAmount;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            HealthSystem playerHealth = collider.GetComponent<HealthSystem>();
            playerHealth.AddHealth(healthAmount);            
            Destroy(gameObject);
        }
    }
}
