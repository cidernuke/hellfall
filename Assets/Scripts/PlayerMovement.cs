using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles player movement, including walking, jumping, double jumping, wall jumping, wall sliding, dashing, and crouching.
/// This script should be attached to a player GameObject with a Rigidbody2D, BoxCollider2D, and SpriteRenderer component.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public IPlayerInput playerInput;

    #region Layer Masks
    // Layer masks to identify ground and wall layers for collision detection
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    private LayerMask groundAndWallLayer;
    #endregion

    #region Movement Variables
    // Components
    private float horizontal;
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    // Movement flags and variables
    public bool grounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private static float speed = 10f;  // Horizontal movement speed
    [SerializeField] private float jumpPower;   // Vertical jump force

    private int facingDirection = 1; // 1 for facing right, -1 for facing left, affects the player
    private bool isFacingRight = true;

    //Fall variables --> HandleGravity is commented out due to problems, for explanation look at HandleGravity()
    //[SerializeField] private float maxFallSpeed = 20f;
    //[SerializeField] private float fallAcceleration = 2.5f; //Acceleration during the Fall

    #endregion

    #region Jumping Variables
    // Wall jumping
    private bool isWallSliding;
    [SerializeField] private float wallSlideSpeed = 2f; // Speed at which the player slides down a wall
    [SerializeField] private Transform wallCheck;
    private int wallSide;
    private int lastWallJumped = 0; // compared to wallSide in logic --> int, not bool
    private bool canWallJump = true;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpDuration = 0.4f;  // Duration during which horizontal input is ignored after a wall jump

    // Double Jump
    private bool isDoubleJumping;

    //Coyote Time
    [SerializeField] private float coyoteTimeDuration = 0.2f;
    private float lastTimeGrounded = -1f; // Set to -1 to prevent coyote time from triggering at game start
    private bool coyoteUsable = true;
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
    [SerializeField] private Sprite standing;  // Sprite used when standing, initialized in unity inspector
    [SerializeField] private Sprite crouching; // Sprite used when crouching, initialized in unity inspector
    [SerializeField] private float crouchSpeed = speed / 2;

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
        // If no input is assigned, use the default input implementation
        if (playerInput == null)
        {
            playerInput = new PlayerInput();
        }

        InitializeComponents();
        InitializeLayers();
        InitializeCrouchVariables();
        InitializeDashVariables();
    }

    /// <summary>
    /// Called once per frame.
    /// Handles input and updates player state.
    /// </summary>
    public void Update()
    {
        horizontal = playerInput.GetHorizontalInput();
        if (isDashing)
        {
            // Skip the rest of the update while dashing
            return;
        }

        if (IsGrounded())
        {
            isDoubleJumping = false;
        }

        HandleJumpInput();
        WallSlide();
        WallJump();

        HandleDashInput();

        if (!isWallJumping)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            body.velocity = new Vector2(horizontal * speed, body.velocity.y);
            bool isWalking = horizontal != 0;
            animator.SetBool("run", isWalking);
            HandleCrouchInput();
            bool isCrouchWalking = isWalking && isCrouching;

            animator.SetBool("crouch", !isCrouchWalking && isCrouching);
            animator.SetBool("crouch_walking", isCrouchWalking);

            if (IsGrounded() && isCrouchWalking)
            {
                body.velocity = new Vector2(horizontal * crouchSpeed, body.velocity.y);
            }
        }
        //HandleGravity(); 
        //ApplyMovement();
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
        groundAndWallLayer = groundLayer | wallLayer;
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

    #region Movement Methods
    private bool IsGrounded()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundAndWallLayer);
        animator.SetBool("grounded", grounded);

        // Store the last time the player touched the ground for coyote time
        if (grounded)
        {
            lastTimeGrounded = Time.time;
            coyoteUsable = true;
        }
        else if (coyoteUsable && Time.time > lastTimeGrounded + coyoteTimeDuration)
        {
            // If Coyote Time duration is exceeded, disable its usability
            coyoteUsable = false;
        }
        return grounded;
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !grounded && horizontal != 0f)
        {
            isWallSliding = true;
            // Determine the wall side (1 for right wall, -1 for left wall)
            wallSide = transform.localScale.x > 0 ? 1 : -1;
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            wallJumpDirection = -wallSide;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));

            // Check if we are on a different wall than last jump
            if (lastWallJumped != wallSide)
            {
                // Reset wall jumping ability since we switched walls
                canWallJump = true;
                lastWallJumped = wallSide;
            }
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (playerInput.GetJumpInput() && wallJumpingCounter > 0f && canWallJump)
        {
            // TODO: make smoother, right now very janky feeling. When jumping from one wall to other, player very fast and then very slow. Hint: body.velocity.x (2f) 
            Vector2 wallJumpingPower = new Vector2(2f, 7f);
            isWallJumping = true;
            body.velocity = new Vector2(wallJumpDirection * wallJumpingPower.x, wallJumpingPower.y);
            // Disable jumping on the same wall again until we touch a new wall
            canWallJump = false;
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool CanUseCoyote()
    {
        return coyoteUsable && !grounded && Time.time < lastTimeGrounded + coyoteTimeDuration;
    }

    // The HandleGravity method is intended to control the player's falling speed 
    // by applying fall acceleration and capping it at a defined max fall speed.
    // This should ensure smoother and more controlled falling behavior.
    // It is currently commented out due to issues with other parts of the code 
    // overwriting body.velocity.y, leading to conflicts that prevent the fall speed 
    // cap from working as intended.

//     private void HandleGravity()
//     {
//         //If the player is grounded and falling (not sure if even necessary)
//         // if(grounded && body.velocity.y >= 0f)
//         // {
//         //     //Set a small downward force to keep the player grounded (not sure if even necessary)
//         //     body.velocity = new Vector2(body.velocity.x, -0.5f);
//         // }
//         // else
//         // {
//         //     //Apply fall acceleration and limit fall speed (Important for smooth falling)
//         //     body.velocity = new Vector2(body.velocity.x, Mathf.MoveTowards(body.velocity.y, -maxFallSpeed, fallAcceleration* Time.fixedDeltaTime));
//         // }

//         // if(!grounded && body.velocity.y < 0f){
//         //     body.velocity = new Vector2(body.velocity.x, Mathf.MoveTowards(body.velocity.y, -maxFallSpeed, fallAcceleration* Time.fixedDeltaTime));
//         //     Debug.Log("Fallspeed (clamped): " + body.velocity.y + " / MaxFallSpeed: " + -maxFallSpeed);
//         // }

//         //float initialGravityScale = body.gravityScale;
//         //body.gravityScale = 0;
//         if(!grounded && body.velocity.y < 0f)
//         {
//             float newFallSpeed = Mathf.MoveTowards(body.velocity.y, -maxFallSpeed, fallAcceleration * Time.fixedDeltaTime);
//             body.velocity = new Vector2(body.velocity.x, newFallSpeed);
//             Debug.Log("Fallspeed (clamped): " + body.velocity.y + " / MaxFallSpeed: " + -maxFallSpeed);
//         }
//         //body.gravityScale = initialGravityScale;
//     }

//     private void ApplyMovement()
// {
//     // Apply the current velocity values calculated in other methods to the Rigidbody2D component
//     body.velocity = new Vector2(body.velocity.x, body.velocity.y);
// }
    #endregion

    #region Input Handling Methods
    private void HandleJumpInput()
    {
        if (playerInput.GetJumpInput())
        {
            animator.SetTrigger("jump"); // Play jump animation on first jump

            //Checks
            if (IsGrounded() || CanUseCoyote())
            {
                // First jump
                // TODO: jumpPower is not initialized anywhere!
                body.velocity = new Vector2(body.velocity.x, jumpPower);
                isDoubleJumping = false; // Reset double jump for the next jump
                coyoteUsable = false;
            }
            else if (!isDoubleJumping)
            {
                // Double jump
                // TODO: jumpPower is not initialized anywhere!
                body.velocity = new Vector2(body.velocity.x, jumpPower);
                isDoubleJumping = true; // Set double jump flag to prevent further jumps
                animator.SetBool("grounded", IsGrounded());
            }
        }
    }

    /// <summary>
    /// Handles dash input and initiates the dash coroutine if possible.
    /// </summary>
    public void HandleDashInput()
    {
        float horizontalInput = playerInput.GetHorizontalInput();
        if (playerInput.GetDashInput() && canDash && horizontalInput != 0 && !isCrouching)
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
        if (playerInput.GetCrouchInput())
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
                animator.SetBool("crouch_walking", false);
            }
        }
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

    #region Input Methods
    // These methods abstract input retrieval, making it easier to modify or mock inputs for testing

    /// <summary>
    /// Retrieves horizontal input from the player.
    /// </summary>
    /// <returns>Float value between -1 and 1 representing horizontal input.</returns>
    // public virtual float GetHorizontalInput()
    // {
    //     return Input.GetAxis("Horizontal");
    // }

    /// <summary>
    /// Checks if the jump input has been pressed.
    /// </summary>
    /// <returns>True if jump input is pressed this frame.</returns>
    // public virtual bool GetJumpInput()
    // {
    //     return Input.GetKeyDown(KeyCode.Space);
    // }

    /// <summary>
    /// Checks if the dash input has been pressed.
    /// </summary>
    /// <returns>True if dash input is pressed this frame.</returns>
    // public virtual bool GetDashInput()
    // {
    //     return Input.GetKeyDown(KeyCode.LeftShift);
    // }

    /// <summary>
    /// Checks if the crouch input is being held down.
    /// </summary>
    /// <returns>True if crouch input is held down.</returns>
    // public virtual bool GetCrouchInput()
    // {
    //     return Input.GetKey(KeyCode.S);
    // }
    #endregion
}
