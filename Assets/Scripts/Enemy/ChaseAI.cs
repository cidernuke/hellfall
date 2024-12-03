using UnityEngine;

public class ChaseAi : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float explosionDistance = 0.5f;

    private bool isExploding = false;

    void Update()
    {
        if (isExploding) return; // Stop moving if exploding

        float distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < chaseDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }

        // Check if enemy is close enough to trigger the explosion
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
