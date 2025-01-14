using UnityEngine;

public class CharacterStats : MonoBehaviour 
{

    // Overall Stats
    // -- Health
    public Stat maxHealth;

    // Attack
    // -- Close-Combat
    public Stat strength;       // Strength Attribute (influences all closeRange values)
    public Stat closeDamage;    // Damage that the player makes with closeRange weapon
    public Stat closeAccuracy;  // Accuracy for closeRange Attack (StandardValue = 100f)
    public Stat closeCooldown;  // Cooldown for closeRange Attack (StandardValue = 0f)
    // public Stat closeRange;

    // -- Ranged
    public Stat intelligence;   // Intelligence Attribute (influences all longRange values)
    public Stat rangedDamage;   // Damage that the player makes with longRange weapon
    public Stat rangedAccuracy; // Accuracy for longRange Attack (StandardValue = 100f)
    public Stat rangedCooldown; // Cooldown for longRange Attack (StandardValue = ?)
    public Stat rangedRange;    // Range for longRange Attack (StandardValue = ?)

    public CharacterStats(
        float maxHealth,
        float strength,
        float closeDamage,
        float closeAccuracy,
        float closeCooldown,
        // float closeRange,
        float intelligence,
        float rangedDamage,
        float rangedAccuracy,
        float rangedCooldown,
        float rangedRange
    ) {
        this.maxHealth = new Stat(maxHealth);
        this.strength = new Stat(strength);
        this.closeDamage = new Stat(closeDamage);
        this.closeAccuracy = new Stat(closeAccuracy);
        this.closeCooldown = new Stat(closeCooldown);
        // this.closeRange = new Stat(closeRange);
        this.intelligence = new Stat(intelligence);
        this.rangedDamage = new Stat(rangedDamage);
        this.rangedAccuracy = new Stat(rangedAccuracy);
        this.rangedCooldown = new Stat(rangedCooldown);
        this.rangedRange = new Stat(rangedRange);
    }
}