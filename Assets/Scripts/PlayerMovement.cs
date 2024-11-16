using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Handles player movement, including walking, jumping, double jumping, wall jumping, wall sliding, dashing, and crouching.
/// This script should be attached to a player GameObject with a Rigidbody2D, BoxCollider2D, and SpriteRenderer component.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    #region Layer Masks
    // Layer masks to identify ground and wall layers for collision detection
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
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
    public bool platformed;
    private bool isDropping = false; // flag for dropping through platform
    [SerializeField] private float dropDuration = 0.5f; // Duration of which collision with platform is ignored
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform flipPivotPoint;
    [SerializeField] private float groundSpeed = 2.3f;  // Horizontal movement speed
    [SerializeField] private float jumpPower;   // Vertical jump force
    [SerializeField] private float jumpBufferTime = 0.1f; //Duration of the jump buffer in seconds
    private float lastTimeJumpPressed = -1f;

    private int facingDirection = 1; // 1 for facing right, -1 for facing left, affects the player
    private bool isFacingRight = true;

    //Fall variables --> HandleGravity is commented out due to problems, for explanation look at HandleGravity()
    [SerializeField] private float maxFallSpeed = -20f;
    //[SerializeField] private float fallAcceleration = 2.5f; //Acceleration during the Fall

    #endregion

    #region Jumping Variables
    // Wall jumping
    private bool isWallSliding;
    [SerializeField] private float wallSlideSpeed = 2f; // Speed at which the player slides down a wall
    [SerializeField] private Transform wallCheck;
    private int wallSide;
    private int lastWallJumped = 0; // compared to wallSide in logic --> int, not bool
    private bool canWallJump = false;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    [SerializeField] private float wallJumpDuration = 0.09f;  // Duration during which horizontal input is ignored after a wall jump

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
    // [SerializeField] private float crouchSpeed = groundSpeed / 2;
    [SerializeField] private float crouchSpeed = 1.5f;

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
    }

    /// <summary>
    /// Called once per frame.
    /// Handles input and updates player state.
    /// </summary>        // Normal horizontal movement speed
    private bool isFalling = false;
    [Range(0f, 1f)]
    [SerializeField] private float groundDecay;
    public void Update()
    {
        horizontal = GetHorizontalInput();

        if (isDashing)
        {
            // Skip the rest of the update while dashing
            return;
        }

        if (IsGrounded())
        {
            isDoubleJumping = false;
        }

        IsOnPlatform(); // until a better solution for its placement is found, it stays here!


        //Detect last time jump pressed -> jump buffering
        if (GetJumpInput())
        {
            lastTimeJumpPressed = Time.time;
        }

        HandleJumpInput();
        // WallSlide(); // moved into WallJump for performance.
        WallJump();
        // HandleDropThroughPlatform();

        HandleDashInput();

        // TODO: problem lies here, and walljump logic.
        if (!isWallJumping)
        {
            print("normal flip");
            Flip();
        }
    }

    private void HandleDropThroughPlatform()
    {
        if (GetDropDownInput() && IsOnPlatform() && !isDropping)
        {
            Collider2D platformCollider = GetPlatformColliderBelow();
            if (platformCollider != null)
            {
                StartCoroutine(DropThroughPlatformCoroutine(platformCollider));
            }
        }
    }

    [SerializeField] private float fallMultiplier = 2.5f;
    // [SerializeField] private float lowJumpMultiplier = 2f;
    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            // body.velocity = new Vector2(horizontal * speed, body.velocity.y);
            if (!isDashing)
            {
                body.velocity = new Vector2(Input.GetAxis("Horizontal") * groundSpeed, body.velocity.y);
            }
            else return;
            bool isWalking = GetHorizontalInput() != 0;
            animator.SetBool("run", isWalking);
            HandleCrouchInput();
            bool isCrouchWalking = isWalking && isCrouching;

            animator.SetBool("crouch", !isCrouchWalking && isCrouching);
            animator.SetBool("crouch_walking", isCrouchWalking);

            if (IsGrounded() && isCrouchWalking)
            {
                print("shouldnt be here");
                body.velocity = new Vector2(horizontal * crouchSpeed, body.velocity.y);
            }

        }
        if (IsGrounded() && GetHorizontalInput() == 0)
        {
            body.velocity *= groundDecay;
        }
        // !Diego, hier vielleicht für orientierung für clamped fall speed.
        // if (body.velocity.y <= 0.2 && !isFalling && !IsGrounded())
        // {
        //     print("DAMMIT");
        //     body.gravityScale *= fallGravityScale;
        //     // body.AddForce(Vector2.down * 15f);
        //     isFalling = true; // Set falling state
        // }
        // else if (body.velocity.y > 0 && isFalling)
        // {
        //     // Reset gravity when jumping up again
        //     print("being reset");
        //     body.gravityScale = 1f;
        //     isFalling = false;
        // }

        if (body.velocity.y < 0 && !isDashing)
        {
            // print("entered gravity place");
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        // Clamped fall speed
        if (body.velocity.y < maxFallSpeed)
        {
            print("fallspeed: " + body.velocity.y + "maxFallSpeed: " + maxFallSpeed);
            body.velocity = new Vector2(body.velocity.x, maxFallSpeed);
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
        platformLayer = LayerMask.GetMask("Platform");
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

    // Used to reset walljump logic, specifically when walljumping once and then landing on the ground.
    private void OnLanding() // Call this when the player lands on the ground
    {
        canWallJump = true;
        lastWallJumped = 0; // Reset the last wall
    }

    private bool IsGrounded()
    {
        // grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundAndWallLayer);
        grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer | platformLayer);
        animator.SetBool("grounded", grounded);

        // Store the last time the player touched the ground for coyote time
        if (grounded)
        {
            OnLanding(); // called here to reset wall jump logic once player lands back on ground --> player can walljump from same wall once grounded after wall jump.
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
        WallSlide();
        if (isWallSliding)
        {
            wallJumpDirection = -wallSide;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));

            // Check if we are on a different wall than last jump
            // print("wallSide: " + wallSide + ", lastWallJumped: " + lastWallJumped);
            if (lastWallJumped != wallSide)
            {
                // Reset wall jumping ability since we switched walls
                // print("wall jump resetted");
                canWallJump = true;
                lastWallJumped = wallSide;
            }
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (GetJumpInput() && wallJumpingCounter > 0f && canWallJump)
        {
            isWallJumping = true;
            body.velocity = new Vector2(wallJumpDirection * 3, 6);

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
        print("invoking");
        isWallJumping = false;
    }

    private void Flip()
    {
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        // if (isFacingRight && horizontal < 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            //? Offset adjustment, currently commented because its very noticable in the game. Fine tune or find other solution.
            // float colliderWidth = boxCollider.size.x / 2;
            // Vector3 offset = new Vector3(-localScale.x * colliderWidth, 0f, 0f);
            // transform.position -= offset;

            // spriteRenderer.flipX = false;
        }// else if (!isFacingRight && horizontal > 0f)
        // {
        //     spriteRenderer.flipX = true;
        // }

        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     GetComponent<SpriteRenderer>().flipX = true;
        //     isFacingRight = false;
        // }

        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     GetComponent<SpriteRenderer>().flipX = false;
        //     isFacingRight = true;
        // }
    }

    private bool CanUseCoyote()
    {
        if (IsGrounded())
        {
            return coyoteUsable && !grounded && Time.time < lastTimeGrounded + coyoteTimeDuration;
        }
        else return false;
    }

    private bool IsOnPlatform()
    {
        platformed = Physics2D.OverlapCircle(groundCheck.position, 0.1f, platformLayer);
        // print("platformed: " + platformed);
        return platformed;
    }

    private Collider2D GetPlatformColliderBelow()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, platformLayer);
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
        // TODO: more gravity, i think also more height --> bigger curve, less distance
        // if (GetJumpInput())
        // {
        //animator.SetTrigger("jump"); // Play jump animation on first jump

        if ((Time.time - lastTimeJumpPressed) <= jumpBufferTime)
        {
            //Checks
            if (IsGrounded() || CanUseCoyote() || IsOnPlatform())
            {
                animator.SetTrigger("jump"); // Play jump animation on first jump

                // print("in first jump");
                // First jump
                // TODO: jumpPower is not initialized anywhere!
                body.velocity = new Vector2(body.velocity.x, jumpPower);
                // body.velocity = new Vector2(body.velocity.x, body.velocity.y * groundSpeed);
                // body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                isDoubleJumping = false; // Reset double jump for the next jump
                coyoteUsable = false;

                //Reset von lastTimeJumpPressed
                lastTimeJumpPressed = -1f;
            }
            //else if
        }
        if (GetJumpInput())
        {
            if (!isDoubleJumping && !IsGrounded())
            {
                // print("in double jump");
                // TODO: jumpPower is not initialized anywhere!
                // body.velocity = new Vector2(GetHorizontalInput() * (speed * jumpHorizontalDamping), jumpPower);
                // body.velocity = new Vector2(body.velocity.x, jumpPower - (jumpPower / 3));
                body.velocity = new Vector2(body.velocity.x, jumpPower / 1.5f);
                // body.velocity = new Vector2(body.velocity.x, Input.GetAxis("Vertical") * groundSpeed);
                // body.gravityScale *= 1.5f;
                isDoubleJumping = true; // Set double jump flag to prevent further jumps
                animator.SetBool("grounded", IsGrounded());
                // animator.SetBool("platformed", IsOnPlatform());

            }
        }
        // }
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
                animator.SetBool("crouch_walking", false);
            }
        }
    }

    public bool CanAttack()
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

        // animator.SetBool("is_dashing", isDashing);
    }

    /// <summary>
    /// Coroutine that handles the dash movement, temporarily disables gravity, and enforces cooldown.
    /// </summary>
    public IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;

        animator.SetBool("is_dashing", true);
        // Disable gravity during the dash for consistent movement
        print("entered coroutine, current gravity scale should be 1: " + body.gravityScale);
        float originalGravity = body.gravityScale;
        print("coroutine, gravity after storing originalGravity, should be 1: " + originalGravity);
        body.gravityScale = 0;

        float dashEndTime = Time.time + dashDuration;
        print("coroutine, gravity scale should be 0: " + body.gravityScale);

        while (Time.time < dashEndTime)
        {
            // Move the player in the dash direction
            body.velocity = new Vector2(dashDirection * dashSpeed, 0);
            yield return null; // Wait for the next frame
        }

        isDashing = false;
        body.gravityScale = originalGravity; // Restore original gravity

        animator.SetBool("is_dashing", false);

        // Wait for dash cooldown before allowing another dash
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
    #endregion

    private IEnumerator DropThroughPlatformCoroutine(Collider2D platformCollider)
    {
        isDropping = true;
        Collider2D playerCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
        yield return new WaitForSeconds(dropDuration);
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
        isDropping = false;
    }


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

    public virtual bool GetDropDownInput()
    {
        // Wir verwenden GetKeyDown, um sicherzustellen, dass die Aktion nur einmal pro Tastendruck ausgeführt wird
        return Input.GetKeyDown(KeyCode.F);
    }
    #endregion
}