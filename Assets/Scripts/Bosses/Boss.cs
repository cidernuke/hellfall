using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	public bool isFlipped = false;
	[SerializeField] private Transform firePoint;
	[SerializeField] private GameObject[] projectiles;
	private Animator animator;

    private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void LookAtPlayer(Transform player)
	{
		Vector3 flipped = transform.localScale;

		//* boss is left of player
		if (transform.position.x < player.position.x && isFlipped)
		{
			flipped.x *= -1f;
			transform.localScale = flipped;
			// transform.Rotate(0f, 180f, 0f);
			isFlipped = false;
		}
		//* boss is right of player
		else if (transform.position.x > player.position.x && !isFlipped)
		{
			flipped.x *= -1f;
			transform.localScale = flipped;
			// transform.Rotate(0f, 180f, 0f);
			isFlipped = true;
		}
	}

	/// <summary>
	/// Finds an inactive projectile in the array and returns its index.
	/// </summary>
	/// <returns></returns>
	private int FindProjectile()
	{
		for (int i = 0; i < projectiles.Length; i++)
		{
			if (!projectiles[i].activeInHierarchy)
			{
				return i;
			}
		}
		return 0;
	}

	/// <summary>
	/// Triggers the ranged attack animation and resets the cooldown timer.
	/// Spawns a projectile and sets the direction.
	/// </summary>
	private void AttackRanged()
	{
		// cooldownTimer = 0;
		int projectileIndex = FindProjectile();
		projectiles[projectileIndex].transform.position = firePoint.position;
		projectiles[projectileIndex].GetComponent<Projectile>().SetDirection(Math.Sign(transform.localScale.x));

		

	}

}