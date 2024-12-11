using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float triggerRadius = 3f; // Range to trigger the explosion
    [SerializeField] private float explosionRadius = 1f; // Radius to apply damage
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float explosionDelay = 2f; // Timer before explosion
    private Animator animator;
    private bool hasExploded = false;
    private bool damageDealt = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check if the player is within the trigger radius
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, triggerRadius, LayerMask.GetMask("Player"));
        if (playerCollider != null && !hasExploded)
        {
            TriggerExplosion();
        }
    }

    public void TriggerExplosion()
    {
        if (hasExploded) return; // Prevent multiple triggers
        hasExploded = true;

        // Trigger the explosion animation
        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }

        // Start explosion sequence
        StartCoroutine(ExplosionSequence());
    }

    private System.Collections.IEnumerator ExplosionSequence()
    {
        // Wait until halfway through the animation to deal damage
        float animationLength = GetAnimationLength("Explosion"); // Replace with your explosion animation name
        float halfwayPoint = animationLength / 2f;
        yield return new WaitForSeconds(halfwayPoint);

        // Damage all objects within the explosion radius
        if (!damageDealt)
        {
            DealDamage();
            damageDealt = true;
        }

        // Wait until the animation ends
        yield return new WaitForSeconds(animationLength - halfwayPoint);

        // Trigger the "Death" animation if available
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Ensure the renderer is disabled before destruction
        GetComponent<SpriteRenderer>().enabled = false;

        // Short delay for cleanup (optional)
        yield return new WaitForSeconds(0.2f);

        // Destroy the GameObject
        Destroy(gameObject);
    }

    private void DealDamage()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<HealthSystem>()?.TakeDamage(damageAmount);
            }
            else if (collider.CompareTag("Enemy"))
            {
                collider.GetComponent<HealthSystem>()?.TakeDamage(damageAmount);
            }
        }
    }

    private float GetAnimationLength(string animationName)
    {
        if (animator.runtimeAnimatorController == null) return 1f;

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        return 1f; // Default value if the animation is not found
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the trigger radius and explosion radius in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
