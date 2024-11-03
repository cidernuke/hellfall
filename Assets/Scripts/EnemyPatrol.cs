using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    private Vector3 initScale;
    private bool movingLeft;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;


    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;

    private void Update()
    {
        if (movingLeft)
        {
            // Check if the enemy has reached the left edge
            if (enemy.position.x >= leftEdge.position.x)
            {
                // Move the enemy to the left
                MoveInDirection(-1);
            }
            else
            {
                DirectionChange();
            }
        }
        else
        {
            // Check if the enemy has reached the right edge
            if (enemy.position.x <= rightEdge.position.x)
            {
                // Move the enemy to the right
                MoveInDirection(1);
            }
            else
            {
                DirectionChange();
            }
        }
    }

    private void DirectionChange()
    {
        anim.SetBool("moving", false);
        
        // Increase the idle timer by the time passed since the last frame
        idleTimer += Time.deltaTime;

        // Check if the idle duration has been exceeded
        if (idleTimer > idleDuration)
        {   
            // Change the movement direction of the enemy
            movingLeft = !movingLeft;
        }
    }

    private void Awake()
    {
        initScale = enemy.localScale;
    }
    
    private void OnDisable()
    {
        anim.SetBool("moving", false);
    }
    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        anim.SetBool("moving", true);
        //Make enemy face direction
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction, initScale.y, initScale.z);

        //Move in that direction
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed, enemy.position.y, enemy.position.z);

    }
}
