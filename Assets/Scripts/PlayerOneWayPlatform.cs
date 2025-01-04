using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    private List<GameObject> currentOneWayPlatforms = new List<GameObject>(); // Platforms the player is interacting with
    [SerializeField] private GameObject player; // Reference to the player GameObject
    private Animator playerAnimator;
    [SerializeField] private BoxCollider2D playerCollider; // Player's collider
    [SerializeField] private float jumpBoost = 5f; // Optional upward force for smooth movement to the top
    [SerializeField] private float height = 1f;
    [SerializeField] private float time = 0.15f;

    void Update()
    {
        // Handle moving through the platform (pressing 'S')
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (var platform in currentOneWayPlatforms)
            {
                // Check if the player is below the platform
                if (platform.transform.position.y < player.transform.position.y)
                {
                    StartCoroutine(DisableCollision(platform));
                    break; // Only move to the first valid platform above
                }
            
            }
        }

        // Handle jumping on top of a platform (pressing 'Space')
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var platform in currentOneWayPlatforms)
            {
                // Check if the player is below the platform
                if (platform.transform.position.y > player.transform.position.y)
                {
                    StartCoroutine(MovePlayerToTopOfPlatform(platform));
                    break; // Only move to the first valid platform above
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Detect collision with a "OneWayPlatform"
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            if (!currentOneWayPlatforms.Contains(collision.gameObject))
            {
                currentOneWayPlatforms.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Remove the platform from the list when exiting collision
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            currentOneWayPlatforms.Remove(collision.gameObject);
        }
    }

    private IEnumerator DisableCollision(GameObject platform)
    {
        // Disable collision for all platforms the player is interacting with
        BoxCollider2D platformCollider = platform.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider);

        yield return new WaitForSeconds(0.5f);

        // Re-enable collision
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }

    private IEnumerator MovePlayerToTopOfPlatform(GameObject platform)
    {
        playerAnimator = player.GetComponent<Animator>();

        if (platform != null)
        {
            // Get the top Y position of the platform
            float platformTopY = platform.GetComponent<Collider2D>().bounds.max.y;


            // Option 1
            // Teleport (with Animation)
            // player.transform.position = new Vector2(player.transform.position.x, platformTopY + height);
            // playerAnimator.SetBool("is_climbing", true);

            // yield return new WaitForSeconds(5f);

            // playerAnimator.SetBool("is_climbing", false);


            // Option 2
            // Lerp moving funtion
            StartCoroutine(MoveSomeone(player.transform.position, new Vector2(player.transform.position.x, platformTopY + height), time));

            // Option 3
            // Apply an upward velocity for a smooth effect (optional)
            // Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            // if (rb != null && jumpBoost > 0)
            // {
            //     rb.velocity = new Vector2(rb.velocity.x, jumpBoost);
            // }
            yield return null;
        }
    }
    private IEnumerator MoveSomeone(Vector2 start, Vector2 end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            // Lerp position between start and end based on elapsed time
            player.transform.position = Vector2.Lerp(start, end, elapsedTime / duration);

            // Increment elapsed time by the time passed since last frame (deltaTime)
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure final position is set to end
        player.transform.position = end;
    }
}
