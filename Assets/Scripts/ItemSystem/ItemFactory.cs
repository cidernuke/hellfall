using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using ItemSystem.Abstract;
using ItemSystem.Items;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{

    [SerializeField] private static ItemData itemData;
    public Item healthItem = new HealthItem(itemData);

    /*
        TODO: Hier werden die Items erstellt.

    */

    

    
}
