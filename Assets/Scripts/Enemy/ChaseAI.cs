using UnityEngine;

public class ChaseAi : MonoBehaviour
{
    [SerializeField] private GameObject player; // Ensure this is assigned in the inspector or dynamically
    [SerializeField] private float speed;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float explosionDistance = 0.5f;

    private bool isExploding = false;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player"); // Automatically find player by tag if not assigned
            if (player == null)
            {
                Debug.LogError("Player GameObject not found! Ensure it is tagged correctly or assigned in the inspector.");
            }
        }
    }

    void Update()
    {
        if (isExploding || player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < chaseDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
            Debug.Log($"Moving towards player. Current position: {transform.position}");
        }

        if (distance < explosionDistance)
        {
            StartExplosion();
        }
    }

    private void StartExplosion()
    {
        isExploding = true;
        GetComponent<Explosion>()?.TriggerExplosion();
    }
}
