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

    public void Awake()
    {
        // Get and store the Rigidbody2D component for efficiency
        body = GetComponent<Rigidbody2D>();
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
    }

    // Handle the horizontal movment of the player
    public void HandleMovementInput()
    {
        float horizontalInput = GetHorizontalInput();

        // Move the player horizontally
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

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
        if (GetJumpInput() && grounded)
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
        if (GetDashInput() && canDash && horizontalInput != 0) 
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
}
