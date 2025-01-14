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
    [SerializeField] private GameObject miniEnemyPrefab;
    [SerializeField] private Transform[] summonPoints;
    [SerializeField] private int maxMiniEnemies = 3;

    [Header("Skill Parameters")]
    [SerializeField] private float healAmount = 50f;

    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private HealthSystem healthSystem;
    private EnemyController enemyController;
    private Transform playerTransform;

    private List<GameObject> spawnedMiniEnemies = new List<GameObject>();
    private bool facingRight = true;
    private bool isDead = false;

    private void Start()
    {
        StartCoroutine(HealPeriodically());
    }

    private void Awake()
    {
        // Assign existing components
        healthSystem = GetComponent<HealthSystem>();
        enemyController = GetComponent<EnemyController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead) return;

        // Check if health reaches zero
        if (healthSystem.currentHealth <= 0)
        {
            HandleDeath();
        }

        // Follow the player if in sight
        if (PlayerInSight())
        {
            FollowPlayer();
        }

        // Summon mini-enemies if below 50% health
        if (healthSystem.currentHealth < healthSystem.startingHealth * 0.5f && spawnedMiniEnemies.Count < maxMiniEnemies)
        {
            SummonMiniEnemies();
        }
    }

    private bool PlayerInSight()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (hit != null)
        {
            playerTransform = hit.transform;
            return true;
        }

        playerTransform = null;
        return false;
    }

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


    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void SummonMiniEnemies()
    {
        anim.SetTrigger("Summon");
        Debug.Log("Summoning mini enemies...");

        foreach (var point in summonPoints)
        {
            if (spawnedMiniEnemies.Count >= maxMiniEnemies) break;

            GameObject miniEnemy = Instantiate(miniEnemyPrefab, point.position, Quaternion.identity);
            spawnedMiniEnemies.Add(miniEnemy);

            // Ensure spawned enemies use existing EnemyController and HealthSystem
            var enemyController = miniEnemy.GetComponent<EnemyController>();
            if (enemyController)
            {
                enemyController.enabled = true;
            }

            var miniHealthSystem = miniEnemy.GetComponent<HealthSystem>();
            if (miniHealthSystem)
            {
                miniHealthSystem.enabled = true;
            }
        }
    }


    public void UseSkill1()
    {
        if (isDead) return;

        anim.SetTrigger("Skill");

        // Heal the boss
        healthSystem.AddHealth(healAmount);
        Debug.Log($"Boss healed by {healAmount}, current health: {healthSystem.currentHealth}");
    }

    private void HandleDeath()
    {
        if (isDead) return; // Ensure death logic is only triggered once
        isDead = true;

        // Play death animation
        anim.SetTrigger("die");

        // Destroy all summoned mini-enemies
        foreach (var miniEnemy in spawnedMiniEnemies)
        {
            if (miniEnemy != null)
            {
                Destroy(miniEnemy);
            }
        }

        StartCoroutine(DisableAfterDeath());
    }

    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(2f); // Wait for the death animation
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

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

}
