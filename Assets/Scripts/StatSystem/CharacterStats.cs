using UnityEngine;

public class CharacterStats : MonoBehaviour 
{

    // Overall Stats
    // -- Health
    public Stat maxHealth;

    // Attack
    // -- Close-Combat
    public Stat strength; 
    public Stat closeDamage; // Damage that the player attacks with
    public Stat closeAccuracy; // ?
    public Stat closeCooldown;
    public Stat closeRange;

    // -- Ranged
    public Stat intelligence;
    public Stat rangedDamage;
    public Stat rangedAccuracy; // ?
    public Stat rangedCooldown;
    public Stat rangedRange;

    public CharacterStats(
        float maxHealth,
        float strength,
        float closeDamage,
        float closeAccuracy,
        float closeCooldown,
        float closeRange,
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
        this.closeRange = new Stat(closeRange);
        this.intelligence = new Stat(intelligence);
        this.rangedDamage = new Stat(rangedDamage);
        this.rangedAccuracy = new Stat(rangedAccuracy);
        this.rangedCooldown = new Stat(rangedCooldown);
        this.rangedRange = new Stat(rangedRange);
    }


    // void Awake ()
    // {

    // }

    // void Update ()
    // {

    // }


    

    // public void TakeDamage (int damage)
    // {
    //     currentHealth -= damage;
    //     Debug.Log(transform.name + " took " + damage + "damage.");
    // }


}