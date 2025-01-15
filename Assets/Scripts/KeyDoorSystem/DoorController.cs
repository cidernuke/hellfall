using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Collider2D doorCollider;  // The Collider blocking the door
    [SerializeField] private Animator anim;            // Open door animation

    private bool isOpen = false;

    private bool playerInRange = false;

    private KeySystem keySystem = GameObject.FindGameObjectWithTag("Player")?.GetComponent<KeySystem>();

    private void Update()
    {
        // Only check KeyPress if door is in range
        if (!isOpen && playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Try to open door
                TryOpenDoor();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void TryOpenDoor()
    {
        //var keySystem = GameObject.FindGameObjectWithTag("Player")?.GetComponent<KeySystem>();
        if (keySystem != null && keySystem.HasAllKeys())
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("Door is locked. You need 3 keys!");
            //Add UI later here
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        if (anim != null) anim.SetTrigger("open");
        if (doorCollider != null) doorCollider.enabled = false;
        Debug.Log("Door opened!");
        keySystem.ResetKeyCount();
        //Later: Update UI

    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (!isOpen && other.CompareTag("Player"))
    //     {
    //         var keySystem = other.GetComponent<KeySystem>();
    //         if (keySystem != null && keySystem.HasAllKeys())
    //         {
    //             // Player got all keys -> open door
    //             OpenDoor();
    //         }
    //         else
    //         {
    //             //TODO: Add UI in case player doesn't have all keys
    //             Debug.Log("Door is locked! You need 3 keys.");
    //         }
    //     }
    // }

    // private void OpenDoor()
    // {
    //     isOpen = true;
    //     // Play animation
    //     if (anim != null)
    //         anim.SetTrigger("open");

    //     //Deactivate collider so player can pass
    //     if (doorCollider != null)
    //         doorCollider.enabled = false;
    // }
}
