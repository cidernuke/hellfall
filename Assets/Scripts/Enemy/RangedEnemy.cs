using UnityEngine;

public class RangedEnemy2 : MonoBehaviour
{
    public Transform player; // Referenz auf den Spieler
    public GameObject bullet; // Bullet-Prefab

    [Header("Movement Settings")]
    public float followRange = 15f; // Reichweite, um dem Spieler zu folgen
    public float shootingRange = 10f; // Reichweite, um zu schießen
    public float moveSpeed = 2f; // Bewegungsgeschwindigkeit

    [Header("Shooting Settings")]
    private float shotCooldown;
    public float startShotCooldown = 2f; // Cooldown zwischen Schüssen

    [Header("References")]
    private Animator animator; // Animator-Referenz
    private HealthSystem healthSystem;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player GameObject nicht gefunden! Tag überprüfen.");
            }
        }

        shotCooldown = startShotCooldown;
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator-Komponente fehlt!");
        }

        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem-Komponente fehlt!");
        }
    }

    private void Update()
    {
        if (healthSystem != null && healthSystem.currentHealth <= 0)
        {
            HandleDeath();
            return;
        }

        if (player != null)
        {
            HandleMovementAndAttack();
        }
    }

    private void HandleMovementAndAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Richtung bestimmen und Enemy flippen
        FlipTowardsPlayer();

        if (distanceToPlayer > shootingRange && distanceToPlayer <= followRange)
        {
            MoveTowardsPlayer();
            animator.SetBool("isShooting", false); // Shooting-Animation deaktivieren
        }
        else if (distanceToPlayer <= shootingRange)
        {
            StopMoving();
            HandleShooting();
        }
        else
        {
            StopMoving();
            animator.SetBool("isShooting", false); // Shooting-Animation deaktivieren
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    private void StopMoving()
    {
        animator.SetBool("isShooting", false); // Animation stoppen, falls nicht schießen
    }

    private void FlipTowardsPlayer()
    {
        Vector3 scale = transform.localScale;
        if (player.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x); // Rechts schauen
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x); // Links schauen
        }
        transform.localScale = scale; // Nur X-Skalierung anpassen
    }

    private void HandleShooting()
    {
        if (shotCooldown <= 0)
        {
            animator.SetBool("isShooting", true);

            // Richtung berechnen
            Vector2 direction = (player.position - transform.position).normalized;

            // Projektil erzeugen
            GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            newBullet.transform.up = direction;

            shotCooldown = startShotCooldown;
        }
        else
        {
            shotCooldown -= Time.deltaTime;
        }
    }

    private void HandleDeath()
    {
        animator.SetTrigger("die");
        Destroy(gameObject, 1f); // Objekt nach 1 Sekunde zerstören (nach Animation)
    }

    public void TakeDamage(float damage)
    {
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(damage);
        }
    }
}
