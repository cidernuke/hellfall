using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    // variables
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float maxLifetime = 2;

    public float playerDamage = 5f;
    public float playerCooldown = 0.25f;
    public float playerRange = 0.7f;
    private bool hit;
    private float directionX;
    private float directionY;
    private float lifeTime;

    // Relevant for Vampire Countess Boss
    private float waitTime;
    private bool isWaiting;
    public bool isVampireBitch = false;

    // references
    private BoxCollider2D boxCollider;
    private Animator animator;
    public PlayerAttack attackSys;

    //damage variants
    public bool isFireBullet = false;
    public bool isIceBullet = false;
    [SerializeField] private bool isFrozen = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit)
        {
            return;
        }

        // Handle waiting before movement. Used for downward attack of the Vampire Countess Boss. Stops projectiles from immediatly moving after spawning.
        if (isWaiting)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                isWaiting = false; // Stop waiting
            }
            return; // Skip movement during the wait
        }

        float movementSpeed = speed * Time.deltaTime;
        if (transform.CompareTag("blood_bullet_up"))
        {
            transform.Translate(0, movementSpeed * directionY, 0);
        }
        else if (transform.CompareTag("blood_bullet_down"))
        {
            transform.Translate(0, movementSpeed * directionY, 0);
        }
        else // Default movement speed of a horizontal projectile, mainly from the player.
        {
            transform.Translate(movementSpeed * directionX, 0, 0);
        }

        SetLifeTime();
    }

    /// <summary>
    /// Sets the lifetime of the projectile.
    /// </summary>
    private void SetLifeTime()
    {
        lifeTime += Time.deltaTime;

        // if (lifeTime >= maxLifetime)
        // {
        //      gameObject.SetActive(false);
        // }

        if(isVampireBitch)
        {
            if (lifeTime >= maxLifetime)
            {
                gameObject.SetActive(false);
            }
        } else
        {
            if (lifeTime >= playerRange)
            {
                gameObject.SetActive(false);
            }
        }
        
    }

    /// <summary>
    /// Checks for collision with walls and enemies.
    /// activates the explosion animation and deactivates the projectile.
    /// if the projectile hits an enemy, the enemy takes damage.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            hit = true;
            boxCollider.enabled = false;
            animator.SetTrigger("explode");
        }

        if (collision.CompareTag("Enemy"))
        {
            //Get Components and check if they exist
            EnemyController enemyController = collision.GetComponent<EnemyController>();

            if (enemyController == null)
            {
                Debug.Log("EnemyController is null");
                return;
            }
            HealthSystem enemyHealth = collision.GetComponent<HealthSystem>();
            if (enemyHealth == null)
            {
                Debug.Log("EnemyHealthSystem is null");
                return;
            }

            //Apply Damage
            enemyHealth.TakeDamage(playerDamage, null, enemyController);

            //für den fall dass es ice bullets sind
            if (isIceBullet)
            {
                enemyController.ApplyIceEffect();
            }
            //für den fall dass es ice bullets sind
            if (isFireBullet)
            {
                enemyHealth.ApplyFireDamage(playerDamage, 2f, 1f);
                enemyController.ApplyFireEffect(2f);
            }

            hit = true;
            boxCollider.enabled = false;
            animator.SetTrigger("explode");
        }

        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (collision.GetComponent<HealthSystem>() == null)
            {
                return;
            }
            collision.GetComponent<HealthSystem>().TakeDamage(damage, playerMovement, null);
            if (!transform.CompareTag("blood_bullet_up") && !transform.CompareTag("blood_bullet_down"))
            {
                hit = true;
                boxCollider.enabled = false;
                animator.SetTrigger("blood_bullet_hit");
            }
        }
    }

    /// <summary>
    /// Sets the direction of the projectile and activates it.
    /// Sets the lifetime to 0.
    /// </summary>
    /// <param name="_direction">The direction of the projectile.</param>
    public void SetDirection(Vector2 _direction, float _waitTime = 0)
    {
        lifeTime = 0;
        directionX = _direction.x;
        directionY = _direction.y;
        waitTime = _waitTime; // Set the wait time
        isWaiting = waitTime > 0; // Determine if the projectile should wait
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction.x)
        {
            localScaleX = -localScaleX;
        }

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Deactivates the projectile.
    /// gets called by the animation "explode" event.
    /// </summary>
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

   
}
