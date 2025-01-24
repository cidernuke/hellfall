using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // On Awake, set current health to starting health

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

    // Cooldown for taking damage from damage zones
    private bool isDamageZoneCooldown = false;
    private float damageZoneCooldownDuration = 1f; // 1 second

    // Flags for damage over time
    private bool isOnFire = false;

    // Respawn variables
    public bool isDead;
    public float respawnHealth;
    private float deathMessageDuration = 3f;

    // Variables and References for bosses
    private Boss bossVampire;
    private EndBossMain bossMain;
    private SpriteRenderer spriteRenderer; // Reference to the boss's SpriteRenderer
    private Color hitColor = Color.white;  // Color to show when hit
    private float flashDuration = 0.1f;    // Duration of the hit effect
    private Color originalColor;

    // private Boss bossVampire;
    // private EndBossMain bossMain;
    // private SpriteRenderer spriteRenderer; // Reference to the boss's SpriteRenderer
    // private Color hitColor = Color.white;  // Color to show when hit
    // private float flashDuration = 0.1f;    // Duration of the hit effect
    // private Color originalColor;

    // Death Messages
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
        respawnHealth = startingHealth; // To avoid null-pointers
        anim = GetComponent<Animator>();
        enemyHealthBar = GetComponentInChildren<EnemyHealthBar>();
        bossVampire = gameObject.GetComponent<Boss>();
        bossMain = gameObject.GetComponent<EndBossMain>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // Store the original color
        // originalColor = spriteRenderer.material.color;
        // spriteRenderer.material.color = Color.white;
    }

    /// <summary>
    /// Takes damage from damage zones with a global cooldown.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    public void TakeDamageFromDamageZone(float damage)
    {
        if (isDamageZoneCooldown) return;

        TakeDamage(damage);
        StartCoroutine(DamageZoneCooldown());
    }

    private IEnumerator DamageZoneCooldown()
    {
        isDamageZoneCooldown = true;
        yield return new WaitForSeconds(damageZoneCooldownDuration);
        isDamageZoneCooldown = false;
    }

    /// <summary>
    /// Updates the invincibility status and cooldown timer. 
    /// Not needed anymore, since neither player nor enemies have a cooldown time.
    /// </summary>
    //public void Update()
    //{
    //    if (isInvincible)
    //    {
    //        damageCooldown -= Time.deltaTime;
    //        if (damageCooldown < 0)
    //        {
    //            isInvincible = false;
    //        }
    //    }
    //}

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Call the logic when a new scene is loaded
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
    /// <param name="playerMovement">The PlayerMovement script to disable on death.</param>
    /// <param name="enemyController">The EnemyController script to disable on death.</param>
    public void TakeDamage(float damage, PlayerMovement playerMovement = null, EnemyController enemyController = null)
    {
        // not needed anymore, since player and enemies don't need a cooldown time
        //if (damage > 0)
        //{
        if (isInvincible)
        {
            return;
        }
        //isInvincible = true;
        //damageCooldown = timeInvincible;
        //}
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        if (currentHealth > 0)
        {
            anim.SetTrigger("hurt");
            // if (bossVampire != null && bossMain != null)
            // {
            //     StartCoroutine(FlashHitEffect());
            // }
            if (isPlayer)
            {
                UpdateHealthUI();
            }
            if (!isPlayer && bossVampire == null && bossMain == null)
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

    // private IEnumerator FlashHitEffect()
    // {
    //     if (bossMain != null)
    //     {
    //         spriteRenderer = bossMain.GetComponent<SpriteRenderer>();
    //     }
    //     else
    //     {
    //         spriteRenderer = bossVampire.GetComponent<SpriteRenderer>();
    //     }
    //     originalColor = spriteRenderer.material.color;

    //     // Change the sprite color to the hit color
    //     spriteRenderer.material.color = hitColor;

    //     // Wait for the flash duration
    //     yield return new WaitForSeconds(4);

    //     // Revert the sprite color to the original color
    //     spriteRenderer.material.color = originalColor;
    // }

    /// <summary>
    /// Increases the current health by the specified health amount and updates the health UI.
    /// </summary>
    /// <param name="healthAmount">The amount of health to add.</param>
    public void AddHealth(float healthAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + healthAmount, 0, startingHealth);
        Debug.Log("Current Health: " + currentHealth + "/ Starting Health: " + startingHealth);
        //UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
        UpdateHealthUI();
    }

    #region Death functionality

    /// <summary>
    /// Kills the player or enemy, disabling their movement and triggering the death animation.
    /// If Enemy is killed, loot is spawned.
    /// </summary>
    /// <param name="playerMovement">The PlayerMovement script to disable on death.</param>
    /// <param name="enemyController">The EnemyController script to disable on death.</param>
    private void Die(PlayerMovement playerMovement = null, EnemyController enemyController = null)
    {
        if (!isDead)
        {
            Debug.Log("Player/Enemy died");

            anim.SetTrigger("die");

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
                        enemy.GetComponent<HealthSystem>().ResetEnemySliderToFullHealth();
                    }
                }
                else
                {
                    Debug.Log("HealthSystem not found on enemyObject");
                }
            }
            else if (enemyController != null)
            {
                print("death of enemy");
                enemyController.enabled = false;
                //StartCoroutine(AutoDestroy.DestroyAfterAnimation(anim, enemyController.gameObject, 0.4f));
                StartCoroutine(HandleEnemyDeath(anim, enemyController, 0.4f));
                // enemyController.SpawnLoot();
            }
            isDead = true;
        }
    }
    #endregion

    // Coroutine to respawn the player
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

    // Coroutine to handle enemy death
    private IEnumerator HandleEnemyDeath(Animator anim, EnemyController enemyController, float animDuration)
    {
        // Wait for death animation
        yield return new WaitForSeconds(animDuration);

        // Spawn loot
        enemyController.SpawnLoot();

        //deactivate the enemy instead of destroying him
        enemyController.OnDeath();
    }

    /// <summary>
    /// Respawns the enemy at the initial position.
    /// </summary>
    /// <param name="initialPosition">The initial position to respawn the enemy.</param>
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
            enemyController.isOnFire = false;
            enemyController.isFrozen = false;
        }

        EnemyPatrol enemyPatrol = GetComponentInParent<EnemyPatrol>();
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = true;
        }
    }

    /// <summary>
    /// Applies fire damage over time. Is not primary damage.
    /// </summary>
    /// <param name="damagePerTick">Damage dealt per tick.</param>
    /// <param name="duration">Total duration of the effect.</param>
    /// <param name="tickInterval">Time between damage ticks.</param>
    public void ApplyFireDamage(float damagePerTick, float duration, float tickInterval)
    {
        if (!isOnFire)
        {
            StartCoroutine(FireDamageCoroutine(damagePerTick, duration, tickInterval));
        }
    }

    /// <summary>
    /// Handles the fire damage over time effect. Is only there for fire damage, doesn't deal primary damage.
    /// </summary>
    private IEnumerator FireDamageCoroutine(float damagePerTick, float duration, float tickInterval)
    {
        isOnFire = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(tickInterval); // wait first, then deal damage
            TakeDamage(damagePerTick * 0.1f); // only 10%
            Debug.Log("Enemy takes fire damage.");
            elapsedTime += tickInterval; // update the elapsed time
        }

        isOnFire = false;
        Debug.Log("Fire damage effect ended.");
    }

    /// <summary>
    /// Updates the health UI.
    /// </summary>
    public void UpdateHealthUI()
    {
        if (isPlayer && UIHandler.instance != null)
        {
            UIHandler.instance.SetHealthValue(currentHealth / startingHealth);
        }
    }

    /// <summary>
    /// Resets the enemy health slider to full health.
    /// </summary>
    public void ResetEnemySliderToFullHealth()
    {
        enemyHealthBar.slider.value = startingHealth;
    }
}
