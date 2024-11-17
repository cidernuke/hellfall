using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifierItem : IItem
{
    float duration { get; set; }
    int amount { get; set; }
    String description { get; set; }

    public void OnTriggerEnter2D(Collider2D other)
    {
    }

    // TODO: has parameter of Type StatSystem ( StatSystem stats )  
    public void modifyStats()
    {
    }
}
