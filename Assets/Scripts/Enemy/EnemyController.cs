using System.Collections.Generic;
using System.Collections;
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
    public float cooldownTimer = Mathf.Infinity;

    [Header("Loot")]
    [SerializeField] public List<LootItem> lootTable = new List<LootItem>();
    [SerializeField] private Vector3 lootPositionOffset;

    // References
    public Animator anim;
    private HealthSystem playerHealth;
    public EnemyPatrol enemyPatrol;
    private PlayerMovement playerMovement;
    public HealthSystem healthSystem;
    private UndeadExecutioner undeadExecutioner;

    private ParticleSystem flameEffect;

    // Respawn variables
    private Vector3 initialPosition;
    private bool isDead = false;

    //Flag for special Attacks
    public bool isFrozen = false;
    public bool isOnFire = false;

    // Variables
    public bool isBoss = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        playerHealth = GameObject.Find("Player").GetComponent<PlayerController>().healthSystem;
        playerMovement = GameObject.Find("Player").GetComponent<PlayerController>().playerMovementSystem;
        healthSystem = GetComponent<HealthSystem>();

        var uE = GetComponent<UndeadExecutioner>();

        // enemyHealthSystem = GetComponent<HealthSystem>();

        //for the fire effect
        flameEffect = GetComponentInChildren<ParticleSystem>();
        if (flameEffect == null)
        {
            Debug.LogError("Flame Particle System not found on enemy.");
        }

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
        if (!isBoss)
        {
            if (PlayerInSight())
            {
                // Checks if cooldown timer has expired
                if (cooldownTimer >= attackCooldown)
                {
                    cooldownTimer = 0;
                    anim.SetTrigger("meleeAttack");
                }
            }
        }

        if (enemyPatrol != null && !isBoss)
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
        if (isBoss)
        {
            return;
        }
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
        print("lootTable count: "+lootTable.Count);
        foreach (LootItem lootItem in lootTable)
        {
        print("lootitem: "+lootItem);
            if (UnityEngine.Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                Vector3 lootPosition = transform.position - lootPositionOffset;
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
        isFrozen = false;
        //frozen effekte deaktivieren
        if (this.enemyPatrol != null)
        {
            this.enemyPatrol.speed = 2.5f;
        }
        this.anim.speed = 1f;
        this.GetComponent<SpriteRenderer>().material.color = new Color(1f, 1f, 1f);

        isOnFire = false;
        isDead = true;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Starts the FreezeEnemy Coroutine
    /// </summary>
    public void ApplyIceEffect()
    {
        if (enemyPatrol == null) return; //No Ice Effect for UndeadExecutioner
        if (!enemyPatrol.enabled) return; // Prevent multiple freezes

        StartCoroutine(FreezeEnemy());
    }

    /// <summary>
    /// Freezes the Enemy, by disabling the EnemyPatrol Script for a certain amount of time.
    /// </summary>
    private IEnumerator FreezeEnemy()
    {
        if (!isFrozen)
        {
            isFrozen = true;
            Debug.Log("Enemy is Frozen");
            if (this.enemyPatrol != null)
            {
                //slow down speed for duration of freeze
                this.enemyPatrol.speed = 0.3f;
            }
            if (this.anim != null)
            {
                //slow down animation for duration of freeze
                this.anim.speed = 0.3f;
            }

            //Save the initial color of the enemy
            var initial = this.GetComponent<SpriteRenderer>().material.color;
            if (initial != null)
            {
                //Change the color of the enemy to blue
                this.GetComponent<SpriteRenderer>().material.color = new Color(0.5f, 0.8f, 0.98f);
            }

            //CooldownTimer set to 0, so the enemy can't attack for a second
            this.cooldownTimer = 0;
            Debug.Log("Enemy is frozen");
            yield return new WaitForSeconds(1f);

            //reset changed fields
            if (this.enemyPatrol != null)
            {
                this.enemyPatrol.speed = 2.5f;
            }
            if (this.anim != null)
            {
                this.anim.speed = 1f;
            }

            if (initial != null)
            {
                this.GetComponent<SpriteRenderer>().material.color = initial;
            }

            isFrozen = false;
        }


    }

    /// <summary>
    /// Starts the Fire Effect Coroutine
    /// </summary>
    public void ApplyFireEffect(float duration)
    {
        if (!isOnFire)
        {
            StartCoroutine(HandleFireEffect(duration));
        }
    }

    /// <summary>
    /// Plays the fire effect
    /// </summary>
    private IEnumerator HandleFireEffect(float duration)
    {
        isOnFire = true;

        // Enable the flame particle system
        if (flameEffect != null)
        {
            flameEffect.Play();
        }

        yield return new WaitForSeconds(duration);

        // Disable the flame particle system
        if (flameEffect != null)
        {
            flameEffect.Stop();
        }

        isOnFire = false;
    }
}
