using UnityEngine;

public class EndBossAnimationController : StateMachineBehaviour
{
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float attackRangeCC; // close combat
    [SerializeField] private float attackRangeChargedAttack; // close combat

    Transform player;
    PlayerMovement playerMovement;
    Rigidbody2D rb;
    EndBossMain boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<EndBossMain>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer(player);

        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);

        if (!boss.isCharging)
        {
            rb.MovePosition(newPos);
        }

        // Boss only attacks with melee if attack range is reached.
        if (Vector2.Distance(player.position, rb.position) <= attackRangeCC)
        {
            animator.SetTrigger("movingAttack");
        }


        if (boss.chargeAttackcooldownTimer >= boss.chargeAttackCooldown)
        {
            if (Vector2.Distance(player.position, rb.position) >= attackRangeChargedAttack)
            {
                animator.SetTrigger("chargedAttack");
                boss.chargeAttackcooldownTimer = 0;
            }
        }

        if (boss.vanishAttackcooldownTimer >= boss.vanishAreaAttackCooldown)
        {
            if (Vector2.Distance(player.position, rb.position) <= 5f && playerMovement.IsDodgingALot)
            {
                boss.PrinterForBossRun("triggered charged attack");
                animator.SetTrigger("initiateVanishing_01");
                boss.isAreaAttack = true;
                boss.vanishAttackcooldownTimer = 0;
                playerMovement.IsDodgingALot = false;
            }
        }
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
