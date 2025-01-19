using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player in range of Key. Press [E] to pick up.");
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

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left range of Key.");
        }
    }

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
            Destroy(gameObject); // Destroy key when picked-up
        }
        else
        {
            Debug.LogWarning("KeySystem component not found on the player!");
        }
    }
}

