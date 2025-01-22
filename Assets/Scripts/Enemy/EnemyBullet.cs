using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    [SerializeField] private int damageAmount = 5;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the HealthSystem and PlayerMovement components from the player
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount, playerMovement); // Apply damage
            }

            Destroy(gameObject); // Destroy the bullet after the collision
        }
    }
}