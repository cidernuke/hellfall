using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    private float vertical;
    private float speed = 8f;
    private bool isLadder;
    [HideInInspector] public bool isClimbing;
    [HideInInspector] public bool jumpedOffOfLadder = false;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");

        if (isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            animator.SetBool("isClimbingLadder", isClimbing);
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
        }
        else
        {
            animator.SetBool("isClimbingLadder", false);
            rb.gravityScale = 1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            // print("colliding with ladder");
            isLadder = true;
        }
    }

    [SerializeField] float boostOffLadder;
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
            // Only slows velocity if there is vertical input
            if (Input.GetAxisRaw("Vertical") > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, boostOffLadder); // Slow donw upward motion, so that player doesnt fly off of ladder
            }
        }
    }

    public void JumpOffLadder()
    {
        print("entered JumpOffLadder()");
        isClimbing = false; // Exit climbing state
        rb.gravityScale = 1f; // Restore gravity for jump
        jumpedOffOfLadder = true;
    }
}