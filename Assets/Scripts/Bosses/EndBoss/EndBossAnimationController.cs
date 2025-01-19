using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBossAnimationController : StateMachineBehaviour
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float attackRangeCC; // close combat
    [SerializeField] private float attackRangeChargedAttack; // close combat

    Transform player;
    Rigidbody2D rb;
    EndBossMain boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<EndBossMain>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer(player);

        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

        // Boss only moves when attack range is not reached. attackRangeCC is set in the editor.
        // if (Vector2.Distance(player.position, rb.position) > attackRangeCC)
        // {
        //     // animator.SetBool("isRunning", true);
        // }
        if (!boss.isCharging)
        {
            rb.MovePosition(newPos);
        }

        // Boss only attacks with melee if attack range is reached.
        if (Vector2.Distance(player.position, rb.position) <= attackRangeCC)
        {
            // boss.PrinterForBossRun("should be attacking");
            animator.SetTrigger("movingAttack");
        }

        if (boss.cooldownTimer >= boss.chargeAttackCooldown)
        {
            // boss.PrinterForBossRun("cooldown over, attackRangeChargedAttack: "+attackRangeChargedAttack);
            if (Vector2.Distance(player.position, rb.position) >= attackRangeChargedAttack)
            {
                // Vector2 newPosInverse = Vector2.MoveTowards(rb.position, -target, 3f * Time.fixedDeltaTime);
                // rb.MovePosition(newPosInverse);
                // animator.SetTrigger("chargedAttack");
                animator.SetTrigger("initiateVanishing_01");
                // boss.PrinterForBossRun("setting trigger, setting isAreaAttack to true");
                boss.isAreaAttack = true;
                boss.cooldownTimer = 0;
            }
        }

        // animator.SetBool("isRunning", false);

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("movingAttack");
        animator.ResetTrigger("chargedAttack");
        animator.ResetTrigger("initiateVanishing_01");
        // animator.ResetTrigger("initiateVanishing_03");
    }

}
