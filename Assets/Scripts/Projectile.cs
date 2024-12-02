using UnityEngine;


public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    private bool hit;
    private float direction;

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
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Wall")
        {
            hit = true;
            boxCollider.enabled = false;
            animator.SetTrigger("explode");
        }
        if(collision.tag == "Enemy")
        {
            EnemyController enemyController = collision.GetComponent<EnemyController>();
            collision.GetComponent<HealthSystem>().TakeDamage(damage, null,enemyController);          
            hit = true;
            boxCollider.enabled = false;
            animator.SetTrigger("explode");
        }
        
    }

    public void SetDirection(float _direction)
    {
        direction = _direction;
        gameObject.SetActive(true);        
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
