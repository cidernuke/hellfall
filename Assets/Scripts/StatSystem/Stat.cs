using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    [SerializeField] private float baseValue;
    [SerializeField] private float modifier = 0f;

    private float calcValue = 0f;


    public Stat (float baseValue, float modifier = 0)
    {
        this.baseValue = baseValue;
        this.modifier = modifier;

        CalculateValue();
    }

    public float GetBaseValue () 
    {
        return baseValue;
    }

    public void SetBaseValue (float value)
    {
        baseValue = value;
        CalculateValue();
    }

    public float GetModifier ()
    {
        return modifier;
    }

    public void SetModifier (float modifierInPercent)
    {
        float modifier = modifierInPercent;
        CalculateValue();
    }

    public void CalculateValue() 
    {
        float amount = baseValue / 100f;
        amount = amount * modifier;
        calcValue = baseValue + modifier;
    }


}