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
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    public void TakeDamage(float damage)
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
        Debug.Log("Current Health: " + currentHealth + "/ Starting Health: " + startingHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
        if (currentHealth <= 0)
        {
            //Die();
            Debug.Log("Player died");
        }
    }
}
