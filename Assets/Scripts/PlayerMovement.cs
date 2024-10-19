using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private bool grounded;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    //Dash variables
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;

    private bool isDashing;

    // Indicates if the player can dash (not on cooldown).
    private bool canDash = true;
    private float dashTime;
    private float dashDirection;

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Get and store the Rigidbody2D component for efficiency
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame.
    private void Update()
    {
        // If the player is dashing, skip the rest of the Update to prevent normal movement.
        if (isDashing)
        {
            return;
        }

        float horizontalInput = Input.GetAxis("Horizontal");

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

        //Handle jump input
        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            Jump();
        }

        // Handle dash input.
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && horizontalInput != 0)
        {
            StartDash(horizontalInput);
        }
    }


    // FixedUpdate is called at fixed intervals and is used for physics updates.
    private void FixedUpdate()
    {
        // Apply dash velocity
        if (isDashing)
        {
            body.velocity = new Vector2(dashDirection * dashSpeed, body.velocity.y);
        }
    }

    // LateUpdate is called after all Update methods have been called.
    private void LateUpdate()
    {
        // Check if dash duration has passed
        if (isDashing && Time.time - dashTime >= dashDuration)
        {
            EndDash();
        }
    }

    private void Jump()
    {
        //Apply vertical velocity to make the player jump
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        grounded = false;
    }

    // Method called when the player starts colliding with another object.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has landed on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void StartDash(float direction)
    {
        // Set isDashing to true to indicate the player is dashing.
        isDashing = true;
        // The player cannot dash again until the cooldown is over.
        canDash = false;
        // Determine the dash direction based on the player's input.
        dashDirection = Mathf.Sign(direction);
        // Record the time when the dash started.
        dashTime = Time.time;

        //Add dash effects here (e.g., particles, animation)
    }

    private void EndDash()
    {
        // Set isDashing to false since the dash is over.
        isDashing = false;
        // Start the cooldown timer before the player can dash again.
        Invoke(nameof(ResetDash), dashCooldown);
    }

    // Method to reset the ability to dash after the cooldown.
    private void ResetDash()
    {
        // Allow the player to dash again.
        canDash = true;
    }
}
