using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    /// <summary>
    /// Coroutine to destroy the GameObject after the death animation is complete.
    /// </summary>
    /// <param name="animator">The Animator component of the GameObject.</param>
    /// <param name="gameObjectToDestroy">The GameObject to destroy.</param>
    /// <param name="delay">Additional delay before destroying the GameObject.</param>    
    public static IEnumerator DestroyAfterAnimation(Animator animator, GameObject gameObjectToDestroy, float delay)
    {
        // Wait for the length of the death animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + delay);
        Destroy(gameObjectToDestroy);
    }
}
