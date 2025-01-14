using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // for the merge
    [Header("Attack Parameters")]
    [SerializeField] public float closeRangeAttackCooldown;
    [SerializeField] public float rangedAttackCooldown;
    [SerializeField] public float range;
    [SerializeField] public float closeDamage;
    [SerializeField] public float rangedDamage;

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

    //variables for attack
    private int attackIndex = 0;
    private int totalAttacks = 2;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        enemyHealth = GetComponent<HealthSystem>();
        enemyController = GetComponent<EnemyController>();
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
    /// Triggers the ranged attack animation and resets the cooldown timer.
    /// Spawns a projectile and sets the direction.
    /// </summary>
    private void AttackRanged()
    {
        anim.SetTrigger("attack_ranged");
        cooldownTimer = 0;
        int projectileIndex = FindProjectile();
        projectiles[projectileIndex].transform.position = firePoint.position;
        projectiles[projectileIndex].GetComponent<Projectile>().SetDirection(Math.Sign(transform.localScale.x));

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

    /*
    * --DamageEnemy()--
    * If Enemy is in Sight, the Enemy takes damage.
    **/
    private void DamageEnemy()
    {
        if (EnemyInSight())
        {
            enemyHealth.TakeDamage(closeDamage, null, enemyController);
        }
    }
}
