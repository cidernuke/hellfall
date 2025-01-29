using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndeadExecutioner : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Player Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Summoning Settings")]
    [SerializeField] private GameObject miniEnemyPrefab;  // Mini enemy prefab
    [SerializeField] private Transform[] summonPoints;  // Points around the boss to spawn mini enemies
    [SerializeField] private int maxMiniEnemies = 3;  // Max number of mini enemies that can be summoned
    private List<GameObject> spawnedMiniEnemies = new List<GameObject>();  // List to track spawned mini enemies

    [Header("Skill Parameters")]
    [SerializeField] private float healAmount = 50f;

    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private HealthSystem healthSystem;
    private EnemyController enemyController;
    private Transform playerTransform;
    private AudioManager audioManager;
    private bool facingRight = true;
    private bool isDead = false;
    public BossHealthBar healthBar;

    [Header("Boss Behavior")]
    [SerializeField] private float maxFollowDistance = 15f;  // The distance at which the boss will stop following and start summoning
    private bool isFollowingPlayer = true;  // Tracks if the boss is currently following the player
	[HideInInspector] public bool didBossKillPlayer = false;


    private void Start()
    {
        StartCoroutine(HealPeriodically());
        StartCoroutine(CheckForDeadMiniEnemies());
    }

    private void Awake()
    {
        // Assign existing components
        healthSystem = GetComponent<HealthSystem>();
        enemyController = GetComponent<EnemyController>();
        anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        healthBar.SetMaxHealth((int)healthSystem.startingHealth);
    }

    private void Update()
    {
        if (healthSystem.currentHealth < healthSystem.startingHealth)
        {
            healthBar.SetHealth((int)healthSystem.currentHealth);
        }

        // Check if health reaches zero
        if (healthSystem.currentHealth <= 0)
        {
            HandleDeath();
        }

        // Follow the player if in sight and within following range
        if (PlayerInSight() && isFollowingPlayer)
        {
            FollowPlayer();
        }

        // Summon mini-enemies if below 50% health or if the player is too far
        if (healthSystem.currentHealth < healthSystem.startingHealth * 0.5f && spawnedMiniEnemies.Count < maxMiniEnemies)
        {
            SummonMiniEnemies();
        }

        // If the player is too far away, stop following and start summoning mini-enemies
        float distanceToPlayer = Vector3.Distance(gameObject.transform.position, playerTransform.position);
        if (distanceToPlayer > maxFollowDistance && isFollowingPlayer)
        {
            StopFollowingAndSummon();
        }
        else if (distanceToPlayer <= maxFollowDistance && !isFollowingPlayer)
        {
            ResumeFollowingPlayer();
        }
        if (distanceToPlayer <= 2)
        {
            anim.SetTrigger("meleeAttack");
        }
        // If the player is back in range, start following again
        // else if (distanceToPlayer <= maxFollowDistance && !isFollowingPlayer)

        if (didBossKillPlayer)
		{
			DeactivateBoss();
		}
    }

    /// <summary>
    /// Checks if the player is within the detection range.
    /// </summary>
    /// <returns>True if the player is in sight, otherwise false.</returns>
    private bool PlayerInSight()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (hit != null)
        {
            playerTransform = hit.transform;
            return true;
        }

        // playerTransform = null;
        return false;
    }

    /// <summary>
    /// Makes the boss follow the player.
    /// </summary>
    private void FollowPlayer()
    {
        if (playerTransform == null) return;

        // Calculate the distance to the player
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // If too close, keep a distance
        if (distance > 2f)  // Adjust this value for a better fit
        {
            // Move towards the player
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += new Vector3(direction.x, 0, 0) * moveSpeed * Time.deltaTime;

            // Flip the sprite based on player's position
            if (playerTransform.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            else if (playerTransform.position.x < transform.position.x && facingRight)
            {
                Flip();
            }
        }
    }

    /// <summary>
    /// Stops the boss from following the player and starts summoning mini-enemies.
    /// </summary>
    private void StopFollowingAndSummon()
    {
        // Stop the boss's movement
        isFollowingPlayer = false;
        moveSpeed = 0f;

        // Summon mini-enemies if not already done
        if (spawnedMiniEnemies.Count < maxMiniEnemies)
        {
            SummonMiniEnemies();
        }
    }

    /// <summary>
    /// Resumes following the player.
    /// </summary>
    private void ResumeFollowingPlayer()
    {
        // Resume following the player
        isFollowingPlayer = true;
        moveSpeed = 2f; // You can set this to the desired speed

        // Flip to face the player if needed
        if (playerTransform.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (playerTransform.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    /// <summary>
    /// Flips the boss's sprite to face the opposite direction.
    /// </summary>
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    /// <summary>
    /// Summons mini-enemies at predefined points.
    /// </summary>
    private void SummonMiniEnemies()
    {
        anim.SetTrigger("Summon");
        Debug.Log("Summoning mini enemies...");

        foreach (var point in summonPoints)
        {
            // Only spawn mini-enemies if there are less than the max
            if (spawnedMiniEnemies.Count >= maxMiniEnemies) break;

            // Instantiate mini enemy at the summon point
            GameObject miniEnemy = Instantiate(miniEnemyPrefab, point.position, Quaternion.identity);
            // miniEnemy.GetComponent<Explosion>().isMinion = true;
            spawnedMiniEnemies.Add(miniEnemy);

            // Ensure spawned enemies use existing EnemyController and HealthSystem
            var miniEnemyController = miniEnemy.GetComponent<EnemyController>();
            if (miniEnemyController)
            {
                miniEnemyController.enabled = true;
                miniEnemyController.isBoss = true;
            }

            var miniHealthSystem = miniEnemy.GetComponent<HealthSystem>();
            if (miniHealthSystem)
            {
                miniHealthSystem.enabled = true;
            }
        }
    }

    /// <summary>
    /// Respawns a mini-enemy at a random spawn point.
    /// </summary>
    /// <param name="deadMiniEnemy">The mini-enemy that died.</param>
    private void RespawnMiniEnemy(GameObject deadMiniEnemy)
    {
        // Remove the dead mini enemy from the list and destroy it
        spawnedMiniEnemies.Remove(deadMiniEnemy);
        Destroy(deadMiniEnemy);

        // Respawn a new mini enemy at a random spawn point
        foreach (var point in summonPoints)
        {
            if (spawnedMiniEnemies.Count >= maxMiniEnemies) break;

            GameObject miniEnemy = Instantiate(miniEnemyPrefab, point.position, Quaternion.identity);
            // miniEnemy.GetComponent<Explosion>().isMinion = true;
            spawnedMiniEnemies.Add(miniEnemy);
            var miniEnemyController = miniEnemy.GetComponent<EnemyController>();
            if (miniEnemyController)
            {
                miniEnemyController.enabled = true;
                miniEnemyController.isBoss = true;
            }

            var miniHealthSystem = miniEnemy.GetComponent<HealthSystem>();
            if (miniHealthSystem)
            {
                miniHealthSystem.enabled = true;
            }
        }
    }

    /// <summary>
    /// Uses the boss's healing skill.
    /// </summary>
    public void UseSkill1()
    {
        if (isDead) return;

        anim.SetTrigger("Skill");

        // Heal the boss
        healthSystem.AddHealth(healAmount);
        Debug.Log($"Boss healed by {healAmount}, current health: {healthSystem.currentHealth}");
    }

    /// <summary>
    /// Handles the boss's death.
    /// </summary>
    private void HandleDeath()
    {
        //if (isDead) return; // Ensure death logic is only triggered once
        //isDead = true;

        // Play death animation
        //anim.SetTrigger("die");

        // Destroy all summoned mini-enemies
        foreach (var miniEnemy in spawnedMiniEnemies)
        {
            if (miniEnemy != null)
            {
                // miniEnemy.GetComponent<Explosion>().isMinion = false;
                Destroy(miniEnemy);
            }
        }
        //StartCoroutine(DisableAfterDeath());
    }

    // public void DestroyMinions()
    // {
    //     // Destroy all summoned mini-enemies
    //     foreach (var miniEnemy in spawnedMiniEnemies)
    //     {
    //         if (miniEnemy != null)
    //         {
    //             miniEnemy.GetComponent<Explosion>().isMinion = false;
    //             Destroy(miniEnemy);
    //         }
    //     }
    // }

    /**private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(2f); // Wait for death animation

        if (enemyController != null)
        {
            Debug.Log("Spawning loot...");
            enemyController.SpawnLoot();
            yield return new WaitForSeconds(0.5f); // Ensure loot has time to spawn
        }
        else
        {
            Debug.LogError("EnemyController is NULL! Loot cannot spawn.");
        }

        gameObject.SetActive(false);
    }**/


    /// <summary>
    /// Periodically heals the boss.
    /// </summary>
    private IEnumerator HealPeriodically()
    {
        while (!isDead)
        {
            // Wait for a random period (e.g., between 5 to 10 seconds)
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            // Use the heal skill
            UseSkill1();
        }
    }

    /// <summary>
    /// Periodically checks for dead mini-enemies and respawns them.
    /// </summary>
    private IEnumerator CheckForDeadMiniEnemies()
    {
        while (!isDead)
        {
            // Check periodically (e.g., every 2 seconds) for dead mini-enemies
            yield return new WaitForSeconds(2f);

            for (int i = spawnedMiniEnemies.Count - 1; i >= 0; i--)
            {
                if (spawnedMiniEnemies[i] == null)
                {
                    // If a mini-enemy is dead (destroyed), respawn a new one
                    RespawnMiniEnemy(spawnedMiniEnemies[i]);
                    spawnedMiniEnemies.RemoveAt(i);  // Remove the reference from the list
                }
            }
        }
    }

    public void OnDeath()
	{
        GameObject.Find("TriggerForBossFightExecutioner").SetActive(false);
        healthBar.gameObject.SetActive(false);
        gameObject.SetActive(false);
        SpawnObject(new Vector3(43.77f, -35f, -4.331337f));
    }

    public void SpawnObject(Vector3 position)
    {
        // Create a new GameObject
        GameObject newObject = new GameObject("MyDynamicObject");

        // Set its position and rotation
        newObject.transform.position = position;

        // Add the desired component
        newObject.AddComponent<Checkpoint>();
        BoxCollider2D collider = newObject.AddComponent<BoxCollider2D>();
        collider.offset = new Vector2(2, (float)0.4);
        collider.size = new Vector2(5, (float)0.1);
        collider.isTrigger = true;
        // Optionally, modify the component
        // myComponent.DoSomething();
    }

    private void DeactivateBoss()
	{
		healthBar.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

    /// <summary>
    /// Draws gizmos in the editor to visualize the detection range.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void AttackSound()
    {
        audioManager.PlaySFX(audioManager.enemy_attack);
    }
}
