using UnityEngine;
using UnityEngine.PlayerLoop;


public class WeaponStats 
{

    // Modifiers in Percent
    [SerializeField] public bool isCloseRangeWeapon;        // decides if Weapons is close or long ranged
    [SerializeField] public float damageModifier = 0f;      // modiferValue for damage Stat
    [SerializeField] public float cooldownModifier = 0f;    // modifierValue for cooldown Stat
    [SerializeField] public float rangeModifier = 0f;       // modifierValue for range Stat (only in long Range Weapon)

    public WeaponStats (bool isCloseRangeWeapon, float damageModifier = 0, float cooldownModifier = 0, float rangeModifier = 0)
    {
        this.isCloseRangeWeapon = isCloseRangeWeapon;
        this.damageModifier = damageModifier;
        this.cooldownModifier = cooldownModifier;
        this.rangeModifier = rangeModifier;
    }

    // Returns damageModifier 
    public float GetDamageModifier()
    {
        return damageModifier;
    }

    // Sets damageModifierValue equal to provided value (modifier)

    public void SetDamageModifier(float modifier)
    {
        damageModifier = modifier;
    }

    // Returns cooldownModifier 
    public float GetCooldownModifier()
    {
        return cooldownModifier;
    }

    // Sets cooldownModifierValue equal to provided value (modifier)
    public void SetCooldownModifier(float modifier)
    {
        cooldownModifier = modifier;
    }

    // Returns rangeModifier 
    public float GetRangeModifier()
    {
        return rangeModifier;
    }

    // Sets rangeModifierValue equal to provided value (modifier)
    public void SetRangeModifier(float modifier)
    {
        rangeModifier = modifier;
    }

}