using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the health system for a game object, including taking damage and temporary invincibility.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    /// <summary>
    /// The initial amount of health the object starts with.
    /// </summary>
    [SerializeField] public float startingHealth = 100;

    /// <summary>
    /// The current amount of health the object has.
    /// </summary>
    public float currentHealth;
    [SerializeField] private Animator anim;
    [SerializeField] private bool isPlayer = false;

    private bool isDead;


    //zum Awake wird current = starting gesetzt

    /// <summary>
    /// The duration for which the object is invincible after taking damage.
    /// </summary>
    public float timeInvincible = 0.5f;

    /// <summary>
    /// Indicates whether the object is currently invincible.
    /// </summary>
    public bool isInvincible;

    /// <summary>
    /// The remaining cooldown time for invincibility.
    /// </summary>
    private float damageCooldown;

    /// <summary>
    /// Initializes the current health to the starting health value.
    /// </summary>
    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        
    }

    /// <summary>
    /// Updates the invincibility status and cooldown timer.
    /// </summary>
    public void Update()
    {
        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }
    }

    /// <summary>
    /// Reduces the current health by the specified damage amount and handles invincibility and death.
    /// Pass in the PlayerMovement or EnemyController script to disable movement on death. 
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    public void TakeDamage(float damage, PlayerMovement playerMovement = null, EnemyController enemyController = null)
    {
        if (damage > 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        if (currentHealth > 0)
        {
            Debug.Log("Current Health: " + currentHealth + "/ Starting Health: " + startingHealth);
            anim.SetTrigger("hurt");
            if (isPlayer)
            {
                UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
            }
            
        }
        else
        {
            Die(playerMovement, enemyController);

        }
    }

    /// <summary>
    /// Increases the current health by the specified health amount and updates the health UI.
    /// </summary>

    public void AddHealth(float healthAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + healthAmount, 0, startingHealth);
        Debug.Log("Current Health: " + currentHealth + "/ Starting Health: " + startingHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
    }

    #region Death funtionality

    /// <summary>
    /// Kills the player or enemy, disabling their movement and triggering the death animation.
    /// If Enemy is killed, loot is spawned.
    /// </summary>
    /// <param name="playerMovement"></param>
    /// <param name="enemyController"></param>

    private void Die(PlayerMovement playerMovement = null, EnemyController enemyController = null)

    {
        if (!isDead)
        {
            Debug.Log("Player died");
            anim.SetTrigger("die");
            if (playerMovement != null)
            {
                playerMovement.enabled = false;

            }
            else if (enemyController != null)
            {
                enemyController.enabled = false;
                StartCoroutine(AutoDestroy.DestroyAfterAnimation(anim, enemyController.gameObject, 0.4f));
                enemyController.SpawnLoot();

            }
            isDead = true;

        }
    }
    #endregion


}
