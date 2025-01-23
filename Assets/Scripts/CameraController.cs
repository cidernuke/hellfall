using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Sets the distance for how far the camera looks ahead, configurable in the editor
    [SerializeField] private float aheadDistance;
    // Adds the number to the y position of the player
    [SerializeField] private float aheadY;
    // Sets the speed at which the camera will adjust its position, configurable in the Editor
    [SerializeField] private float cameraSpeed;
    // Tracks how far ahead the camera is looking currently
    private float lookAhead;
    // Sets the player which the camera tracks, configurable in the editor
    private Transform player;

    /// <summary>
    /// Initializes the player transform. If not assigned in the inspector, it searches for a GameObject with the tag "Player".
    /// </summary>
    private void Awake()
    {
        // If the player is not already assigned in the Inspector,
        // search for it at runtime by the tag "Player":
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                Debug.LogError("No GameObject with tag 'Player' found in the scene!");
            }
        }
    }

    /// <summary>
    /// Updates the camera position based on the player's position.
    /// </summary>
    private void Update()
    {
        if (player == null)
        {
            // Do nothing as long as no player is found
            return;
        }

        transform.position = new Vector3(player.position.x + lookAhead, player.position.y + aheadY, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, aheadDistance * player.localScale.x, Time.deltaTime * cameraSpeed);
    }
}
