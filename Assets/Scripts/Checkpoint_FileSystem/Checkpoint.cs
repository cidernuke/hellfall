using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player has reached a checkpoint
        if (!isActive && collision.gameObject.CompareTag("Player"))
        {
            // Get access to the PlayerMovement-Script
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                //player.UpdateRespawnPoint(transform.position);
                //print(transform.position);
                player.UpdateRespawnPoint(player.transform.position);
                ActivateCheckpoint();
            }
        }
    }

    private void ActivateCheckpoint()
    {
        isActive = true;
        print("Checkpoint activated");
        // Add Visualisation or sound here
    }
}
