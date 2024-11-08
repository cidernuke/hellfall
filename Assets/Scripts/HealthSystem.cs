using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    
    [SerializeField] public float startingHealth;
    public float currentHealth;
    
    //zum Awake wird current = starting gesetzt
    private void Awake()
    {
        currentHealth = startingHealth;
    }   

    /// <summary>
    /// Reduces the current health by the specified damage amount, updates the health UI, 
    /// and checks if the health has dropped to zero or below.
    /// </summary>
    /// <param name="damage">The amount of damage to take.</param>
    public void TakeDamage(float damage)
    {

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        Debug.Log("Current Health: " + currentHealth + "/ Starting Health: " + startingHealth);
        //UIHandler.instance.SetHealthValue(currentHealth / (float)startingHealth);
        if (currentHealth <= 0)
        {
            //Die();
            Debug.Log("Player died");
        }
    }
}
