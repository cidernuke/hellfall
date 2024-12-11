using UnityEngine;

public class RangedEnemy2 : MonoBehaviour
{
    public Transform player;
    public GameObject bullet;

    [Header("Shooting Settings")]
    private float shotCooldown;
    public float startShotCooldown;

    [Header("References")]
    private HealthSystem healthSystem;
    private EnemyController enemyController; // Reference to the EnemyController

    private void Start()
    {
        // Automatically find player by tag if not assigned
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform; // Set the player Transform
            }
            else
            {
                Debug.LogError("Player GameObject not found! Ensure it is tagged correctly or assigned in the inspector.");
            }
        }

        // Initialize shooting cooldown
        shotCooldown = startShotCooldown;

        // Get the HealthSystem component
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("No HealthSystem component found on the enemy!");
        }

        // Get the EnemyController component
        enemyController = GetComponent<EnemyController>();
        if (enemyController == null)
        {
            Debug.LogError("No EnemyController component found on the enemy!");
        }
    }

    private void Update()
    {
        // Aim towards the player
        if (player != null)
        {
            Vector2 direction = player.position - transform.position;
            transform.up = direction;
        }

        // Shoot bullets if cooldown is ready
        if (shotCooldown <= 0)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            shotCooldown = startShotCooldown;
        }
        else
        {
            shotCooldown -= Time.deltaTime;
        }

        // Check if the enemy's health is zero or less, and destroy the enemy
        if (healthSystem != null && healthSystem.currentHealth <= 0)
        {
            Destroy(gameObject); // Destroy the enemy's GameObject
        }
    }

    /// <summary>
    /// Public method to take damage, delegating to the HealthSystem.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (healthSystem != null)
        {
            // Pass the reference of EnemyController to handle death behavior
            healthSystem.TakeDamage(damage, null, enemyController);
        }
    }
}
