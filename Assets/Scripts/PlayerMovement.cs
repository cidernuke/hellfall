using UnityEngine;
using System.Collections; // Important for Coroutines

public class PlayerMovement : MonoBehaviour
{
    #region Layer Masks
    private LayerMask wallLayer;
    private LayerMask groundLayer;

    #endregion

    #region Movement Variables
    private Rigidbody2D body;
    private bool grounded;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    #endregion


    #region Advanced Jumping Variables
    //Multiple Jumps
    [SerializeField] private int possibleJumps = 2;
    private int jumpCounter;

    // Wall Jumping
    [SerializeField] private float wallSlideSpeed = 2f; // Speed of sliding down a wall
    [SerializeField] private float wallJumpX = 15f;     // Horizontal force while Wall-Jumping

    private bool isTouchingWall = false;
    private bool hasWallJumped = false;

    private bool isWallJumping = false;
    private float wallJumpDuration = 0.2f;
    private float wallJumpStartTime;
    #endregion

    #region Dash Variables
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool isDashing;
    private bool canDash = true;
    private float dashDirection;
    #endregion

    #region Crouch Variables
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    [SerializeField] private Sprite standing;
    [SerializeField] private Sprite crouching;

    private Vector2 standingSize;
    private Vector2 crouchingSize;
    private Vector2 standingOffset;
    private Vector2 crouchingOffset;
    private bool isCrouching = false;
    #endregion

    #region Unity Methods
    public void Awake()
    {
        InitializeComponents();
        InitializeLayers();
        InitializeCrouchVariables();
        InitializeDashVariables();
        jumpCounter = possibleJumps;
    }

    public void Update()
    {
        if (isDashing)
        {
            return;
        }

        isTouchingWall = IsTouchingWall();

        HandleMovementInput();
        HandleJumpInput();
        HandleDashInput();
        HandleCrouchInput();
    }
    #endregion

    #region Initialization Methods
    private void InitializeComponents()
    {
        // Get and store the Rigidbody2D component for efficiency
        body = GetComponent<Rigidbody2D>();
        // Get the BoxCollider component for efficiency
        boxCollider = GetComponent<BoxCollider2D>();
        // Get the SpriteRenderer component for efficiency
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Error checking
        if (body == null)
            Debug.LogError("Rigidbody2D not found!");
        if (boxCollider == null)
            Debug.LogError("BoxCollider2D not found!");
        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer not found!");
    }

    private void InitializeLayers()
    {
        wallLayer = LayerMask.GetMask("Wall");
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void InitializeCrouchVariables()
    {
        // Get the default size from the existing collider
        standingSize = boxCollider.size;
        standingOffset = boxCollider.offset;

        // Calculate crouch size and offset
        float crouchHeight = standingSize.y * 0.5f; // Crouch size = 1/2 of standing size
        float sizeDifference = standingSize.y - crouchHeight;

        crouchingSize = new Vector2(standingSize.x, crouchHeight);
        // Adjust offset so that the bottom of the collider remains the same
        crouchingOffset = new Vector2(standingOffset.x, standingOffset.y - sizeDifference / 2f);

        // Set initial sprite
        spriteRenderer.sprite = standing;
        boxCollider.size = standingSize;
        boxCollider.offset = standingOffset;
    }

    private void InitializeDashVariables()
    {
        isDashing = false;
        canDash = true;
    }
    #endregion

    #region Input Handling Methods
    // Handle the horizontal movement of the player
    public void HandleMovementInput()
    {
        float horizontalInput = GetHorizontalInput();

        // Adjust speed if crouching
        float currentSpeed = speed;
        if (isCrouching)
        {
            currentSpeed *= 0.5f; // Half the speed while crouching
        }

        if (isWallJumping)
        {
            //While Wall-Jumping no horizontal movement, to prevent overriding the force of the Wall-Jump
            if (Time.time > wallJumpStartTime + wallJumpDuration)
            {
                isWallJumping = false;
            }
        }
        else if (isTouchingWall && !grounded && body.velocity.y < 0)
        {
            //Wall-Sliding
            body.velocity = new Vector2(0, -wallSlideSpeed);
        }
        else
        {
            //Move the player horizontally
            body.velocity = new Vector2(horizontalInput * currentSpeed, body.velocity.y);
        }

        if (!isWallJumping)
        {
            // Flip the player's sprite based on movement direction
            if (horizontalInput > 0.01f)
            {
                transform.localScale = Vector3.one;
            }
            else if (horizontalInput < -0.01f)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

    }

    // Handle the player's jumping
    public void HandleJumpInput()
    {
        if (GetJumpInput() && !isCrouching)
        {
            if (grounded)
            {
                //Regular Jump
                // Apply vertical velocity to make the player jump
                body.velocity = new Vector2(body.velocity.x, jumpPower);
                grounded = false;
                jumpCounter--;
                hasWallJumped = false;
            }
            else if (isTouchingWall && !hasWallJumped)
            {
                //Wall-Jump
                WallJump();
                hasWallJumped = true;
                jumpCounter--; //Delete this line, if Player should be able to Air-Jump after Wall-Jump
            }
            else if (jumpCounter > 0)
            {
                //Double-Jump
                body.velocity = new Vector2(body.velocity.x, jumpPower);
                jumpCounter--;
            }
        }
    }

    // Handle the player's dash
    public void HandleDashInput()
    {
        float horizontalInput = GetHorizontalInput();
        if (GetDashInput() && canDash && horizontalInput != 0 && !isCrouching)
        {
            StartDash(horizontalInput);
        }
    }

    // Handle the player's crouch
    public void HandleCrouchInput()
    {
        if (GetCrouchInput())
        {
            if (!isCrouching)
            {
                spriteRenderer.sprite = crouching;
                boxCollider.size = crouchingSize;
                boxCollider.offset = crouchingOffset;
                isCrouching = true;
            }
        }
        else
        {
            if (isCrouching)
            {
                spriteRenderer.sprite = standing;
                boxCollider.size = standingSize;
                boxCollider.offset = standingOffset;
                isCrouching = false;
            }
        }
    }
    #endregion

    #region Collision Methods
    public void OnCollisionEnter2D(Collision2D collision)
    {
        int collisionLayer = collision.gameObject.layer;
        if ((groundLayer.value & (1 << collisionLayer)) != 0)
        {
            grounded = true;
            jumpCounter = possibleJumps; // Reset jumpCounter
            hasWallJumped = false;       // Reset Wall-Jump Flag
        }
        else if ((wallLayer.value & (1 << collisionLayer)) != 0)
        {
            hasWallJumped = false; //Reset when touching a new Wall
        }
        // Check if the player has landed on the ground
        // if (collision.gameObject.CompareTag("Ground"))
        // {
        //     grounded = true;
        //     jumpCounter = possibleJumps; //Reset the jump counter
        // }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        // if (collision.gameObject.CompareTag("Ground"))
        // {
        //     grounded = false;
        // }
        int collisionLayer = collision.gameObject.layer;
        if ((groundLayer.value & (1 << collisionLayer)) != 0)
        {
            grounded = false;
        }

        //Noch aus Wall-Jump V1
        // if ((wallLayer.value & (1 << collisionLayer)) != 0)
        // {
        //     hasWallJumped = false;
        // }
    }
    #endregion

    #region Dash Coroutine
    public void StartDash(float direction)
    {
        dashDirection = Mathf.Sign(direction);
        StartCoroutine(DashCoroutine());
    }

    public IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;

        // Disable gravity during dash for consistent movement
        float originalGravity = body.gravityScale;
        body.gravityScale = 0;

        float dashEndTime = Time.time + dashDuration;

        while (Time.time < dashEndTime)
        {
            body.velocity = new Vector2(dashDirection * dashSpeed, 0);
            yield return null; // Wait for the next frame
        }

        isDashing = false;
        body.gravityScale = originalGravity;

        // Wait for the dash cooldown
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
    #endregion

    #region Wall Methods
    private bool IsTouchingWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            new Vector2(transform.localScale.x, 0),
            0.1f,
            wallLayer);
        return raycastHit.collider != null;
    }

    private void WallJump()
    {
        //Applying force away from the Wall
        float horizontalForce = -Mathf.Sign(transform.localScale.x) * wallJumpX;
        Vector2 force = new Vector2(horizontalForce, jumpPower);
        //body.velocity = new Vector2(horizontalForce, jumpPower);
        body.AddForce(force, ForceMode2D.Impulse);

        // Flip Player
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        grounded = false;

        //Set a flag to prevent movement input from overriding the wall jump
        isWallJumping = true;
        wallJumpStartTime = Time.time;
    }
    #endregion

    #region Input Methods
    // Abstracted input methods to simulate in tests easily
    // Method to get horizontal input

    public virtual float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }

    // Method to get jump input
    public virtual bool GetJumpInput()
    {
        //getKey vs getKeyDown
        // GetKey remains true as long as the key is held down
        // GetKeyDown is true only in the single frame when the key is initially pressed
        return Input.GetKeyDown(KeyCode.Space);
    }

    // Method to get dash input
    public virtual bool GetDashInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    // Method to get crouch input
    public virtual bool GetCrouchInput()
    {
        return Input.GetKey(KeyCode.S);
    }
    #endregion
}
