using UnityEngine;

public class ChaseAi : MonoBehaviour
{
    // Reference to the player GameObject that the AI will chase
    [SerializeField] private GameObject player; // Ensure this is assigned in the inspector or dynamically

    // Movement speed of the AI
    [SerializeField] private float speed;

    // The distance at which the AI will start chasing the player
    [SerializeField] private float chaseDistance;

    // The distance at which the AI will trigger the explosion
    [SerializeField] private float explosionDistance = 0.5f;

    // Boolean to track if the AI is in an exploding state
    private bool isExploding = false;

    // Start is called before the first frame update
    private void Start()
    {
        // If player is not assigned in the inspector, try to find it dynamically by its tag
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Automatically find player by tag if not assigned
            if (player == null)
            {
                Debug.LogError("Player GameObject not found! Ensure it is tagged correctly or assigned in the inspector.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the AI is exploding or the player is not assigned, do nothing
        if (isExploding || player == null) return;

        // Calculate the distance between the AI and the player
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // Calculate direction to the player
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize(); // Normalize the direction vector to avoid faster movement diagonally

        // Calculate the angle for rotation to face the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // If the AI is within chase distance, move towards the player
        if (distance < chaseDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            // Rotate to face the player
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }

        // If the AI is within explosion distance, trigger explosion
        if (distance < explosionDistance)
        {
            StartExplosion();
        }
    }

    // Method to start explosion
    private void StartExplosion()
    {
        isExploding = true; // Set explosion flag to true
        GetComponent<Explosion>()?.TriggerExplosion(); // Trigger the explosion using the Explosion script (if attached)
    }
}
