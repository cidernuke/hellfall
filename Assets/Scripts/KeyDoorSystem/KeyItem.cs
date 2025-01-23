using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("UI Hint")]
    [Tooltip("Assign a small UI object (e.g. Text) that says 'Press E to pick up'.")]
    [SerializeField] private GameObject pickUpHintUI; 

    private bool playerInRange = false;
    private ItemRespawner respawner;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Get the ItemRespawner on the same object
        respawner = GetComponent<ItemRespawner>();
    }

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object.
    /// </summary>
    /// <param name="collider">The other collider involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player in range of Key. Press [E] to pick up.");

            if (pickUpHintUI != null)
                pickUpHintUI.SetActive(true);
        }
    }

    // Version where the Player can just walk over the Key instead of pressing E
    //    private void OnTriggerEnter2D(Collider2D collider)
    // {
    //     if (collider.CompareTag("Player"))
    //     {
    //         playerInRange = true;
    //         Version where the Player can just walk into the key
    //         var keySystem = collider.GetComponent<KeySystem>();
    //         if (keySystem != null)
    //         {
    //             //Player collect key with collision
    //             keySystem.AddKey();

    //             // Destroy this key || respawner technique
    //             Destroy(gameObject);
    //         }
    //         else
    //         {
    //             Debug.LogWarning("KeySystem component not found on the player!");
    //         }
    //     }
    // }

    /// <summary>
    /// Called when another collider exits the trigger collider attached to this object.
    /// </summary>
    /// <param name="collider">The other collider involved in this collision.</param>
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left range of Key.");

            if (pickUpHintUI != null)
                pickUpHintUI.SetActive(false);
        }
    }

    /// <summary>
    /// Called once per frame to update the object's state.
    /// </summary>
    private void Update()
    {
        // Only react if player is in range
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickupKey();
            }
        }
    }

    /// <summary>
    /// Handles the logic for picking up the key.
    /// </summary>
    private void PickupKey()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("No Player found in scene!");
            return;
        }

        var keySystem = playerObj.GetComponent<KeySystem>();
        if (keySystem != null)
        {
            keySystem.AddKey();
            Debug.Log("Key picked up via E-press!");
            //Destroy(gameObject); // Destroy key when picked-up

            if (respawner != null)
            {
                respawner.CollectItem();
            }
            else
            {
                // Fallback: if no respawner, just destroy the key
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.LogWarning("KeySystem component not found on the player!");
        }
    }
}

