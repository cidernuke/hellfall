using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    [SerializeField] private float baseValue;
    [SerializeField] private float modifier = 0f;


    public Stat (float baseValue, float modifier = 0)
    {
        this.baseValue = baseValue;
        this.modifier = modifier;
    }

    public float GetBaseValue () 
    {
        return baseValue;
    }

    public void SetBaseValue (float value)
    {
        baseValue = value;
    }

    public float GetModifier ()
    {
        return modifier;
    }

    // public void SetModifier (int modifierInPercent)
    // {
    //     float amount = (float) baseValue / 100f;
    //     amount = amount * modifierInPercent;
    //     modifier = amount;
    // }



}