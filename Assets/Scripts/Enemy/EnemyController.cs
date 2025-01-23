using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // for the merge
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float height;
    [SerializeField] private float damage;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    [Header("Loot")]
    [SerializeField] public List<LootItem> lootTable = new List<LootItem>();

    // References
    private Animator anim;
    private HealthSystem playerHealth;
    public EnemyPatrol enemyPatrol;
    private PlayerMovement playerMovement;
    public HealthSystem healthSystem;

    // Respawn variables
    private Vector3 initialPosition;
    private bool isDead = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerController>().healthSystem;
        playerMovement = GameObject.Find("Player").GetComponent<PlayerController>().playerMovementSystem;
        healthSystem = GetComponent<HealthSystem>();

        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem component not found on Enemy.");
        }

        initialPosition = transform.position;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterEnemy(this);
        }
        else
        {
            Debug.LogError("GameManager.Instance is null, Enemy cannot be registered.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Increments the cooldown timer for the time that passed since the last frame
        cooldownTimer += Time.deltaTime;

        // Only attacks if player is in sight
        if (PlayerInSight())
        {
            // Checks if cooldown timer has expired
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack");
            }
        }

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }
    }

    /// <summary>
    ///Performs a BoxCast to check if player is in reach.
    ///@return true if player is in reach, otherwise false.
    /// </summary>
    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y * height, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<HealthSystem>();

            // So that the enemy stops attacking the player after death
            if (playerHealth.currentHealth <= 0)
            {
                return false;
            }

            // Retrieve the PlayerMovement object when the player is in sight
            playerMovement = hit.transform.GetComponent<PlayerMovement>();
        }
        return hit.collider != null;
    }

    /// <summary>
    ///Draws a box model to visualize the area of the BoxCast
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y * height, boxCollider.bounds.size.z));
    }

    /// <summary>
    ///If player is in sight, the player takes damage.
    ///</summary>
    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            playerHealth.TakeDamage(damage, playerMovement);
        }
    }

    /// <summary>
    ///Spawns loot items from the loot table.
    ///If the random number is less than the drop chance, the item is spawned.
    ///</summary>
    public void SpawnLoot()
    {
        foreach (LootItem lootItem in lootTable)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                Vector3 lootPosition = transform.position - new Vector3(0, 1, 0);
                Instantiate(lootItem.itemPrefab, lootPosition, Quaternion.identity);
            }
        }
    }

    /// <summary>
    ///  Gets the initial position of the enemy.
    ///  </summary>
    /// <returns>The initial position of the enemy.</returns>
    public Vector3 GetInitialPosition()
    {
        return initialPosition;
    }

    /// <summary>
    ///Handles the enemy's death.
    ///</summary>
    public void OnDeath()
    {
        isDead = true;
        gameObject.SetActive(false);
    }
}
