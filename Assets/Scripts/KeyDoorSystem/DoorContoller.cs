using UnityEngine;

/// <summary>
/// Controls a door that can be visually opened if the player has enough keys.
/// This script should be placed on the same GameObject that has the Animator and a trigger collider.
/// The player can open the door only if he has enough keys 
/// </summary>
public class DoorController2 : MonoBehaviour
{
    [SerializeField] private Animator doorAnimator;
    private bool isDoorOpen = false;

    // True if the player is currently within the door's trigger zone
    private bool playerInRange = false;


    /// <summary>
    /// Checks each frame if the player is in range and if the door is still closed. 
    /// If so, it waits for the player to press "E" to attempt opening the door.
    /// </summary>
    private void Update()
    {
        // Only proceed if the door is still closed and the player is inside the trigger zone
        if (!isDoorOpen && playerInRange)
        {
            // If the player presses "E", try to open the door
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryOpenDoor();
            }
        }
    }

    /// <summary>
    /// Activates the 'playerInRange' flag if it's the Player.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isDoorOpen && other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player in door trigger zone. Press [E] to open.");
        }
    }

    /// <summary>
    /// Deactivates the 'playerInRange' flag if it's the Player.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isDoorOpen && other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left the door trigger zone.");
        }
    }

    /// <summary>
    /// Attempts to open the door by checking if the player has the required number of keys.
    /// </summary>
    private void TryOpenDoor()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("No Player found in scene!");
            return;
        }

        // KeySystem suchen
        var keySystem = playerObj.GetComponent<KeySystem>();
        if (keySystem != null && keySystem.HasAllKeys())
        {
            // If the player has enough keys, open the door
            OpenDoor();
        }
        else
        {
            Debug.Log("Door is locked. Player has not enough keys!");
        }
    }

    /// <summary>
    /// Performs the actual door opening by triggering the animation and 
    /// optionally resetting the player's key count.
    /// </summary>
    private void OpenDoor()
    {
        isDoorOpen = true;
        Debug.Log("Door opened!");

        // Play the animation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("open");
        }

        // Reset keys
        var keySystem = FindObjectOfType<KeySystem>();
        if (keySystem != null)
        {
            keySystem.SetKeyCount(0);
        }
    }
}
