using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Triggers;
using ItemSystem.Abstract;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    // Script References
    [Header("Scripts")]

    [SerializeField] public HealthSystem healthSystem;
    [SerializeField] public PlayerAttack playerAttackSystem;
    [SerializeField] public InventorySystem inventorySystem;
    public CharacterStats playerStats;

    [Header("Player Components")]
    [SerializeField] public Animator anim;
    [SerializeField] public BoxCollider boxCollider;

    [Header("Character Stats")]
    [SerializeField] public float vitality = 1f;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float currentHealth = 100f;

    // -- Close
    [SerializeField] public float strength = 1f;
    [SerializeField] public float closeDamage = 5f;

    // -- Ranged
    [SerializeField] public float intelligence = 1f;
    [SerializeField] public float rangedDamage = 5f;
    [SerializeField] public float rangedCooldown = 0.25f;
    [SerializeField] public float rangedRange = 2f;



    void Awake()
    {
        // Ensure that HealthSystem is assigned either via inspector or automatically
        if (healthSystem == null)
        {
            healthSystem = GetComponent<HealthSystem>();  // Automatically find it
        }

        if (playerAttackSystem == null)
        {
            playerAttackSystem = GetComponent<PlayerAttack>();
        }

        if (inventorySystem == null)
        {
            inventorySystem = GetComponent<InventorySystem>();
        }

        if (playerStats == null)
        {
            playerStats = new CharacterStats(
                vitality,
                maxHealth,
                strength,
                closeDamage,
                intelligence,
                rangedDamage,
                rangedCooldown,
                rangedRange
            );
        }
    }

    void Update()
    {
        // Update HealthSystem values
        if (healthSystem != null)
        {
            // if startingHealth in healthSystem isn't the same as in playerStats, then value in HealthSystem is changed
            // Example: maxHealth value is increased through Upgrade Altar
            if (healthSystem.startingHealth != playerStats.maxHealth.GetCalcValue())
            {
                healthSystem.startingHealth = playerStats.maxHealth.GetCalcValue();
            }

            // if currentHealth is changed in healthSystem, then new value is set for local variable currentHealth in PlayerControllerx
            if (healthSystem.currentHealth != currentHealth)
            {
                currentHealth = healthSystem.currentHealth;
            }
        }

        // Update AttackSystem values
        if (playerAttackSystem != null)
        {
            // if closeDamage value in PlayerAttack isn't the same as in playerStats, then value in PlayerAttack is changed
            if (playerAttackSystem.closeDamage != playerStats.closeDamage.GetCalcValue())
            {
                playerAttackSystem.closeDamage = playerStats.closeDamage.GetCalcValue();
            }

            // if rangedDamage value in PlayerAttack isn't the same as in playerStats, then value in PlayerAttack is changed
            if (playerAttackSystem.rangedDamage != playerStats.rangedDamage.GetCalcValue())
            {
                playerAttackSystem.rangedDamage = playerStats.rangedDamage.GetCalcValue();
            }

            // if rangedAttackCooldown value in PlayerAttack isn't the same as in playerStats, then value in PlayerAttack is changed
            if (playerAttackSystem.rangedAttackCooldown != playerStats.rangedCooldown.GetCalcValue())
            {
                playerAttackSystem.rangedAttackCooldown = playerStats.rangedCooldown.GetCalcValue();
            }

            // if rangedAttackRange value in PlayerAttack isn't the same as in playerStats, then value in PlayerAttack is changed
            if (playerAttackSystem.range != playerStats.rangedRange.GetCalcValue())
            {
                playerAttackSystem.range = playerStats.rangedRange.GetCalcValue();
            }
        }

        //Update CharacterStats based on active Weapons
        if (inventorySystem != null)
        {
            // slots[4] in Inventory = Close Range Weapon
            ShortRangeWeapon closeRangeWeapon = inventorySystem.slots[3].storedItem as ShortRangeWeapon;
            ItemData itemDataClose = inventorySystem.slots[3].itemData;
            // Checks if there is a closeRangeWeapon
            if (closeRangeWeapon != null)
            {
                // Communicates the Modifier values from the close range weapon to the player Stats, if they have changed
                if (closeRangeWeapon.weaponStats.GetDamageModifier() != playerStats.closeDamage.GetModifier())
                {
                    playerStats.closeDamage.SetModifier(closeRangeWeapon.weaponStats.GetDamageModifier());
                }

                //Communicate the type of the weapon equipped, to the player Attack Script
                if (itemDataClose.isFire == true)
                {
                    playerAttackSystem.iceDamageClose = false;
                    playerAttackSystem.fireDamageClose = true;
                }
                if (itemDataClose.isIce == true)
                {
                    playerAttackSystem.iceDamageClose = true;
                    playerAttackSystem.fireDamageClose = false;
                }

            }
            // slots[4] in Inventory = Long Range Weapon
            LongRangeWeapon longRangeWeapon = inventorySystem.slots[4].storedItem as LongRangeWeapon;
            ItemData itemDataRange = inventorySystem.slots[4].itemData;
            // Checks if there is a longRangeWeapon
            if (longRangeWeapon != null)
            {
                // Communicates the Modifier values from the long range weapon to the player Stats, if they have changed
                if (longRangeWeapon.weaponStats.GetDamageModifier() != playerStats.rangedDamage.GetModifier())
                {
                    playerStats.rangedDamage.SetModifier(longRangeWeapon.weaponStats.GetDamageModifier());
                }
                if (longRangeWeapon.weaponStats.GetCooldownModifier() != playerStats.rangedCooldown.GetModifier())
                {
                    playerStats.rangedCooldown.SetModifier(longRangeWeapon.weaponStats.GetCooldownModifier());
                }
                if (longRangeWeapon.weaponStats.GetRangeModifier() != playerStats.rangedCooldown.GetModifier())
                {
                    playerStats.rangedRange.SetModifier(longRangeWeapon.weaponStats.GetRangeModifier());
                }

                //Communicate the type of the weapon equipped, to the player Attack Script
                if (itemDataRange.isFire == true)
                {
                    playerAttackSystem.iceDamageRange = false;
                    playerAttackSystem.fireDamageRange = true;
                }
                if (itemDataRange.isIce == true)
                {
                    playerAttackSystem.iceDamageRange = true;
                    playerAttackSystem.fireDamageRange = false;
                }
            }
        }


    }
}
