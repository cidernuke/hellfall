using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // for the merge
    [Header("Attack Parameters")]
    public float closeRangeAttackCooldown;
    public float rangedAttackCooldown;
    public float range;
    public float closeDamage;
    public float rangedDamage;
    [SerializeField] private float damage;

    // for special attacks
    [SerializeField] private bool fireDamage = false;
    [SerializeField] private bool iceDamage = false;
    [SerializeField] private bool isFrozen = false;



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
    private AudioManager audioManager;

    //variables for attack
    private int attackIndex = 0;
    private int totalAttacks = 2;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        enemyHealth = GetComponent<HealthSystem>();
        enemyController = GetComponent<EnemyController>();
        playerController = GetComponent<PlayerController>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();


    }

    /// <summary>
    /// Checks if the player is able to attack and if the cooldown is over.
    /// Calls the Attack() method if left mouse button is clicked.
    /// Calls the AttackRanged() method if right mouse button is clicked.
    /// </summary>
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cooldownTimer >= closeRangeAttackCooldown)
        {
            Attack();
        }
        //print(cooldownTimer >= rangedAttackCooldown);
        if (Input.GetMouseButtonDown(1) && cooldownTimer >= rangedAttackCooldown)
        {
            AttackRanged();
        }
        cooldownTimer += Time.deltaTime;
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
                audioManager.PlaySFX(audioManager.closeAttack_01);     
                break;
            case 1:
                anim.SetTrigger("attack_02");
                audioManager.PlaySFX(audioManager.closeAttack_02);
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
        if (!playerMovement.isFalling)
        {
            playerMovement.blockInput = true;
            playerMovement.body.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        audioManager.PlaySFX(audioManager.rangedAttack);
        anim.SetTrigger("attack_ranged");
        cooldownTimer = 0;
        int projectileIndex = FindProjectile();

        // important because projectiles are children of Player. Setting them to null makes them independant of the Players Transform
        projectiles[projectileIndex].transform.parent = null;
        projectiles[projectileIndex].transform.position = firePoint.position;

        int directionX = Math.Sign(transform.localScale.x);
        projectiles[projectileIndex].GetComponent<Projectile>().SetDirection(new Vector2(directionX, 0));
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

    /// <summary>
    /// Deals damage to the enemy if it is in sight.
    /// </summary>
    private void DamageEnemy()
    {
        if (EnemyInSight())
        {            
            if (fireDamage)
            {
                //Note: TakeDamage still needs to be called, since fire damage is only secondary and applied over time
                enemyHealth.TakeDamage(damage, null, enemyController);
                enemyHealth.ApplyFireDamage(damage, 2f, 1f);
            }
            else if (iceDamage)
            {
                //Note: TakeDamage still needs to be called, since ice damage only freezes the opponent
                enemyHealth.TakeDamage(damage, null, enemyController);
                //FreezeEnemy();
            }
            else
            {
                enemyHealth.TakeDamage(damage, null, enemyController);
            }
        }
    }

    /// <summary>
    /// Freezes the Enemy, by disabling the EnemyPatrol Script for a certain amount of time.
    /// </summary>
    /* private IEnumerator FreezeEnemy()
    {
        if (!isFrozen)
        {
            //broken
            //enemyController.enemyPatrol.enabled = false
            //CooldownTimer set to 0, so the enemy can't attack for a second

            enemyController.cooldownTimer = 0;
            yield return new WaitForSeconds(1f);
            //enemyController.GetComponent<EnemyPatrol>().enabled = true;
        }
        yield return null;

    }*/
}
