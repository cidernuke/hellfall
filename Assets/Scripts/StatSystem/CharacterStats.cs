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
}