using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat 
{
    [SerializeField] private int modifier = 0;
    [SerializeField] private float baseValue;

    public float GetValue () 
    {
        return baseValue + modifier;
    }

    // public void SetModifier (int modifierInPercent)
    // {
    //     float amount = (float) baseValue / 100f;
    //     amount = amount * modifierInPercent;
    //     modifier = amount;
    // }

    public float GetModifier ()
    {
        return modifier;
    }





}