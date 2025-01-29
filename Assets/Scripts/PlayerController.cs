using System.Collections;
using Unity.VisualScripting.YamlDotNet.Serialization.ObjectGraphVisitors;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Script References
    [Header("Scripts")]

    [SerializeField] public HealthSystem healthSystem;
    [SerializeField] public PlayerAttack playerAttackSystem;
    [SerializeField] public InventorySystem inventorySystem;
    [SerializeField] public PlayerMovement playerMovementSystem;
    public CharacterStats playerStats;

    [Header("Player Components")]
    [SerializeField] public Animator anim;
    [SerializeField] public BoxCollider boxCollider;

    [Header("Character Stats")]
    [SerializeField] public float vitality = 0f;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public float currentHealth = 100f;

    // -- Close
    [SerializeField] public float strength = 0f;
    [SerializeField] public float closeDamage = 5f;

    // -- Ranged
    [SerializeField] public float intelligence = 0f;
    [SerializeField] public float rangedDamage = 5f;
    [SerializeField] public float rangedCooldown = 0.25f;
    [SerializeField] public float rangedRange = 0.7f;


    /// <summary>
    /// Initializes the PlayerController by ensuring all necessary components are assigned.
    /// If any component is not assigned via the inspector, it attempts to find the component on the GameObject.
    /// Initializes playerStats with the provided character stats.
    /// </summary>
    void Awake()
    {
        SetUpReferences(); 
    }
   

    /// <summary>
    /// Updates the PlayerController every frame.
    /// Synchronizes the health and attack system values with the player stats.
    /// Updates the character stats based on the active weapons in the inventory.
    /// </summary>
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
                healthSystem.UpdateHealthUI();
            }

            // if currentHealth is changed in healthSystem, then new value is set for local variable currentHealth in PlayerControllerx
            if (healthSystem.currentHealth != currentHealth)
            {
                currentHealth = healthSystem.currentHealth;
                healthSystem.UpdateHealthUI();
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

            // if rangedAttackCooldown value in PlayerAttack isn't the same as in playerStats, then value in PlayerAttack is changed
            if (playerAttackSystem.rangedAttackCooldown != playerStats.rangedCooldown.GetCalcValue())
            {
                playerAttackSystem.rangedAttackCooldown = playerStats.rangedCooldown.GetCalcValue();
            }


            int projectileIndex = playerAttackSystem.FindProjectile();

            for (int i = 0; i < projectileIndex; i++)
            {
                Projectile projectile = playerAttackSystem.projectiles[i].GetComponent<Projectile>();

                if ( projectile.playerDamage != playerStats.rangedDamage.GetCalcValue() ) 
                {
                    projectile.playerDamage = playerStats.rangedDamage.GetCalcValue();
                }

                if ( projectile.playerRange != playerStats.rangedRange.GetCalcValue() ) 
                {
                    projectile.playerRange = playerStats.rangedRange.GetCalcValue();
                }
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
                playerAttackSystem.shortEquipped = true;
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
                if(itemDataClose.isIce == false && itemDataClose.isFire == false)
                {
                    playerAttackSystem.iceDamageClose = false;
                    playerAttackSystem.fireDamageClose = false;
                }

            }
            //set bool to false, so player can't do a short range attack without a weapon
            if (closeRangeWeapon == null)
            {
                playerAttackSystem.shortEquipped = false;
            }

            // slots[4] in Inventory = Long Range Weapon
            LongRangeWeapon longRangeWeapon = inventorySystem.slots[4].storedItem as LongRangeWeapon;
            ItemData itemDataRange = inventorySystem.slots[4].itemData;
            // Checks if there is a longRangeWeapon
            if (longRangeWeapon != null)
            {
                playerAttackSystem.longEquipped = true;
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
                if(itemDataRange.isIce == false && itemDataRange.isFire == false)
                {
                    playerAttackSystem.iceDamageRange = false;
                    playerAttackSystem.fireDamageRange = false;
                }
            }
            //set bool to false, so player can't do a long range attack without a weapon
            if (longRangeWeapon == null)
            {
                playerAttackSystem.longEquipped = false;
            }

        }


    }

    public void SetUpReferences()
    {
        // Ensure that HealthSystem is assigned either via inspector or automatically
        if (healthSystem == null)
        {
            healthSystem = GetComponent<HealthSystem>();  // Automatically find it
            Debug.Log("Health System: " + healthSystem);
        }

        if(playerAttackSystem == null)
        {
            playerAttackSystem = GetComponent<PlayerAttack>();
        }

        if(inventorySystem == null)
        {
            inventorySystem = GetComponent<InventorySystem>();
        }

        if(playerMovementSystem == null)
        {
            playerMovementSystem = GetComponent<PlayerMovement>();
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
            Debug.Log("Player Stats: " + playerStats);
            

        }
    }


    public void handlePowerUp()
    {
        StartCoroutine(upgradeStrength());
    }


    /// <summary>
    /// Temporarily upgrades the player's strength and close damage, changes the player's color to light green,
    /// and then reverts the changes after a delay. Gets called by the Powerup Item.
    /// </summary>
    /// <returns>IEnumerator for coroutine handling.</returns>
    public IEnumerator upgradeStrength()
    {
        playerStats.strength.SetBaseValue(playerStats.strength.GetBaseValue() + 5f);

        playerStats.closeDamage.SetBaseValue(playerStats.closeDamage.GetBaseValue() + 10f);

        var initial = this.GetComponent<SpriteRenderer>().material.color;

        if (initial != null)
        {
            //Change the color of the player to light green
            this.GetComponent<SpriteRenderer>().material.color = new Color(0.5f, 0.98f, 0.8f);
        }

        yield return new WaitForSeconds(8f);

        playerStats.strength.SetBaseValue(playerStats.strength.GetBaseValue() - 5f);

        playerStats.closeDamage.SetBaseValue(playerStats.closeDamage.GetBaseValue() - 10f);

        if (initial != null)
        {
            //Change the color of the player to inital
            this.GetComponent<SpriteRenderer>().material.color = initial;
        }
    }
}
