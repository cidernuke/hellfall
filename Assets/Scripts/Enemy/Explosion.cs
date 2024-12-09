using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float explosionDelay = 2f; // Timer before explosion
    private Animator animator;
    private bool hasExploded = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerExplosion()
    {
        Invoke(nameof(Explode), explosionDelay);
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Trigger the explosion animation
        animator.SetTrigger("Explode");

        // Start the explosion sequence
        StartCoroutine(ExplosionSequence());
    }

    private System.Collections.IEnumerator ExplosionSequence()
    {
        // Damage all objects within the explosion radius
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

        // Wait for the animation to finish
        float animationLength = GetAnimationLength("Explosion"); // Replace with your animation name
        yield return new WaitForSeconds(animationLength);

        // Destroy the game object after the animation finishes
        animator.SetTrigger("Death");
        Destroy(gameObject);
    }

    private float GetAnimationLength(string animationName)
    {
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
}
