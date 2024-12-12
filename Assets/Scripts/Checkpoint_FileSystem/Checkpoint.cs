using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    private bool isActive = false;

    private PlayerMovement playerMovement;
    private HealthSystem healthSystem;
    private SoulShardSystem soulShardSystem;
    //private InventorySystem inventorySystem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the player has reached a checkpoint
        if (!isActive && collision.gameObject.CompareTag("Player"))
        {
            // Get access to the PlayerMovement-Script
            //PlayerMovement player = collision.GetComponent<PlayerMovement>();
            playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                //player.UpdateRespawnPoint(transform.position);
                //print(transform.position);
                playerMovement.UpdateRespawnPoint(playerMovement.transform.position, checkpointID);

                healthSystem = collision.GetComponent<HealthSystem>();
                soulShardSystem = collision.GetComponent<SoulShardSystem>();
                //inventorySystem = collision.GetComponent<InventorySystem>();

                //if (healthSystem != null)
                //if (healthSystem != null && inventorySystem != null && soulShardSystem != null)
                if (healthSystem != null && InventorySystem.Instance != null && soulShardSystem != null)
                {
                    ActivateCheckpoint();
                }
                else
                {
                    Debug.LogError("Eine oder mehrere benötigte Komponenten wurden nicht gefunden. HealthSystem: " + healthSystem + "| SoulshardSystem: " + soulShardSystem + "| inventorySystem: " + InventorySystem.Instance);
                    //Debug.LogError("Eine oder mehrere benötigte Komponenten wurden nicht gefunden. HealthSystem: "+ healthSystem+ "| SoulshardSystem: " + soulShardSystem+ "| inventorySystem: " + inventorySystem);
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

        healthSystem.respawnHealth = healthSystem.currentHealth;

        // Spiel speichern über den GameManager
        GameManager.Instance.SaveGame();

        // Spiel speichern
        //SaveManager.Instance.SaveGame(playerMovement, healthSystem,soulShardSystem,InventorySystem.Instance);
        //SaveManager.Instance.SaveGame(playerMovement, healthSystem,soulShardSystem,inventorySystem);
        // Add Visualisation or sound here
    }
}
