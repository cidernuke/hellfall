using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    private Transform player; // Reference to the player
    [SerializeField] private float minX = -10f; // Minimum x boundary for the background
    [SerializeField] private float maxX = 10f;  // Maximum x boundary for the background
    [SerializeField] private float offset = 0f; // Optional offset for positioning

    private Vector3 initialPosition; // To store the initial y and z positions of the background

    private void Start()
    {
        // Store the initial position of the background
        initialPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (PlayerInBossRoom())
        {
            // Calculate the new x position based on the player's x position and offset
            float targetX = Mathf.Clamp(player.position.x + offset, minX, maxX);

            // Update the background's position while keeping its y and z positions constant
            transform.position = new Vector3(targetX, initialPosition.y, initialPosition.z);
        }
    }

    private bool PlayerInBossRoom()
    {
        // Replace this logic with your actual condition for the boss room
        // Example: Check if the player is within a certain range or area
        return player.position.x >= minX && player.position.x <= maxX;
    }
}
