using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    [SerializeField] private readonly float speed = 2.5f;
    [SerializeField] private float attackRangeCC; // close combat

    Transform player;
    Rigidbody2D rb;
    Boss boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    /// <summary>
    /// This function initializes the player, rigidbody and boss
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// <summary>
    /// This funciton contains the logic of following the player and for when to attack him with which attacks.
    /// </summary>
    /// <param name="animator">Animator of the boss.</param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer(player);

        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

        // Boss only moves when attack range is not reached. attackRangeCC is set in the editor.
        if (Vector2.Distance(player.position, rb.position) > attackRangeCC)
        {
            animator.SetBool("isRunning", true);
            rb.MovePosition(newPos);
        }

        // Boss only attacks with blood_bullets_forward if attack range is reached.
        if (Vector2.Distance(player.position, rb.position) <= attackRangeCC)
        {
            animator.SetTrigger("attack_01");
            // animator.SetTrigger("attack_01.5");
        }

        if (boss.cooldownTimer >= boss.upwardAttackCooldown)
        {
            if (Vector2.Distance(player.position, rb.position) >= attackRangeCC && !boss.isInSecondPhase)
            {
                animator.SetTrigger("attack_02");
                boss.cooldownTimer = 0;
            }
            else if (Vector2.Distance(player.position, rb.position) >= attackRangeCC && boss.isInSecondPhase)
            {
                // boss.PrinterForBossRun("attack 2.5");
                animator.SetTrigger("attack_02.5");
                boss.cooldownTimer = 0;
            }
        }

        animator.SetBool("isRunning", false);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("attack_01");
        animator.ResetTrigger("attack_01.5");
        animator.ResetTrigger("attack_02");
        animator.ResetTrigger("attack_02.5");
    }
}
