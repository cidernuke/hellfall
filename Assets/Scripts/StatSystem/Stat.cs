using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat 
{

    /*
    Each Stat has three attributes:
     - baseValue: base Stat value, influenced by strength (closeRange) or intelligence (longRange)
     - modifier: modifier value, influenced by equipped Weapon (closeRange or longRange)
     - calcValue: calculated Value = baseValue * modifier

    Example:
     - Stat: damage
        - baseValue = 5   // Value is 5, as strength = 2
        - modifier = 10   // Value is 10 (in percent), as equipped CloseRangeWeapon has a damage modifier of +10%
        - calcValue = 5.5 // Value is 5.5 = 5 (baseValue) * 10% (modifier)
    */

    // baseValue: base Stat value, influenced by strength (closeRange) or intelligence (longRange)
    [SerializeField] private float baseValue;

    // modifier: modifier value, influenced by equipped Weapon (closeRange or longRange)
    [SerializeField] private float modifier = 0f;

    // calculated Value = baseValue * modifier (in percent)
    private float calcValue = 0f;


    public Stat (float baseValue, float modifier = 0)
    {
        this.baseValue = baseValue;
        this.modifier = modifier;

        CalculateValue();
    }

    // Returns baseValue
    public float GetBaseValue () 
    {
        return baseValue;
    }

    // Sets baseValue equal to provided value (value)
    public void SetBaseValue (float value)
    {
        baseValue = value;
        CalculateValue();
    }

    // Returns modifier
    public float GetModifier ()
    {
        return modifier;
    }

    // Sets modifierValue equal to provided value (modifierInPercent)
    public void SetModifier (float modifierInPercent)
    {
        float modifier = modifierInPercent;
        CalculateValue();
    }

    // Calculates calcValue and returns it
    public float GetCalcValue()
    {
        CalculateValue();
        return calcValue;
    }

    // Calculates calcValue and sets it
    public void CalculateValue() 
    {
        float onePercentOfBaseValue = baseValue / 100f;         // Calculate onePercentOfBaseValue
        float amount = onePercentOfBaseValue * modifier;        // Calculate Amount by multiplying onePercentOfBaseValue with modifier, which is in percent
        calcValue = baseValue + amount;                         // set CalcValue, by adding calaculated amount to baseValue
    
        /*
        Example Calculation:
        0.05 = 5 / 100
        0.5 = 0.05 * 10
        5.5 = 5 + 0.5
        */
    
    }
}