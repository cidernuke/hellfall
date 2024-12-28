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
    [SerializeField] private LayerMask platformLayer;
    #endregion

    #region Checkpoint
    private int lastCheckpointID;
    private Vector3 respawnPosition;
    public bool isDead = false;
    #endregion

    #region Movement Variables
    // Components
    private float horizontal;
    public Rigidbody2D body;
    public Animator animator;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private new Transform transform;

    // Movement flags and variables
    private bool grounded;
    public bool platformed;
    private bool isDropping = false; // flag for dropping through platform
    [SerializeField] private float dropDuration = 0.5f; // Duration of which collision with platform is ignored
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private float groundSpeed = 2.3f;  // Horizontal movement speed
    [SerializeField] private float jumpPower;   // Vertical jump force
    [SerializeField] private float jumpBufferTime = 0.1f; //Duration of the jump buffer in seconds
    private float lastTimeJumpPressed = -1f;

    private int facingDirection = 1; // 1 for facing right, -1 for facing left, affects the player
    public bool isFacingRight = true;
    private LadderMovement ladderMovement;

    // Fall variables
    [SerializeField] private float maxFallSpeed = -20f;

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
    private bool isFalling = false;

    // Double Jump
    private bool isDoubleJumping;

    // Coyote Time
    [SerializeField] private float coyoteTimeDuration = 0.2f;
    private float lastTimeGrounded = -1f; // Set to -1 to prevent coyote time from triggering at game start
    private bool coyoteUsable = true;
    #endregion

    #region Dash Variables
    // Dashing mechanics
    [SerializeField] private float dashSpeed = 30f; // Speed during dash
    [SerializeField] private float dashDuration = 0.2f; // Duration of the dash
    [SerializeField] private float dashCooldown = 1f; // Cooldown time before dash can be used again

    private bool isDashing = false;
    private bool canDash = true;
    private float dashDirection;
    #endregion

    #region Crouch Variables
    // Crouching mechanics
    [SerializeField] private Sprite standing; // Sprite used when standing, initialized in unity inspector
    [SerializeField] private Sprite crouching; // Sprite used when crouching, initialized in unity inspector
    [SerializeField] private float crouchSpeed = 1.5f;

    private Vector2 standingSize; // Collider size when standing
    private Vector2 crouchingSize; // Collider size when crouching
    private Vector2 standingOffset; // Collider offset when standing
    private Vector2 crouchingOffset; // Collider offset when crouching
    public bool isCrouching = false;
    #endregion

    #region Blocking variables
    [SerializeField] public bool blockWalk = false;     // blocks Player from walking, if true
    [SerializeField] public bool blockJump = false;     // blocks Player from jumping, if true
    [SerializeField] public bool blockCrouch = false;   // blocks Player from crouching, if true
    [SerializeField] public bool blockDash = false;     // blocks Player from dashing, if true
    [SerializeField] public bool blockInput = false;    // blocks UserInput, if true
    #endregion

    #region Particle Animator
    public GameObject playerFX;
    public Animator playerFXAnimator;
    public GameObject wallDust;
    public ParticleSystem wallDustParticleSystem;
    public GameObject floorDust;
    public ParticleSystem floorDustParticleSystem;

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

        // Initialise the respawnPosition to the Start Position of the Player
        respawnPosition = transform.position;
    }

    /// <summary>
    /// Called once per frame.
    /// Handles input and updates player state.
    /// </summary>        
    public void Update()
    {
        if (!blockInput)
        {
            horizontal = playerInput.GetHorizontalInput();
        }

        ResetAnimation();

        if (isDashing)
        {
            // Skip the rest of the update while dashing
            return;
        }

        if (IsGrounded())
        {
            isDoubleJumping = false;
        }

        //Detect last time jump pressed -> jump buffering
        if (playerInput.GetJumpInput())
        {
            lastTimeJumpPressed = Time.time;
        }

        if (!blockJump) { HandleJumpInput(); }

        if (!blockDash) { HandleDashInput(); }

        WallSlide();
        // WallSlide(); --> moved into WallJump for performance.
        if (!blockJump) { WallJump(); }

        // Necessary for jump logic not to break.
        if (ladderMovement.jumpedOffOfLadder)
        {
            ladderMovement.jumpedOffOfLadder = false;
        }

        if (!isWallJumping && !isWallSliding)
        {
            Flip();
        }
    }

    [SerializeField] private float fallMultiplier = 2.5f;
    [Range(0f, 1f)]
    [SerializeField] private float groundDecay;
    private void FixedUpdate()
    {
        if (!blockInput)
        {
            if (!isWallJumping)
            {
                if (!isDashing)
                {
                    body.velocity = new Vector2(playerInput.GetHorizontalInput() * groundSpeed, body.velocity.y);
                }
                else return;
                bool isWalking = playerInput.GetHorizontalInput() != 0;
                animator.SetBool("run", isWalking);


                if (!blockCrouch) { HandleCrouchInput(); }
                bool isCrouchWalking = isWalking && isCrouching;

                animator.SetBool("crouch", !isCrouchWalking && isCrouching);
                animator.SetBool("crouch_walking", isCrouchWalking);

                if (IsGrounded() && isCrouchWalking)
                {
                    body.velocity = new Vector2(horizontal * crouchSpeed, body.velocity.y);
                }

                if (body.velocity.y < 0 && !isFalling && !IsGrounded())
                {
                    isFalling = true; // Set falling state
                    animator.SetBool("is_falling", isFalling);
                }
            }
        }

        // Ensures that traction of the ground properly stops player when no movement input is detected. Only active when player is not moving and is grounded.
        if (IsGrounded() && playerInput.GetHorizontalInput() == 0)
        {
            body.velocity *= groundDecay;
        }

        // Sort of like gravity. Accelerates player fall speed when apex is reached.
        if (body.velocity.y < 0 && !isDashing)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        // Clamped fall speed
        if (body.velocity.y < maxFallSpeed)
        {
            body.velocity = new Vector2(body.velocity.x, maxFallSpeed);
        }
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
        transform = GetComponent<Transform>();
        // Get reference to the Animator component
        animator = GetComponent<Animator>();

        // Get Player Effects Object
        playerFX = GameObject.Find("Player_Effects");
        // Get Player Effects Animator
        playerFXAnimator = playerFX.GetComponent<Animator>();
        // Get Wall Dust Game Object
        wallDust = GameObject.Find("Wall_Dust");
        wallDustParticleSystem = wallDust.GetComponent<ParticleSystem>();
        // Get Floor Dust Game Object
        floorDust = GameObject.Find("Floor_Dust");
        floorDustParticleSystem = floorDust.GetComponent<ParticleSystem>();


        if (body == null)
            Debug.LogError("Rigidbody2D component not found!");
        if (boxCollider == null)
            Debug.LogError("BoxCollider2D component not found!");
        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer not found!");
        if (transform == null)
            Debug.LogError("Transform not found!");
        if (animator == null)
            Debug.LogError("Animator not found!");
        if (playerFX == null)
            Debug.LogError("playerFX component not found!");
        if (playerFXAnimator == null)
            Debug.LogError("playerFXAnimator component not found!");
        if (wallDust == null)
            Debug.LogError("Wall_Dust component not found!");
        if (wallDustParticleSystem == null)
            Debug.LogError("Wall_Dust Particle System component not found!");
        if (floorDust == null)
            Debug.LogError("Floor_Dust component not found!");
        if (floorDustParticleSystem == null)
            Debug.LogError("Floor_Dust Particle System component not found!");

    }

    /// <summary>
    /// Initializes layer masks for ground and wall detection.
    /// </summary>
    private void InitializeLayers()
    {
        wallLayer = LayerMask.GetMask("Wall");
        groundLayer = LayerMask.GetMask("Ground");
        platformLayer = LayerMask.GetMask("Platform");
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
    void OnLanding() // Called when the player lands on the ground
    {
        canWallJump = true;
        lastWallJumped = 0; // Reset the last wall        
    }

    private bool IsGrounded()
    {
        // Check if the player is currently grounded
        bool previouslyGrounded = grounded; // Store the previous grounded state
        grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer | platformLayer);
        animator.SetBool("grounded", grounded);

        if (grounded && IsWalled())
        {
            print("works in theory");
        }

        // Checks if player stopped climbing and is grounded. Important for animation transition
        ladderMovement = ladderMovement = GetComponent<LadderMovement>();
        // ladderMovement.jumpedOffOfLadder = false;
        if (ladderMovement.isClimbing && grounded && Mathf.Abs(Input.GetAxisRaw("Vertical")) == 0f)
        {
            ladderMovement.isClimbing = false;
        }

        if (grounded && !previouslyGrounded)
        {
            // Trigger floor dust particles when the player lands
            floorDust.transform.position = new Vector2(transform.position.x, transform.position.y - 1.4f);
            floorDustParticleSystem.Play();
        }


        // Store the last time the player touched the ground for coyote time
        if (grounded)
        {
            isFalling = false;
            animator.SetBool("is_falling", isFalling);
            wallDustParticleSystem.Stop();

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

            // Wall Dust Particle
            wallDustParticleSystem.Play();
            wallDust.transform.position = new Vector2(transform.position.x + 0.4f * wallSide, transform.position.y - 0.2f);


            if (wallJumpDirection < 0 || wallJumpDirection > 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                animator.SetBool("is_wall_sliding", isWallSliding);
            }
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
            isWallSliding = false;
            animator.SetBool("is_wall_sliding", isWallSliding);
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

        if (!blockInput)
        {
            if (playerInput.GetJumpInput() && wallJumpingCounter > 0f && canWallJump)
            {
                isWallJumping = true;
                body.velocity = new Vector2(wallJumpDirection * 3, 6);

                // Disable jumping on the same wall again until we touch a new wall
                canWallJump = false;
                wallJumpingCounter = 0f;

                // Flips player during walljump
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
        if (IsGrounded())
        {
            return coyoteUsable && !grounded && Time.time < lastTimeGrounded + coyoteTimeDuration;
        }
        else return false;
    }
    #endregion

    private bool doubleJump;
    #region Input Handling Methods
    private void HandleJumpInput()
    {
        ladderMovement = ladderMovement = GetComponent<LadderMovement>();
        if ((Time.time - lastTimeJumpPressed) <= jumpBufferTime)
        {
            // First jump
            if (IsGrounded() || CanUseCoyote() || (ladderMovement != null && ladderMovement.isClimbing))
            {
                animator.SetTrigger("jump"); // Play jump animation on first jump

                // Handles logic to jump off of a ladder
                if (ladderMovement != null && ladderMovement.isClimbing)
                {
                    ladderMovement.JumpOffLadder();
                    body.velocity = new Vector2(body.velocity.y, jumpPower * 1.2f);
                }
                else
                {
                    body.velocity = new Vector2(body.velocity.y, jumpPower);
                }

                isDoubleJumping = false; // Reset double jump for the next jump
                coyoteUsable = false;

                // Reset von lastTimeJumpPressed
                lastTimeJumpPressed = -1f;
            }
        }

        // Double Jump
        if (!blockInput)
        {
            if (playerInput.GetJumpInput())
            {
                if (!isDoubleJumping && !IsGrounded() && !ladderMovement.jumpedOffOfLadder)
                {
                    //Double-Jump Particle Animation
                    playerFX.transform.position = new Vector2(transform.position.x + 0.1f, transform.position.y - 0.35f);
                    playerFXAnimator.SetBool("hasDoubleJumped", true);

                    body.velocity = new Vector2(body.velocity.x, jumpPower / 1.5f);
                    isDoubleJumping = true; // Set double jump flag to prevent further jumps
                    animator.SetBool("grounded", IsGrounded());

                }
            }
        }
    }


    /// <summary>
    /// Handles dash input and initiates the dash coroutine if possible.
    /// </summary>
    public void HandleDashInput()
    {
        if (!blockInput)
        {
            float horizontalInput = playerInput.GetHorizontalInput();
            if (playerInput.GetDashInput() && canDash && horizontalInput != 0 && !isCrouching)
            {
                StartDash(horizontalInput);
                animator.SetTrigger("dash");
            }
        }
    }

    /// <summary>
    /// Handles crouch input and updates the player's sprite and collider accordingly.
    /// </summary>
    public void HandleCrouchInput()
    {
        if (!blockInput)
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
                    // Check for obstacles above
                    Collider2D obstacle = Physics2D.OverlapCircle(ceilingCheck.position, 0.2f, platformLayer | wallLayer);

                    // Only exits if no obstacle is detected above the player
                    if (!obstacle)
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
        }
    }

    #endregion

    #region Checkpoint

    // Method to update the respawn-point
    public void UpdateRespawnPoint(Vector3 newRespawnPosition, int checkpointID)
    {
        lastCheckpointID = checkpointID;
        respawnPosition = newRespawnPosition;
    }

    // Method to get the checkpointID for the SaveManager
    public int GetLastCheckpointID()
    {
        return lastCheckpointID;
    }

    public void Respawn()
    {
        // Set the position of the Player to the respawn-point
        transform.position = respawnPosition;

        // // Berechne den Offset zwischen dem Spieler-Pivot und dem Ground-Check
        // float pivotToGroundCheckOffset = transform.position.y - groundCheck.position.y;

        // // Passe die Respawn-Position an
        // Vector3 adjustedRespawnPosition = new Vector3(
        //     respawnPosition.x,
        //     respawnPosition.y + pivotToGroundCheckOffset,
        //     respawnPosition.z
        // );

        // // Setze die Position des Spielers
        // transform.position = adjustedRespawnPosition;

        // Reactivate the PlayerMovement-script
        enabled = true;

        // Reset death-flag
        isDead = false;

        // Reset velocity
        body.velocity = Vector2.zero;

        //TODO: Add respawn animation here
        //Reset die trigger
        animator.ResetTrigger("die");
        //Set Idle animation again, if not player is invisible unitl the animation is changed
        animator.Play("Idle");
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

        animator.SetBool("is_dashing", true);
        // Disable gravity during the dash for consistent movement
        float originalGravity = body.gravityScale;
        body.gravityScale = 0;

        float dashEndTime = Time.time + dashDuration;

        HealthSystem healthSystem = GetComponent<HealthSystem>();
        while (Time.time < dashEndTime)
        {
            // Move the player in the dash direction
            body.velocity = new Vector2(dashDirection * dashSpeed, 0);
            healthSystem.isInvincible = true;
            yield return null; // Wait for the next frame
        }

        isDashing = false;
        body.gravityScale = originalGravity; // Restore original gravity

        animator.SetBool("is_dashing", false);

        // Wait for dash cooldown before allowing another dash
        yield return new WaitForSeconds(dashCooldown);

        healthSystem.isInvincible = false;
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

    private void ResetAnimation()
    {
        playerFXAnimator.SetBool("resetAnimation", true);

        AnimatorStateInfo stateInfo = playerFXAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Transition"))
        {
            playerFXAnimator.SetBool("hasDoubleJumped", false);
        }

    }

    #region getter for tests
    public Rigidbody2D Body => body;
    public float WallSlideSpeed => wallSlideSpeed;
    public bool IsWallSliding => isWallSliding;
    public bool Grounded => grounded;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public bool IsDashing => isDashing;
    public bool IsCrouching => isCrouching;
    public bool IsWallJumping => isWallJumping;

    public Vector3 RespawnPosition => respawnPosition;

    #endregion
}

