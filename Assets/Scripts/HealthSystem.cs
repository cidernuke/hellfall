using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the health system for a game object, including taking damage and temporary invincibility.
/// </summary>
public class HealthSystem : MonoBehaviour
{
    /// <summary>
    /// The initial amount of health the object starts with.
    /// </summary>
    [SerializeField] public float startingHealth = 100;

    [SerializeField] private EnemyHealthBar enemyHealthBar;

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
    public bool isDead;
    public float respawnHealth;
    private float deathMessageDuration = 3f;

    //Death Messages
    private string[] deathMessages =
    {
        "Your torment is far from over.",
        "You are not worthy to endure the eternal flames.",
        "Failure echoes through the inferno.",
        "Rise again, or be forgotten among the damned.",
        "You are not worthy to go any further.",
        "Even hell rejects the weak.",
        "Your torment is far from over.",
        "The abyss spits you back out.",
        "The flames consume your soul, yet they grant no escape."
    };

    /// <summary>
    /// Initializes the current health to the starting health value.
    /// </summary>
    private void Awake()
    {
        currentHealth = startingHealth;
        respawnHealth = startingHealth; //To avoid null-pointers
        anim = GetComponent<Animator>();
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Rufe die Logik auf, wenn eine neue Szene geladen wurde
        if (this != null)
        {
            this.UpdateHealthUI();
        }
        else
        {
            Debug.LogWarning("HealthSystem reference is missing!");
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
                UpdateHealthUI();
                //UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
            }
            if (!isPlayer)
            {
                enemyHealthBar.updateHealthBar(currentHealth, startingHealth);
            }
        }
        else
        {
            if (isPlayer)
            {
                UIHandler.instance.SetHealthValue(0f);
            }
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
        //UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
        UpdateHealthUI();
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
            Debug.Log("Player/Enemy died");

            anim.SetTrigger("die");

            //Show death message
            // DeathUIManager.Instance.ShowDeathMessage("You are not worthy to go any further.", 3f);

            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                isDead = true;

                StartCoroutine(RespawnPlayer(playerMovement));

                //Update enemies health ui
                var enemyObject = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemyObject != null)
                {
                    foreach (var enemy in enemyObject)
                    {
                        //print("we da champs");
                        enemy.GetComponent<HealthSystem>().ResetEnemySliderToFullHealth();
                        // enemy.ResetEnemySliderToFullHealth();
                    }
                }
                else
                {
                    Debug.Log("HealthSystem not found on enemyObject");
                }
            }
            else if (enemyController != null)
            {
                enemyController.enabled = false;
                //StartCoroutine(AutoDestroy.DestroyAfterAnimation(anim, enemyController.gameObject, 0.4f));
                StartCoroutine(HandleEnemyDeath(anim, enemyController, 0.4f));
                enemyController.SpawnLoot();

            }
            isDead = true;
        }
    }
    #endregion

    // Coroutine to Respawnen the player
    private IEnumerator RespawnPlayer(PlayerMovement playerMovement)
    {
        //Wait for death animation
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        //Pick death message
        int randomIndex = UnityEngine.Random.Range(0, deathMessages.Length);
        string selectedMessage = deathMessages[randomIndex];

        //Show death message
        DeathUIManager.Instance.ShowDeathMessage(selectedMessage, deathMessageDuration);

        //Wait for death message being played
        yield return new WaitForSeconds(deathMessageDuration);

        //Call the respawn method from the GameManagers
        GameManager.Instance.RespawnPlayer();

        //Reactivate the PlayerMovement
        playerMovement.enabled = true;

        isDead = false;
    }

    // Coroutine to Respawnen the enemies
    private IEnumerator HandleEnemyDeath(Animator anim, EnemyController enemyController, float animDuration)
    {
        // Wait for death animation
        yield return new WaitForSeconds(animDuration);

        // Spawn loot
        enemyController.SpawnLoot();

        //deactivate the enemy instead of destroying him
        enemyController.OnDeath();
    }

    public void RespawnEnemy(Vector3 initialPosition)
    {
        // Reset values
        currentHealth = startingHealth;
        isDead = false;
        isInvincible = false;
        damageCooldown = 0f;

        // Reset animations
        if (anim != null)
        {
            anim.ResetTrigger("die");
            anim.ResetTrigger("hurt");
            anim.Play("Idle"); // ensure there is a default animation
        }

        // reactivate game-object
        gameObject.SetActive(true);

        // reset position
        transform.position = initialPosition;

        //Reactivate EnemyController and EnemyPatrol
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.enabled = true;
        }

        EnemyPatrol enemyPatrol = GetComponentInParent<EnemyPatrol>();
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = true;
        }
    }

    public void UpdateHealthUI()
    {
        if (isPlayer && UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(currentHealth / startingHealth);
        }
    }

    public void ResetEnemySliderToFullHealth()
    {
        enemyHealthBar.slider.value = startingHealth;
    }
}
