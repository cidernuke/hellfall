using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float attackRange = 3f;

    Transform player;
    Rigidbody2D rb;
    Boss boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();

        // animator.SetBool("isRunning", true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer(player);
        
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

        // Boss only moves when attack range is not reached
        if (Vector2.Distance(player.position, rb.position) > attackRange)
        {
            animator.SetBool("isRunning", true);
            rb.MovePosition(newPos);
        }

        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("attack_01");
            animator.SetBool("isRunning", false);
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("attack_01");
    }
}
