using UnityEngine;

public class RangedEnemy2 : MonoBehaviour
{
    // References to the player and bullet prefab
    public Transform player; // Reference to the player's Transform
    public GameObject bullet; // Bullet prefab that the enemy will shoot

    // Movement settings for the enemy
    [Header("Movement Settings")]
    public float followRange = 15f; // Range within which the enemy will follow the player
    public float shootingRange = 10f; // Range within which the enemy will start shooting
    public float moveSpeed = 2f; // Speed at which the enemy moves towards the player

    // Shooting settings for the enemy
    [Header("Shooting Settings")]
    private float shotCooldown; // Tracks the time before the next shot
    public float startShotCooldown = 2f; // Initial cooldown time between shots

    // References to necessary components
    [Header("References")]
    private Animator animator; // Animator for handling animations
    private HealthSystem healthSystem; // Health system to manage enemy health

    // Start is called before the first frame update
    private void Start()
    {
        // If the player is not manually assigned, find the player using the "Player" tag
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player GameObject not found! Check the tag.");
            }
        }

        // Initialize cooldown and component references
        shotCooldown = startShotCooldown;
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }

        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem component is missing!");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // If the enemy's health is less than or equal to 0, handle death
        if (healthSystem != null && healthSystem.currentHealth <= 0)
        {
            HandleDeath();
            return;
        }

        // If the player exists, handle movement and attack behavior
        if (player != null)
        {
            HandleMovementAndAttack();
        }
    }

    // Handle the movement and attack logic of the enemy
    private void HandleMovementAndAttack()
    {
        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Flip the enemy to face the player
        FlipTowardsPlayer();

        // If the player is within follow range but outside shooting range, move towards the player
        if (distanceToPlayer > shootingRange && distanceToPlayer <= followRange)
        {
            MoveTowardsPlayer();
            animator.SetBool("isShooting", false); // Disable shooting animation while moving
        }
        // If the player is within shooting range, stop moving and shoot
        else if (distanceToPlayer <= shootingRange)
        {
            StopMoving();
            HandleShooting();
        }
        // If the player is outside follow range, stop moving and disable shooting animation
        else
        {
            StopMoving();
            animator.SetBool("isShooting", false); // Disable shooting animation
        }
    }

    // Move the enemy towards the player
    private void MoveTowardsPlayer()
    {
        // Calculate direction towards the player and normalize it
        Vector2 direction = (player.position - transform.position).normalized;
        // Move the enemy towards the player
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    // Stop the enemy from moving
    private void StopMoving()
    {
        animator.SetBool("isShooting", false); // Disable shooting animation while not shooting
    }

    // Flip the enemy to face the player (based on the player's position)
    private void FlipTowardsPlayer()
    {
        Vector3 scale = transform.localScale;
        if (player.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x); // Face right if the player is to the right
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x); // Face left if the player is to the left
        }
        transform.localScale = scale; // Apply the flip by adjusting only the x-scale
    }

    // Handle the shooting behavior of the enemy
    private void HandleShooting()
    {
        // If the shot cooldown is over, shoot a bullet
        if (shotCooldown <= 0)
        {
            animator.SetBool("isShooting", true); // Trigger shooting animation

            // Calculate direction to shoot in
            Vector2 direction = (player.position - transform.position).normalized;

            // Instantiate the bullet and point it in the calculated direction
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            newBullet.transform.up = direction;

            // Reset the shot cooldown
            shotCooldown = startShotCooldown;
        }
        else
        {
            shotCooldown -= Time.deltaTime; // Decrease cooldown over time
        }
    }

    // Handle the death of the enemy
    private void HandleDeath()
    {
        animator.SetTrigger("die"); // Trigger death animation
        Destroy(gameObject, 1f); // Destroy the enemy object after 1 second (to allow death animation to play)
    }

    // Method to take damage from external sources (e.g., player attacks)
    public void TakeDamage(float damage)
    {
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage); // Apply damage to the health system
        }
    }
}
