using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    // for the merge
    [Header("Attack Parameters")]
    public float closeRangeAttackCooldown;
    public float rangedAttackCooldown;
    public float range;
    public float closeDamage;
    public float rangedDamage;

    // for special attacks
    [SerializeField] public bool fireDamageClose = false;
    [SerializeField] public bool iceDamageClose = false;
    [SerializeField] public bool fireDamageRange = false;
    [SerializeField] public bool iceDamageRange = false;

    //for smash attack
    [Header("Smash Attack")]
    [SerializeField] private float smashDamage; // Damage for smash attack
    [SerializeField] private float smashRange = 3f; // Range of the smash attack
    [SerializeField] private float smashCooldown = 5f; // Cooldown for the smash attack
    private float smashCooldownTimer = Mathf.Infinity; // Timer for smash attack cooldown
    public bool isSmashing = false; // Track if smash attack is active

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Enemy Layer")]
    [SerializeField] private LayerMask enemyLayer;
    private float cooldownTimer = Mathf.Infinity;

    [Header("Ranged Attack")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] projectiles;


    // references
    private Animator anim;
    private HealthSystem enemyHealth;
    private PlayerMovement playerMovement;
    private EnemyController enemyController;
    private PlayerController playerController;

    //variables for attack
    private int attackIndex = 0;
    private int totalAttacks = 2;

    public bool shortEquipped = false;
    public bool longEquipped = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        enemyController = GetComponent<EnemyController>();
        enemyHealth = GetComponent<HealthSystem>();
        playerController = GetComponent<PlayerController>();

    }

    /// <summary>
    /// Checks if the player is able to attack and if the cooldown is over.
    /// Calls the Attack() method if left mouse button is clicked and a Short Range Weapon is equipped
    /// Calls the AttackRanged() method if right mouse button is clicked and a Long Range Weapon is euqipped
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cooldownTimer >= closeRangeAttackCooldown && !Input.GetKey(KeyCode.Q))
        {
            if (shortEquipped)
            {
                Attack();
            }
        }

        if (Input.GetMouseButtonDown(1) && cooldownTimer >= rangedAttackCooldown && !Input.GetKey(KeyCode.Q))
        {
            if (longEquipped)
            {
                AttackRanged();
            }
        }

        if (playerMovement.isDoubleJumping && Input.GetKey(KeyCode.LeftShift) && cooldownTimer >= smashCooldown)
        {
            SmashAttack();
        }


        cooldownTimer += Time.deltaTime;

        smashCooldownTimer += Time.deltaTime; // Update smash attack cooldown
    }


    /*
   * --EnemyInSight()-- 
   * Performs a BoxCast to check if enemy is in reach. 
   * @return true, if enemy is in reach, if not false.
   **/
    private bool EnemyInSight()
    {

        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, enemyLayer);

        if (hit.collider != null)
        {
            enemyHealth = hit.transform.GetComponent<HealthSystem>();
            enemyController = hit.transform.GetComponent<EnemyController>();

        }
        return hit.collider != null;
    }

    /*
   * --OnDrawGizmos()--
   * Draws a Boxmodel to visualize the area of the BoxCast
   **/
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));

        // Smash Range Gizmo (Dynamic based on smashRange)
        Gizmos.color = Color.blue;
        // Use the dynamic smashRange instead of the hardcoded value
        Vector2 boxSize = new Vector2(smashRange, 1f);  // Adjust this for the desired smash width
        Vector2 boxPosition = new Vector2(transform.position.x, transform.position.y - 1.5f);  // Position below player
        Gizmos.DrawWireCube(boxPosition, boxSize);
    }

    /// <summary>
    /// Triggers the attack animation and resets the cooldown timer.
    /// Switches between the different attack animations depending on the counter.
    /// </summary>
    private void Attack()
    {
        if (!playerMovement.isFalling)
        {
            playerMovement.blockInput = true;
            playerMovement.body.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        switch (attackIndex)
        {
            case 0:
                anim.SetTrigger("attack_01");
                break;
            case 1:
                anim.SetTrigger("attack_02");
                break;
        }

        attackIndex = (attackIndex + 1) % totalAttacks;
        cooldownTimer = 0;
    }

    /// <summary>
    /// Enables player movement after the attack animation (either melee or ranged) completes
    /// </summary>
    public void OnAttackComplete()
    {
        playerMovement.blockInput = false;
        playerMovement.body.constraints = RigidbodyConstraints2D.None;
        playerMovement.body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    /// <summary>
    /// Triggers the ranged attack animation and resets the cooldown timer.
    /// Spawns a projectile and sets the direction.
    /// </summary>
    private void AttackRanged()
    {
        //if (!playerMovement.isFalling)
        //{
        //    playerMovement.blockInput = true;
        //    playerMovement.body.constraints = RigidbodyConstraints2D.FreezePosition;
        //}

        anim.SetTrigger("attack_ranged");
        cooldownTimer = 0;
        int projectileIndex = FindProjectile();

        // important because projectiles are children of Player. Setting them to null makes them independant of the Players Transform
        projectiles[projectileIndex].transform.parent = null;
        projectiles[projectileIndex].transform.position = firePoint.position;

        int directionX = Math.Sign(transform.localScale.x);
        projectiles[projectileIndex].GetComponent<Projectile>().SetDirection(new Vector2(directionX, 0));

        // Set fire or ice flags
        if (fireDamageRange)
        {

            projectiles[projectileIndex].GetComponent<Projectile>().attackSys = this;
            projectiles[projectileIndex].GetComponent<Projectile>().isFireBullet = true;
            projectiles[projectileIndex].GetComponent<Projectile>().isIceBullet = false;
        }
        if (iceDamageRange)
        {
            projectiles[projectileIndex].GetComponent<Projectile>().attackSys = this;
            projectiles[projectileIndex].GetComponent<Projectile>().isIceBullet = true;
            projectiles[projectileIndex].GetComponent<Projectile>().isFireBullet = false;
        }
    }

    /// <summary>
    /// Finds an inactive projectile in the array and returns its index.
    /// </summary>
    /// <returns></returns>
    private int FindProjectile()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (!projectiles[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }


    // Smash Attack - Deals area damage around the player
    private void SmashAttack()
    {
        if (cooldownTimer < smashCooldown || playerMovement.IsGrounded()) return;

        Debug.Log("Smash attack triggered!");
        anim.SetTrigger("smashAttack");
        smashCooldownTimer = 0;

        isSmashing = true;  // Set smash state (damage will be applied on landing)
    }

    public void ApplySmashDamage()
    {
        Debug.Log("Smash attack hits the ground!");

        Vector2 boxSize = new Vector2(3f, 1f); // Adjust width & height
        Vector2 boxPosition = new Vector2(transform.position.x, transform.position.y - 1.5f); // Position below player

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(boxPosition, boxSize, 0, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            HealthSystem enemyHealth = enemy.GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(smashDamage, null, enemy.GetComponent<EnemyController>());
            }
        }
    }


    /// <summary>
    /// Deals damage to the enemy if it is in sight.
    /// </summary>
    private void DamageEnemy()
    {
        if (EnemyInSight())
        {
            if (fireDamageClose)
            {
                //Note: TakeDamage still needs to be called, since fire damage is only secondary and applied over time
                enemyController.ApplyFireEffect(2f);
                enemyHealth.TakeDamage(closeDamage, null, enemyController);
                enemyHealth.ApplyFireDamage(closeDamage, 2f, 1f);
                return;
            }
            if (iceDamageClose)
            {
                //Note: TakeDamage still needs to be called, since ice damage only freezes the opponent
                enemyController.ApplyIceEffect();
                enemyHealth.TakeDamage(closeDamage, null, enemyController);
                return;
            }

            enemyHealth.TakeDamage(closeDamage, null, enemyController);

        }
    }
}
