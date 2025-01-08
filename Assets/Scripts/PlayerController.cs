using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Triggers;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour 
{
    // Script References
    [Header("Scripts")]

    [SerializeField] public HealthSystem healthSystem;
    [SerializeField] public PlayerAttack playerAttackSystem;
    public CharacterStats playerStats;

    [Header("Player Components")]
    [SerializeField] public Animator anim;
    [SerializeField] public BoxCollider boxCollider;

    [Header("Character Stats")]
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float currentHealth = 100f;

    // -- Close
    [SerializeField] public float strength = 1f;
    [SerializeField] public float closeDamage = 5f;
    [SerializeField] public float closeAccuracy = 100f; 
    [SerializeField] public float closeCooldown = 0f;
    [SerializeField] public float closeRange = 2f;

    // -- Ranged
    [SerializeField] public float intelligence = 1f;
    [SerializeField] public float rangedDamage = 5f;
    [SerializeField] public float rangedAccuracy = 100f;
    [SerializeField] public float rangedCooldown = 0.25f;
    [SerializeField] public float rangedRange = 2f;

    

    void Awake()
    {
        playerStats = new CharacterStats(
            maxHealth,
            strength,
            closeDamage,
            closeAccuracy,
            closeCooldown,
            closeRange,
            intelligence,
            rangedDamage,
            rangedAccuracy,
            rangedCooldown,
            rangedRange
        );

        // Ensure that HealthSystem is assigned either via inspector or automatically
        if (healthSystem == null)
        {
            healthSystem = GetComponent<HealthSystem>();  // Automatically find it
        }

        if(playerAttackSystem == null)
        {
            playerAttackSystem = GetComponent<PlayerAttack>();
        }
    }

    void Update()
    {
        // Checking and updating the stats for each relevant variable
        // if (maxHealth != playerStats.maxHealth.GetBaseValue()) {
        //     playerStats.maxHealth.SetBaseValue(maxHealth);
        // }

        // if (strength != playerStats.strength.GetBaseValue()) {
        //     playerStats.strength.SetBaseValue(strength);
        // }
        // if (closeDamage != playerStats.closeDamage.GetBaseValue()) {
        //     playerStats.closeDamage.SetBaseValue(closeDamage);
        // }
        // if (closeAccuracy != playerStats.closeAccuracy.GetBaseValue()) {
        //     playerStats.closeAccuracy.SetBaseValue(closeAccuracy);
        // }
        // if (closeCooldown != playerStats.closeCooldown.GetBaseValue()) {
        //     playerStats.closeCooldown.SetBaseValue(closeCooldown);
        // }
        // if (closeRange != playerStats.closeRange.GetBaseValue()) {
        //     playerStats.closeRange.SetBaseValue(closeRange);
        // }

        // if (intelligence != playerStats.intelligence.GetBaseValue()) {
        //     playerStats.intelligence.SetBaseValue(intelligence);
        // }
        // if (rangedDamage != playerStats.rangedDamage.GetBaseValue()) {
        //     playerStats.rangedDamage.SetBaseValue(rangedDamage);
        // }
        // if (rangedAccuracy != playerStats.rangedAccuracy.GetBaseValue()) {
        //     playerStats.rangedAccuracy.SetBaseValue(rangedAccuracy);
        // }
        // if (rangedCooldown != playerStats.rangedCooldown.GetBaseValue()) {
        //     playerStats.rangedCooldown.SetBaseValue(rangedCooldown);
        // }
        // if (rangedRange != playerStats.rangedRange.GetBaseValue()) {
        //     playerStats.rangedRange.SetBaseValue(rangedRange);
        // }

        // Update HealthSystem values
        if (healthSystem != null)
        {
            if (healthSystem.startingHealth != playerStats.maxHealth.GetBaseValue()) 
            {
                healthSystem.startingHealth = playerStats.maxHealth.GetBaseValue();
            }
            if (healthSystem.currentHealth != currentHealth) 
            {
                currentHealth = healthSystem.currentHealth;
            }
        }

        // Update AttackSystem values
        if (playerAttackSystem != null)
        {
            if(playerAttackSystem.damage != playerStats.closeDamage.GetBaseValue())
            {
                playerAttackSystem.damage = playerStats.closeDamage.GetBaseValue();
            }

            if(playerAttackSystem.range != playerStats.closeRange.GetBaseValue())
            {
                playerAttackSystem.range = playerStats.closeRange.GetBaseValue();
            }

            if(playerAttackSystem.closeRangeAttackCooldown != playerStats.closeCooldown.GetBaseValue())
            {
                playerAttackSystem.closeRangeAttackCooldown = playerStats.closeCooldown.GetBaseValue();
            }

            if(playerAttackSystem.rangedAttackCooldown != playerStats.rangedCooldown.GetBaseValue())
            {
                playerAttackSystem.rangedAttackCooldown = playerStats.rangedCooldown.GetBaseValue();
            }
        }

    }
}
