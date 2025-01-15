using System.Collections;
using UnityEngine;

public class SpiritAI : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float followDistance = 10f;  // Distance at which the spirit starts following
    [SerializeField] private float stopDistance = 1f;     // Distance to stop and be in front of the player

    [Header("Player Detection")]
    [SerializeField] private LayerMask playerLayer;
    private Transform playerTransform;

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Calculate the distance between the spirit and player
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Follow the player if they are within followDistance and outside the stopDistance
        if (distance < followDistance && distance > stopDistance)
        {
            FollowPlayer();
        }
        // Stop moving when in front of the player within stopDistance
        else if (distance <= stopDistance)
        {
            StopAndFacePlayer();
        }
    }

    private IEnumerator FindPlayer()
    {
        // Look for the player with the specified layer tag (or use other logic if necessary)
        while (playerTransform == null)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, followDistance, playerLayer);
            if (hit != null)
            {
                playerTransform = hit.transform;
            }
            yield return null;
        }
    }

    private void FollowPlayer()
    {
        // Move towards the player in the X direction only
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0; // Keep the y-position constant, only adjust the x-direction

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void StopAndFacePlayer()
    {
        // Stop movement (no translation)
        moveSpeed = 0f;

        // Keep the spirit directly in front of the player along the X-axis
        Vector3 position = transform.position;

        // Adjust spirit to the player's x-position, and move slightly in front
        position.x = playerTransform.position.x + (stopDistance);

        // Set the spirit's y-position to be the same as the player's y-position
        position.y = playerTransform.position.y;

        transform.position = position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, followDistance);  // Visualize the follow range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);    // Visualize the stop range
    }
}
