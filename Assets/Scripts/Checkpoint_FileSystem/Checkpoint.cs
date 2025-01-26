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

    private void Start()
    {
        animator = GetComponent<Animator>();        
        if (animator == null)
        {
            Debug.LogError("Animator-Component is missing on the Checkpoint.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player has reached a checkpoint
        if (!isActive && collision.gameObject.CompareTag("Player"))
        {
            // Get access to the PlayerMovement-Script
            playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                //player.UpdateRespawnPoint(transform.position);
                //print(transform.position);
                playerMovement.UpdateRespawnPoint(playerMovement.transform.position, checkpointID);
                

                healthSystem = collision.GetComponent<HealthSystem>();
                soulShardSystem = collision.GetComponent<SoulShardSystem>();
                //inventorySystem = collision.GetComponent<InventorySystem>();

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
