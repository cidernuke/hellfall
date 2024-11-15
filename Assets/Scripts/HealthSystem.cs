using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    [SerializeField] public float startingHealth;
    public float currentHealth;
    [SerializeField] private Animator anim;

    private bool isDead;


    //zum Awake wird current = starting gesetzt
    private void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Reduces the current health by the specified damage amount, updates the health UI, 
    /// and checks if the health has dropped to zero or below.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    public void TakeDamage(float damage, PlayerMovement playerMovement = null, EnemyController enemyController = null)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        // UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
        if (currentHealth > 0)
        {
            Debug.Log("Current Health: " + currentHealth + "/ Starting Health: " + startingHealth);
            anim.SetTrigger("hurt");
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
        // UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
    }

    #region Death funtionality

    /// <summary>
    /// Kills the player or enemy, disabling their movement and triggering the death animation.
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

            }
            isDead = true;

        }
    }
    #endregion


}
