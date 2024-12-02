using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // for the merge
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float damage;

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

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        enemyHealth = GetComponent<HealthSystem>();
        enemyController = GetComponent<EnemyController>();        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cooldownTimer >= attackCooldown && playerMovement.CanAttack())
        {
            Attack();
        }
        if (Input.GetMouseButtonDown(1) && cooldownTimer >= attackCooldown && playerMovement.CanAttack())
        {
            projectiles[0].SetActive(true);
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
    private void Attack()
    {
        anim.SetTrigger("attack_01");
        cooldownTimer = 0;
    }

    private void AttackRanged()
    {
        
        anim.SetTrigger("attack_ranged");
        cooldownTimer = 0;
        int projectileIndex = FindProjectile();
        projectiles[projectileIndex].transform.position = firePoint.position;
        projectiles[projectileIndex].GetComponent<Projectile>().SetDirection(Math.Sign(transform.localScale.x));
        
    }
    
    private int FindProjectile()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (!projectiles[i].activeInHierarchy)
            {
                Debug.Log("Projectile active" + i);
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
            enemyHealth.TakeDamage(damage, null, enemyController);
        }
    }
}
