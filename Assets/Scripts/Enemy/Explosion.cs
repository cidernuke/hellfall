using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float triggerRadius = 3f;
    [SerializeField] private float explosionRadius = 1f;
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private LayerMask playerLayer;

    private Animator animator;
    private bool hasExploded = false;
    private bool damageDealt = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, triggerRadius, playerLayer);
        if (playerCollider != null && !hasExploded)
        {
            Debug.Log("Player entered trigger radius. Triggering explosion.");
            TriggerExplosion();
        }
    }

    public void TriggerExplosion()
    {
        if (hasExploded) return;

        hasExploded = true;
        Debug.Log("Explosion triggered!");

        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }

        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        float animationLength = GetAnimationLength("Explosion");
        yield return new WaitForSeconds(animationLength / 2); // Wait halfway through the animation

        if (!damageDealt)
        {
            DealDamage();
            damageDealt = true;
        }

        yield return new WaitForSeconds(animationLength / 2); // Wait for the rest of the animation

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        yield return new WaitForSeconds(0.2f); // Allow death animation to play

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
        return 1f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
