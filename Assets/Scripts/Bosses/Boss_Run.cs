using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    [SerializeField] private readonly float speed = 2.5f;
    [SerializeField] private readonly float attackRangeCC = 3f; // close combat

    Transform player;
    Rigidbody2D rb;
    Boss boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //* Attack one should maybe only activate if player is further away, otherwise different attack?
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer(player);

        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

        // Boss only moves when attack range is not reached
        if (Vector2.Distance(player.position, rb.position) > attackRangeCC)
        {
            animator.SetBool("isRunning", true);
            rb.MovePosition(newPos);
        }

        if (Vector2.Distance(player.position, rb.position) <= attackRangeCC)
        {
            animator.SetTrigger("attack_01");
        }

        //! problem with cooldown, not really working
        // if (boss.cooldownTimer < boss.upwardAttackCooldown)
        // {
        //     boss.PrinterForBossRun("still cooling down");
        //     return;
        // }
        if (boss.cooldownTimer >= boss.upwardAttackCooldown)
        {
            if (Vector2.Distance(player.position, rb.position) >= attackRangeCC && !boss.isInSecondPhase)
            {
                // boss.PrinterForBossRun("cooldown reached: "+boss.cooldownTimer);
                // animator.SetTrigger("attack_02");
                animator.SetTrigger("attack_02.5");
                boss.cooldownTimer = 0;
            }
            // else if (Vector2.Distance(player.position, rb.position) >= attackRangeCC && boss.isInSecondPhase)
            // {
            //     boss.PrinterForBossRun("attack 2.5");
            //     animator.SetTrigger("attack_02.5");
            // }
        }
        // if (Vector2.Distance(player.position, rb.position) >= attackRangeCC && !boss.isInSecondPhase)
        // {
        //     animator.SetTrigger("attack_02");
        //     boss.cooldownTimer = 0;
        // }
        // else if (Vector2.Distance(player.position, rb.position) >= attackRangeCC && boss.isInSecondPhase)
        // {
        //     animator.SetTrigger("attack_02.5");
        // }

        animator.SetBool("isRunning", false);
    }

    


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("attack_01");
        animator.ResetTrigger("attack_02");
        animator.ResetTrigger("attack_02.5");
    }
}
