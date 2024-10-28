using UnityEngine;
using System.Collections; // Important for Coroutines

public class PlayerMovement : MonoBehaviour
{
    #region Movement Variables
    private Rigidbody2D body;
    private Animator animator;
    private bool grounded;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
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
        InitializeCrouchVariables();
        InitializeDashVariables();
           
    }

    public void Update()
    {
        if (isDashing)
        {
            return;
        }

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
        // Get reference to the Animator component
        animator = GetComponent<Animator>(); 

        // Error checking
        if (body == null)
            Debug.LogError("Rigidbody2D not found!");
        if (boxCollider == null)
            Debug.LogError("BoxCollider2D not found!");
        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer not found!");
        if (animator == null)
            Debug.LogError("Animator not found!");
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
        //animator.SetBool("crouch_walking", false);

        // Move the player horizontally
        body.velocity = new Vector2(horizontalInput * currentSpeed, body.velocity.y);

        // Flip the player's sprite based on movement direction
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Update the animator's parameters
        animator.SetBool("run", horizontalInput != 0);       
        animator.SetBool("crouch_walking", isCrouching && horizontalInput != 0);
        if(horizontalInput != 0 && isCrouching){
            animator.SetBool("crouch", false);
        }

        
          
        
    }

    // Handle the player's jumping
    public void HandleJumpInput()
    {
        if (GetJumpInput() && grounded && !isCrouching)
        {
            // Apply vertical velocity to make the player jump
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");                 
            grounded = false;
        }
        animator.SetBool("grounded", grounded);
    }

    // Handle the player's dash
    public void HandleDashInput()
    {
        float horizontalInput = GetHorizontalInput();
        if (GetDashInput() && canDash && horizontalInput != 0 && !isCrouching)
        {
            StartDash(horizontalInput);
            animator.SetTrigger("dash");
        }
    }

    // Handle the player's crouch
    public void HandleCrouchInput()
    {
        float horizontalInput = GetHorizontalInput();

        if (GetCrouchInput())
        {
            if (!isCrouching)
            {
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
    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has landed on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
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
        return Input.GetKey(KeyCode.Space);
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
