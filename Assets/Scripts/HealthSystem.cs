using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    //Respawn variables
    private bool isDead;
    public float respawnHealth;
    //[SerializeField] private TMP_Text dietMessage;

    /// <summary>
    /// Initializes the current health to the starting health value.
    /// </summary>
    private void Awake()
    {
        currentHealth = startingHealth;
        respawnHealth = startingHealth; //To avoid null-pointers
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

        //     Vector3 playerPos = transform.position;
        // GameManager.Instance.ShowDeathMessage(playerPos);

            anim.SetTrigger("die");
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                isDead = true;

                StartCoroutine(RespawnPlayer(playerMovement));

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

    // Coroutine zum Respawnen des Spielers
    private IEnumerator RespawnPlayer(PlayerMovement playerMovement)
    {
        // Warte auf die Todesanimation
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // Rufe die Respawn-Methode des GameManagers auf
        GameManager.Instance.RespawnPlayer();

        // Aktiviere die Spielerbewegung wieder
        playerMovement.enabled = true;

        // Setze isDead zurück
        isDead = false;

        //Befor Refactoring:

        // // wait for the die animation
        // yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // //Works so the Player get his inital LP
        // // reset health
        // //currentHealth = startingHealth;

        // currentHealth = respawnHealth;
        // print("Player health after respawn: " + currentHealth);

        // // Later: Update the health-bar
        // // UIHandler.instance.SetHealthValue(currentHealth / startingHealth);

        // // Call the Respawn method from playerMovement
        // playerMovement.Respawn();
        // //isInvincible = false;

        // // Reset isDead
        // isDead = false;
    }
}
