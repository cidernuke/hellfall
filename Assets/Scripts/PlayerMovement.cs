using UnityEngine;
using System.Collections;

/// <summary>
/// Handles player movement, including walking, jumping, double jumping, wall jumping, wall sliding, dashing, and crouching.
/// This script should be attached to a player GameObject with a Rigidbody2D, BoxCollider2D, and SpriteRenderer component.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    #region Layer Masks
    // Layer masks to identify ground and wall layers for collision detection
    private LayerMask wallLayer;
    private LayerMask groundLayer;
    #endregion

    #region Movement Variables
    // Components
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    // Movement flags and variables
    public bool grounded;
    [SerializeField] private float speed;       // Horizontal movement speed
    [SerializeField] private float jumpPower;   // Vertical jump force

    private int facingDirection = 1; // 1 for facing right, -1 for facing left
    #endregion

    #region Jumping Variables
    // Multiple jumps (e.g., double jump)
    [SerializeField] private int possibleJumps = 2; // Total number of jumps allowed before landing
    private int jumpCounter;                        // Tracks remaining jumps

    // Wall jumping
    [SerializeField] private float wallSlideSpeed = 2f; // Speed at which the player slides down a wall
    [SerializeField] private float wallJumpX = 10f;     // Horizontal force applied during a wall jump

    // Wall colliders
    private Collider2D wallColliderLeft = null;
    private Collider2D wallColliderRight = null;
    private Collider2D lastWallJumpedFrom = null;

    private bool isWallJumping = false;
    private float wallJumpDuration = 0.2f;  // Duration during which horizontal input is ignored after a wall jump
    private float wallJumpStartTime;         // Time when the wall jump started
    #endregion

    #region Dash Variables
    // Dashing mechanics
    [SerializeField] private float dashSpeed = 30f;      // Speed during dash
    [SerializeField] private float dashDuration = 0.2f;  // Duration of the dash
    [SerializeField] private float dashCooldown = 1f;    // Cooldown time before dash can be used again

    private bool isDashing;
    private bool canDash = true;
    private float dashDirection;
    #endregion

    #region Crouch Variables
    // Crouching mechanics
    [SerializeField] private Sprite standing;  // Sprite used when standing
    [SerializeField] private Sprite crouching; // Sprite used when crouching

    private Vector2 standingSize;    // Collider size when standing
    private Vector2 crouchingSize;   // Collider size when crouching
    private Vector2 standingOffset;  // Collider offset when standing
    private Vector2 crouchingOffset; // Collider offset when crouching
    public bool isCrouching = false;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Called when the script instance is being loaded.
    /// Used for initialization.
    /// </summary>
    public void Awake()
    {
        InitializeComponents();
        InitializeLayers();
        InitializeCrouchVariables();
        InitializeDashVariables();
        jumpCounter = possibleJumps;
    }

    /// <summary>
    /// Called once per frame.
    /// Handles input and updates player state.
    /// </summary>
    public void Update()
    {
        if (isDashing)
        {
            // Skip the rest of the update while dashing
            return;
        }

        CheckWallTouch();

        HandleMovementInput();
        HandleJumpInput();
        HandleDashInput();
        HandleCrouchInput();
    }
    #endregion

    #region Initialization Methods
    /// <summary>
    /// Initializes and validates required components.
    /// </summary>
    private void InitializeComponents()
    {
        // Get and check required components
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get reference to the Animator component
        animator = GetComponent<Animator>();

        if (body == null)
            Debug.LogError("Rigidbody2D component not found!");
        if (boxCollider == null)
            Debug.LogError("BoxCollider2D component not found!");
        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer not found!");
        if (animator == null)
            Debug.LogError("Animator not found!");
    }

    /// <summary>
    /// Initializes layer masks for ground and wall detection.
    /// </summary>
    private void InitializeLayers()
    {
        wallLayer = LayerMask.GetMask("Wall");
        groundLayer = LayerMask.GetMask("Ground");
    }

    /// <summary>
    /// Initializes variables related to crouching mechanics.
    /// </summary>
    private void InitializeCrouchVariables()
    {
        // Get default size and offset from the existing collider
        standingSize = boxCollider.size;
        standingOffset = boxCollider.offset;

        // Calculate crouch size and offset
        float crouchHeight = standingSize.y * 0.5f; // Crouching reduces height by half
        float sizeDifference = standingSize.y - crouchHeight;

        crouchingSize = new Vector2(standingSize.x, crouchHeight);
        // Adjust offset so the bottom of the collider remains the same
        crouchingOffset = new Vector2(standingOffset.x, standingOffset.y - sizeDifference / 2f);

        // Set initial sprite and collider size
        spriteRenderer.sprite = standing;
        boxCollider.size = standingSize;
        boxCollider.offset = standingOffset;
    }

    /// <summary>
    /// Initializes variables related to dashing mechanics.
    /// </summary>
    private void InitializeDashVariables()
    {
        isDashing = false;
        canDash = true;
    }
    #endregion

    #region Input Handling Methods
    /// <summary>
    /// Handles horizontal movement input and updates the player's velocity accordingly.
    /// </summary>
    public void HandleMovementInput()
    {
        float horizontalInput = GetHorizontalInput();
        // Update the animator's parameters
        bool isWalking = horizontalInput != 0;
        bool isCrouchWalking = isCrouching && isWalking;

        // Adjust speed if crouching
        float currentSpeed = speed;

        if (isCrouching)
        {
            currentSpeed *= 0.5f; // Half speed when crouching
            animator.SetBool("crouch_walking", isCrouchWalking);
            animator.SetBool("crouch", isCrouching && !isWalking);
        }

        if (isWallJumping)
        {
            // Prevent horizontal movement input from overriding the wall jump
            if (Time.time > wallJumpStartTime + wallJumpDuration)
            {
                isWallJumping = false;
            }
        }
        else if ((wallColliderLeft != null || wallColliderRight != null) && !grounded && body.velocity.y < 0)
        {
            // Player is wall sliding
            body.velocity = new Vector2(body.velocity.x, -wallSlideSpeed);

            // Allow player to move away from the wall
            if ((wallColliderLeft != null && horizontalInput > 0) || (wallColliderRight != null && horizontalInput < 0))
            {
                // Player moves away from the wall
                body.velocity = new Vector2(horizontalInput * currentSpeed, body.velocity.y);

                // Flip player sprite based on input direction
                if (horizontalInput > 0.01f && facingDirection == -1)
                {
                    FlipPlayer();
                }
                else if (horizontalInput < -0.01f && facingDirection == 1)
                {
                    FlipPlayer();
                }
            }
        }
        else
        {
            // Normal horizontal movement
            body.velocity = new Vector2(horizontalInput * currentSpeed, body.velocity.y);

            // Flip player sprite based on input direction
            if (horizontalInput > 0.01f && facingDirection == -1)
            {
                FlipPlayer();
            }
            else if (horizontalInput < -0.01f && facingDirection == 1)
            {
                FlipPlayer();
            }
        }

        // Update the animator's parameters
        animator.SetBool("run", horizontalInput != 0);
    }

    /// <summary>
    /// Handles jump input and performs regular jumps, double jumps, and wall jumps.
    /// </summary>
    public void HandleJumpInput()
    {
        if (GetJumpInput() && !isCrouching)
        {
            if (grounded)
            {
                // Perform a regular jump
                body.velocity = new Vector2(body.velocity.x, jumpPower);
                animator.SetTrigger("jump");
                grounded = false;
                jumpCounter = possibleJumps - 1; // Decrease jump counter
                isWallJumping = false;
                lastWallJumpedFrom = null;
            }
            else if ((wallColliderLeft != null || wallColliderRight != null) && !isWallJumping)
            {
                // Determine current wall collider
                Collider2D currentWall = wallColliderLeft != null ? wallColliderLeft : wallColliderRight;

                // Check if the current wall is different from the last wall jumped from
                if (currentWall != null && currentWall != lastWallJumpedFrom)
                {
                    // Perform a wall jump
                    WallJump(currentWall);
                    animator.SetTrigger("jump");
                }
            }
            else if (jumpCounter > 0)
            {
                // Perform a double jump
                body.velocity = new Vector2(body.velocity.x, jumpPower);
                jumpCounter--;
                animator.SetTrigger("jump");
            }
        }
        animator.SetBool("grounded", grounded);
    }

    /// <summary>
    /// Handles dash input and initiates the dash coroutine if possible.
    /// </summary>
    public void HandleDashInput()
    {
        float horizontalInput = GetHorizontalInput();
        if (GetDashInput() && canDash && horizontalInput != 0 && !isCrouching)
        {
            StartDash(horizontalInput);
            animator.SetTrigger("dash");
        }
    }

    /// <summary>
    /// Handles crouch input and updates the player's sprite and collider accordingly.
    /// </summary>
    public void HandleCrouchInput()
    {
        float horizontalInput = GetHorizontalInput();

        if (GetCrouchInput())
        {
            if (!isCrouching)
            {
                // Enter crouching state
                spriteRenderer.sprite = crouching;
                boxCollider.size = crouchingSize;
                boxCollider.offset = crouchingOffset;
                isCrouching = true;
                animator.SetBool("crouch", true);

            }
        }
        else
        {
            if (isCrouching)
            {
                // Exit crouching state
                spriteRenderer.sprite = standing;
                boxCollider.size = standingSize;
                boxCollider.offset = standingOffset;
                isCrouching = false;
                animator.SetBool("crouch", false);

            }
        }        


    }
    #endregion

    #region Collision Methods
    /// <summary>
    /// Called when the player collides with another collider.
    /// Used to detect when the player lands on the ground.
    /// </summary>
    /// <param name="collision">Collision data.</param>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        // Check if the collision is with the ground layer
        if ((groundLayer.value & (1 << collisionLayer)) != 0)
        {
            // Player has landed on the ground
            grounded = true;
            jumpCounter = possibleJumps; // Reset jump counter
            isWallJumping = false;
            lastWallJumpedFrom = null;
        }
    }

    /// <summary>
    /// Called when the player stops colliding with another collider.
    /// Used to detect when the player leaves the ground.
    /// </summary>
    /// <param name="collision">Collision data.</param>
    public void OnCollisionExit2D(Collision2D collision)
    {
        int collisionLayer = collision.gameObject.layer;

        // Check if the collision was with the ground layer
        if ((groundLayer.value & (1 << collisionLayer)) != 0)
        {
            // Player has left the ground
            grounded = false;
        }
        // Note: We do not reset lastWallJumpedFrom when leaving the wall to prevent infinite wall jumps
    }

    public bool canAttack()
    {
        float horizontalInput = GetHorizontalInput();

        return horizontalInput == 0 && grounded;
    }
    #endregion

    #region Dash Coroutine
    /// <summary>
    /// Initiates the dash action in the specified direction.
    /// </summary>
    /// <param name="direction">Direction of the dash (-1 for left, 1 for right).</param>
    public void StartDash(float direction)
    {
        dashDirection = Mathf.Sign(direction);
        StartCoroutine(DashCoroutine());
    }

    /// <summary>
    /// Coroutine that handles the dash movement, temporarily disables gravity, and enforces cooldown.
    /// </summary>
    public IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;

        // Disable gravity during the dash for consistent movement
        float originalGravity = body.gravityScale;
        body.gravityScale = 0;

        float dashEndTime = Time.time + dashDuration;

        while (Time.time < dashEndTime)
        {
            // Move the player in the dash direction
            body.velocity = new Vector2(dashDirection * dashSpeed, 0);
            yield return null; // Wait for the next frame
        }

        isDashing = false;
        body.gravityScale = originalGravity; // Restore original gravity

        // Wait for dash cooldown before allowing another dash
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
    #endregion

    #region Wall Methods
    /// <summary>
    /// Checks if the player is touching a wall on the left or right side.
    /// Updates wall colliders accordingly.
    /// </summary>
    private void CheckWallTouch()
    {
        // Cast a box collider to the left to detect walls
        RaycastHit2D raycastHitLeft = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.left,
            0.1f,
            wallLayer);

        // Cast a box collider to the right to detect walls
        RaycastHit2D raycastHitRight = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.right,
            0.1f,
            wallLayer);

        // Update wall colliders based on collision results
        wallColliderLeft = raycastHitLeft.collider;
        wallColliderRight = raycastHitRight.collider;
    }

    /// <summary>
    /// Performs a wall jump by applying a force away from the wall and flipping the player's direction.
    /// </summary>
    /// <param name="currentWall">The wall collider from which the player is jumping.</param>
    private void WallJump(Collider2D currentWall)
    {
        // Apply a force away from the wall
        float horizontalForce = -facingDirection * wallJumpX;
        Vector2 force = new Vector2(horizontalForce, jumpPower);
        body.velocity = Vector2.zero; // Reset current velocity
        body.AddForce(force, ForceMode2D.Impulse);

        // Immediately flip the player's facing direction
        //FlipPlayer();

        grounded = false;
        isWallJumping = true;
        wallJumpStartTime = Time.time;

        lastWallJumpedFrom = currentWall; // Store the wall we just jumped from

        // Reset jump counter after wall jump to allow double jump
        jumpCounter = possibleJumps - 1;
    }

    /// <summary>
    /// Flips the player's facing direction and updates the sprite accordingly.
    /// </summary>
    private void FlipPlayer()
    {
        // Invert the facing direction
        facingDirection *= -1;

        // Flip the player's sprite by inverting the x scale
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    #endregion

    #region Input Methods
    // These methods abstract input retrieval, making it easier to modify or mock inputs for testing

    /// <summary>
    /// Retrieves horizontal input from the player.
    /// </summary>
    /// <returns>Float value between -1 and 1 representing horizontal input.</returns>
    public virtual float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// Checks if the jump input has been pressed.
    /// </summary>
    /// <returns>True if jump input is pressed this frame.</returns>
    public virtual bool GetJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    /// <summary>
    /// Checks if the dash input has been pressed.
    /// </summary>
    /// <returns>True if dash input is pressed this frame.</returns>
    public virtual bool GetDashInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    /// <summary>
    /// Checks if the crouch input is being held down.
    /// </summary>
    /// <returns>True if crouch input is held down.</returns>
    public virtual bool GetCrouchInput()
    {
        return Input.GetKey(KeyCode.S);
    }
    #endregion
}
