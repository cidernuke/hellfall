using System.Collections;
using TMPro;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    private bool isActive = false;

    private PlayerMovement playerMovement;
    private HealthSystem healthSystem;
    private SoulShardSystem soulShardSystem;
    //private InventorySystem inventorySystem;
    private Animator animator;
    [SerializeField] private TMP_Text checkpointMessage;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator-Component is missing on the Checkpoint.");
        }

        if (checkpointMessage != null)
        {
            // Zu Beginn ausblenden
            checkpointMessage.gameObject.SetActive(false);
        }
    }

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

        if (animator != null)
        {
            animator.Play("activate_checkpoint");
        }

        healthSystem.respawnHealth = healthSystem.currentHealth;

        // Spiel speichern über den GameManager
        GameManager.Instance.SaveGame();

        ShowCheckpointMessage();

        // Spiel speichern
        //SaveManager.Instance.SaveGame(playerMovement, healthSystem,soulShardSystem,InventorySystem.Instance);
        //SaveManager.Instance.SaveGame(playerMovement, healthSystem,soulShardSystem,inventorySystem);
        // Add Visualisation or sound here
    }

    private void ShowCheckpointMessage()
    {
        if (checkpointMessage != null)
        {
            checkpointMessage.text = "Checkpoint,\nSaved game.";
            checkpointMessage.gameObject.SetActive(true);

            StartCoroutine(HideMessageAfterDelay(3f));
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        checkpointMessage.gameObject.SetActive(false);
    }
}
