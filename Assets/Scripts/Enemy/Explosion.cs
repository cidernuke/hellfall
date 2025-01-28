using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float triggerRadius = 3f;  // The radius within which the explosion is triggered
    [SerializeField] private float explosionRadius = 1f; // The radius in which damage is dealt by the explosion
    [SerializeField] private int damageAmount = 20; // The amount of damage dealt to the player and enemies
    [SerializeField] private LayerMask playerLayer; // The layer mask to detect the player in the trigger radius

    private Animator animator;  // Animator for handling explosion and death animations
    private bool hasExploded = false;  // Flag to check if the explosion has already occurred
    private bool damageDealt = false;  // Flag to ensure damage is dealt only once during the explosion

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();  // Get the animator component attached to this object
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if player is in the trigger radius
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, triggerRadius, playerLayer);

        if (playerCollider != null && !hasExploded) // If player enters trigger and explosion hasn't occurred
        {
            TriggerExplosion();  // Trigger explosion
        }
    }

    // This method is called to initiate the explosion
    public void TriggerExplosion()
    {
        if (hasExploded) return;  // If explosion has already occurred, exit

        hasExploded = true;  // Mark the explosion as triggered

        if (animator != null)
        {
            animator.SetTrigger("Explode");  // Trigger the explosion animation
        }

        StartCoroutine(ExplosionSequence());  // Start the explosion sequence
    }

    // The explosion sequence involves dealing damage, playing animations, and destroying the object
    private IEnumerator ExplosionSequence()
    {
        float animationLength = GetAnimationLength("Explosion");  // Get the length of the explosion animation
        yield return new WaitForSeconds(animationLength / 2);  // Wait for half of the explosion animation

        if (!damageDealt)  // If damage hasn't been dealt
        {
            DealDamage();  // Deal damage to nearby objects (player and enemies)
            damageDealt = true;  // Mark that damage has been dealt
        }

        yield return new WaitForSeconds(animationLength / 2);  // Wait for the rest of the explosion animation

        if (animator != null)
        {
            animator.SetTrigger("die");  // Trigger the death animation of the object
        }

        yield return new WaitForSeconds(0.2f);  // Wait for the death animation to play

        Destroy(gameObject);  // Destroy the object after explosion
    }

    // Deals damage to all objects within the explosion radius
    private void DealDamage()
    {
        // Get all colliders within the explosion radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D collider in hitColliders)
        {
            // If the collider is the player, deal damage to the player
            if (collider.CompareTag("Player"))
            {
                HealthSystem healthSystem = collider.GetComponent<HealthSystem>();
                PlayerMovement playerMovement = collider.GetComponent<PlayerMovement>();

                if (healthSystem != null && playerMovement != null)
                {
                    healthSystem.TakeDamage(damageAmount, playerMovement);
                }
                else
                {
                    Debug.LogError("HealthSystem or PlayerMovement component not found on player!");
                }
            }



            // If the collider is an enemy, deal damage to the enemy
            // commented so no enemy damage
            //! bug for 2-key-drop is here
            // else if (collider.CompareTag("Enemy"))
            // {
            //     collider.GetComponent<HealthSystem>()?.TakeDamage(damageAmount);
            // }
        }
    }

    // Retrieves the length of the specified animation clip by name
    private float GetAnimationLength(string animationName)
    {
        if (animator.runtimeAnimatorController == null) return 1f;  // Return default length if no controller is set

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;  // Get all animation clips
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)  // If the clip matches the animation name
            {
                return clip.length;  // Return the length of the animation
            }
        }
        return 1f;  // Return default length if the animation is not found
    }

    // Draw gizmos in the editor to show the trigger and explosion radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;  // Set color for trigger radius
        Gizmos.DrawWireSphere(transform.position, triggerRadius);  // Draw trigger radius

        Gizmos.color = Color.red;  // Set color for explosion radius
        Gizmos.DrawWireSphere(transform.position, explosionRadius);  // Draw explosion radius
    }
}
