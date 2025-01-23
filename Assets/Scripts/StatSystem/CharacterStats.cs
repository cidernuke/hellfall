using UnityEngine;

public class CharacterStats 
{
    // Overall Stats
    // -- Health
    public Stat vitality;
    public Stat maxHealth;      // maximum health the player can have

    // Attack
    // -- Close-Combat
    public Stat strength;       // Strength Attribute (influences all closeRange values)
    public Stat closeDamage;    // Damage that the player makes with closeRange weapon

    // -- Ranged
    public Stat intelligence;   // Intelligence Attribute (influences all longRange values)
    public Stat rangedDamage;   // Damage that the player makes with longRange weapon
    public Stat rangedCooldown; // Cooldown for longRange Attack (StandardValue = ?)
    public Stat rangedRange;    // Range for longRange Attack (StandardValue = ?)

    public CharacterStats(
        float vitality,
        float maxHealth,
        float strength,
        float closeDamage,
        float intelligence,
        float rangedDamage,
        float rangedCooldown,
        float rangedRange
    ) {
        this.vitality = new Stat(vitality);
        this.maxHealth = new Stat(maxHealth);
        this.strength = new Stat(strength);
        this.closeDamage = new Stat(closeDamage);
        this.intelligence = new Stat(intelligence);
        this.rangedDamage = new Stat(rangedDamage);
        this.rangedCooldown = new Stat(rangedCooldown);
        this.rangedRange = new Stat(rangedRange);
    }

    /// <summary>
    /// Increments the vitality stat by 1, if it is 9 or less.
    /// Also increases maxHealth by 10.
    /// </summary>
    public void IncrementVitality()
    {
        if(vitality.GetBaseValue() <= 9)
        {
            vitality.SetBaseValue(vitality.GetBaseValue() + 1f);

            maxHealth.SetBaseValue(maxHealth.GetBaseValue() + 10f);
        }
    }

    /// <summary>
    /// Decreases the vitality stat by 1, if it is between 1 and 10.
    /// Also decreases maxHealth by 10.
    /// Logs a message if vitality is 0.
    /// </summary>
    public void DecreaseVitality()
    {
        if(vitality.GetBaseValue() == 0)
        {
            Debug.Log("Vitality cannot be lower than 0");
        }
        
        if(vitality.GetBaseValue() >= 1 && vitality.GetBaseValue() <= 10)
        {
            vitality.SetBaseValue(vitality.GetBaseValue() - 1f);

            maxHealth.SetBaseValue(maxHealth.GetBaseValue() - 10f);
        }
    }

    /// <summary>
    /// Increments the strength stat by 1, if it is 9 or less.
    /// Also increases closeDamage by 5.
    /// </summary>
    public void IncrementStrength()
    {
        if(strength.GetBaseValue() <= 9)
        {
            strength.SetBaseValue(strength.GetBaseValue() + 1f);

            closeDamage.SetBaseValue(closeDamage.GetBaseValue() + 5f);
        }
    }

    /// <summary>
    /// Decreases the strength stat by 1, if it is between 1 and 10.
    /// Also decreases closeDamage by 5.
    /// </summary>
    public void DecreaseStrength()
    {
        if(strength.GetBaseValue() >= 1 && strength.GetBaseValue() <= 10)
        {
            strength.SetBaseValue(strength.GetBaseValue() - 1f);

            closeDamage.SetBaseValue(closeDamage.GetBaseValue() - 5f);
        }
    }

    /// <summary>
    /// Increments the intelligence stat by 1, if it is 9 or less.
    /// Also increases rangedDamage by 3, decreases rangedCooldown by 0.1,
    /// and increases rangedRange by 0.1.
    /// </summary>
    public void IncrementIntelligence()
    {
        if(intelligence.GetBaseValue() <= 9)
        {
            intelligence.SetBaseValue(intelligence.GetBaseValue() + 1f);

            rangedDamage.SetBaseValue(rangedDamage.GetBaseValue() + 3f);
            rangedCooldown.SetBaseValue(rangedCooldown.GetBaseValue() - 0.1f);
            rangedRange.SetBaseValue(rangedRange.GetBaseValue() + 0.1f);
        }
    }

    /// <summary>
    /// Decreases the intelligence stat by 1, if it is between 1 and 10.
    /// Also decreases rangedDamage by 3, increases rangedCooldown by 0.1,
    /// and decreases rangedRange by 0.1.
    /// </summary>
    public void DecreaseIntelligence()
    {
        if(intelligence.GetBaseValue() >= 1 && intelligence.GetBaseValue() <= 10)
        {
            intelligence.SetBaseValue(intelligence.GetBaseValue() - 1f);

            rangedDamage.SetBaseValue(rangedDamage.GetBaseValue() - 3f);
            rangedCooldown.SetBaseValue(rangedCooldown.GetBaseValue() + 0.1f);
            rangedRange.SetBaseValue(rangedRange.GetBaseValue() - 0.1f);
        }
    }
}