using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    [SerializeField] private int damageAmount = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // Get the HealthSystem component from the player
            HealthSystem playerHealth = other.gameObject.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount); // Apply damage
            }
            Destroy(gameObject); // Destroy the bullet after the collision
        }
    }
}
