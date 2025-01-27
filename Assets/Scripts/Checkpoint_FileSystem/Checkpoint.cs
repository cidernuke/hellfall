using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    private bool isActive = false;

    private PlayerMovement playerMovement;
    private HealthSystem healthSystem;
    private SoulShardSystem soulShardSystem;
    private Animator animator;    
    private float checkpointMessageDuration = 2f;

    /// <summary>
    /// Initializes the animator.
    /// </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();        
        if (animator == null)
        {
            Debug.LogError("Animator-Component is missing on the Checkpoint.");
        }
    }
    /// <summary>
    /// Checks if the player has reached a checkpoint.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player has reached a checkpoint
        if (!isActive && collision.gameObject.CompareTag("Player"))
        {
            // Get access to the PlayerMovement-Script
            playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.UpdateRespawnPoint(playerMovement.transform.position, checkpointID);
                

                healthSystem = collision.GetComponent<HealthSystem>();
                soulShardSystem = collision.GetComponent<SoulShardSystem>();

                if (healthSystem != null && InventorySystem.Instance != null && soulShardSystem != null)
                {
                    ActivateCheckpoint();
                }
                else
                {
                    Debug.LogError("Eine oder mehrere benötigte Komponenten wurden nicht gefunden. HealthSystem: " + healthSystem + "| SoulshardSystem: " + soulShardSystem + "| inventorySystem: " + InventorySystem.Instance);
                }
            }
            else
            {
                Debug.LogError("PlayerMovement-Komponente wurde nicht gefunden.");
            }
        }
    }

    /// <summary>
    /// Activates the checkpoint.
    /// </summary>
    private void ActivateCheckpoint()
    {
        isActive = true;
        Debug.Log("Checkpoint " + checkpointID + " aktiviert");

        if (animator != null)
        {
            animator.Play("activate_checkpoint");
        }

        healthSystem.respawnHealth = healthSystem.currentHealth;

        // Save game with GameManager
        GameManager.Instance.SaveGame();

        //ShowCheckpointMessage();
        DeathUIManager.Instance.ShowCheckpointMessage("Checkpoint,\nSaved game.", checkpointMessageDuration);
    }
}
