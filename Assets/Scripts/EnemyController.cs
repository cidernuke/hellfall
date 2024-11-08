using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private float damage;

    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    // References
    private Animator anim;
    private HealthSystem playerHealth;
    private EnemyPatrol enemyPatrol;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        playerHealth  = GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        // Increments the Cooldown-Timer for the time that past since the last frame
        cooldownTimer += Time.deltaTime;

        // Only Attacks if Player is in Sight
        if (PlayerInSight())
        {
            // Checks if Cooldown Timer has expired
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("meleeAttack");
                  
            }
        }

        if(enemyPatrol != null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }

    }
    /*
    * --PlayerInSight()-- 
    * Performs a BoxCast to check if player is in reach. 
    * @return true, if player is in reach, if not false.
    **/
    private bool PlayerInSight()
    {

        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if(hit.collider != null)
        {
            playerHealth = hit.transform.GetComponent<HealthSystem>();
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

    /*
    * --DamagePlayer()--
    * If Player is in Sight, the Player takes damage.
    **/
    private void DamagePlayer()
    {
        if(PlayerInSight())
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
