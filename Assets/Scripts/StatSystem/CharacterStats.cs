using UnityEngine;

public class CharacterStats : MonoBehaviour 
{

    // Overall Stats
    // -- Health
    public Stat health;

    // Attack
    // -- Close-Combat
    public Stat strength;
    public Stat closeDamage;
    public Stat closeAccuracy; // ?
    public Stat closeCooldown;

    // -- Ranged
    public Stat intelligence;
    public Stat rangedDamage;
    public Stat rangedAccuracy; // ?
    public Stat rangedCooldown;


    void Awake ()
    {

    }

    void Update ()
    {

    }

    // public void TakeDamage (int damage)
    // {
    //     currentHealth -= damage;
    //     Debug.Log(transform.name + " took " + damage + "damage.");
    // }


}