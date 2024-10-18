using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private bool grounded;

    private void Awake()
    {
        // Cache the Rigidbody2D component for efficiency
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
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
    }

    private void Jump()
    {
        //Apply vertical velocity to make the player jump
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has landed on the ground
        if(collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }
}
