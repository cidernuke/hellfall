using UnityEngine;

public class Projectile : MonoBehaviour
{
    // variables
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    private bool hit;
    private float direction;
    private float directionY;
    private float lifeTime;
    private readonly float maxLifetime = 3f;

    // references
    private BoxCollider2D boxCollider;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }


    private void Update()
    {
        if (hit)
        {
            return;
        }

        if (transform.CompareTag("blood_bullet_up"))
        {
            float movementSpeed = speed * Time.deltaTime * directionY;
            transform.Translate(0, movementSpeed, 0);
        }
        else
        {
            float movementSpeed = speed * Time.deltaTime * direction;
            transform.Translate(movementSpeed, 0, 0);
        }

        SetLifeTime();
    }

    /// <summary>
    /// Sets the lifetime of the projectile.
    /// </summary>
    private void SetLifeTime()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > maxLifetime)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Checks for collision with walls and enemies.
    /// activates the explosion animation and deactivates the projectile.
    /// if the projectile hits an enemy, the enemy takes damage.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            hit = true;
            boxCollider.enabled = false;
            animator.SetTrigger("explode");
        }

        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.GetComponent<EnemyController>();
            if (collision.GetComponent<HealthSystem>() == null)
            {
                return;
            }
            collision.GetComponent<HealthSystem>().TakeDamage(damage, null, enemyController);
            hit = true;
            boxCollider.enabled = false;
            animator.SetTrigger("explode");
        }
        
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (collision.GetComponent<HealthSystem>() == null)
            {
                return;
            }
            collision.GetComponent<HealthSystem>().TakeDamage(damage, playerMovement, null);
            if (!transform.CompareTag("blood_bullet_up"))
            {
                hit = true;
                boxCollider.enabled = false;
                animator.SetTrigger("blood_bullet_hit");
            }
        }
    }

    /// <summary>
    /// Sets the direction of the projectile and activates it.
    /// Sets the lifetime to 0.
    /// </summary>
    /// <param name="_direction">The direction of the projectile.</param>
    public void SetDirection(Vector2 _direction)
    {
        lifeTime = 0;
        direction = _direction.x;
        directionY = _direction.y;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction.x)
        {
            localScaleX = -localScaleX;
        }

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Deactivates the projectile.
    /// gets called by the animation "explode" event.
    /// </summary>
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
