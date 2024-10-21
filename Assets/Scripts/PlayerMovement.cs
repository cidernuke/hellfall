using UnityEngine;
using System.Collections; // Important for Coroutinen

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private bool grounded;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;


    // Dash variables
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;

    private bool isDashing;
    private bool canDash = true;
    private float dashDirection;


    // Crouch variables
    [SerializeField] private float crouchSpeedReduction = 0.5f; // Reduce speed while crouching
    [SerializeField] private Vector3 crouchScale = new Vector3(1, 0.5f, 1); // Player becomes smaller
    private Vector3 originalScale;
    private bool isCrouching;

    public void Awake()
    {
        // Get and store the Rigidbody2D component for efficiency
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale; // Store the original scale of the player
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

        // Adjust speed if crouching
        float currentSpeed = isCrouching ? speed * crouchSpeedReduction : speed;

        // Move the player horizontally
        body.velocity = new Vector2(horizontalInput * currentSpeed, body.velocity.y);

        // Flip the player's sprite based on movement direction
        if (horizontalInput > 0.01f)
        {
            //transform.localScale = isCrouching ? crouchScale : Vector3.one;
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), isCrouching ? crouchScale.y : originalScale.y, originalScale.z);
        }
        else if (horizontalInput < -0.01f)
        {
            //transform.localScale = isCrouching ? new Vector3(-crouchScale.x, crouchScale.y, crouchScale.z) : new Vector3(-1, 1, 1);
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), isCrouching ? crouchScale.y : originalScale.y, originalScale.z);
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

    // Handle the player's dash
    public void HandleDashInput()
    {
        float horizontalInput = GetHorizontalInput();
        if (GetDashInput() && canDash && horizontalInput != 0 && !isCrouching) 
        {
            StartDash(horizontalInput);
        }
    }

    // Handle the player's crouch input
    public void HandleCrouchInput()
    {
        if (GetCrouchInput()) // If crouch key is pressed
        {
            if (!isCrouching)
            {
                isCrouching = true;
                transform.localScale = new Vector3(transform.localScale.x, crouchScale.y, crouchScale.z); //Reduce player's size and maintain X direction while crouching
                //transform.localScale = crouchScale; // Reduce player's size
            }
        }
        else
        {
            if (isCrouching)
            {
                isCrouching = false;
                transform.localScale =new Vector3(transform.localScale.x, originalScale.y, originalScale.z); // Restore original size and maintain direction
                //transform.localScale = originalScale; // Reset player's size
            }
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

    // Method to get crouch input
    public virtual bool GetCrouchInput()
    {
        return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S);
    }
}

