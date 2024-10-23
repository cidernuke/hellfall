using UnityEngine;
using System.Collections; // Important for Coroutinen

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private bool grounded;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    // Dash variables
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool isDashing;
    private bool canDash = true;
    private float dashDirection;


    //Crouch variables
    [SerializeField] private Sprite standing;
    [SerializeField] private Sprite crouching;

    private Vector2 standingSize;
    private Vector2 crouchingSize;
    private Vector2 standingOffset;
    private Vector2 crouchingOffset;
    private bool isCrouching = false;

    // Sprite positions
    private Vector3 standingSpritePosition;
    private Vector3 crouchingSpritePosition;


    public void Awake()
    {
        // Get the Rigidbody2D component for efficiency
        body = GetComponent<Rigidbody2D>();

        // Get the BoxCollider component for efficiency
        boxCollider = GetComponent<BoxCollider2D>();

        // Get the SpriteRenderer component for efficiency
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Get the default size from the existing collider
        standingSize = boxCollider.size;

        standingOffset = boxCollider.offset;

        // Crouch-Größe und Offset einstellen
        float crouchHeight = standingSize.y * 0.5f; // Beispiel: halbe Höhe
        float sizeDifference = standingSize.y - crouchHeight;

        crouchingSize = new Vector2(standingSize.x, crouchHeight);
        // Offset so anpassen, dass die Unterkante des Colliders gleich bleibt
        crouchingOffset = new Vector2(standingOffset.x, standingOffset.y - sizeDifference / 2f);

        // Initiale Einstellungen
        spriteRenderer.sprite = standing;
        boxCollider.size = standingSize;
        boxCollider.offset = standingOffset;


        // //Make sure the coruch size is set
        // if (crouchingSize == Vector2.zero)
        // {
        //     crouchingSize = new Vector2(standingSize.x, standingSize.y * 0.5f); // Crouch size = 1/2
        // }

        // // Set initial sprite
        // spriteRenderer.sprite = standing;
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

    // Handle the horizontal movment of the player
    public void HandleMovementInput()
    {
        float horizontalInput = GetHorizontalInput();

        float initialSpeed = speed;
        if (isCrouching)
        {
            initialSpeed *= 0.5f; // Half the speed while crouching
        }

        // Move the player horizontally
        body.velocity = new Vector2(horizontalInput * initialSpeed, body.velocity.y);

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

    // Handle the player's jumping
    public void HandleJumpInput()
    {
        if (GetJumpInput() && grounded && !isCrouching)
        {
            // Apply vertical velocity to make the player jump
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            grounded = false;
        }
    }

    //Handle the player's crouch
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

    // Handle the player's dash
    public void HandleDashInput()
    {
        float horizontalInput = GetHorizontalInput();
        if (GetDashInput() && canDash && horizontalInput != 0 && !isCrouching)
        {
            StartDash(horizontalInput);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has landed on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

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

    //Method to get crouch input
    public virtual bool GetCrouchInput()
    {
        return Input.GetKey(KeyCode.S);
    }
}
