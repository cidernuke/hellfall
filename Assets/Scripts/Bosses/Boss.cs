using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	public bool isFlipped = false;
	[SerializeField] private Transform firePoint;
	[SerializeField] private GameObject[] projectilesForward;
	[SerializeField] private GameObject[] projectilesUpward;
	private Transform player;
	private Vector3 playerPosition;
	private readonly float groundCoordinates = 30.25f;
    [SerializeField] private readonly float upwardAttackCooldown = 3f;
    private float cooldownTimer = Mathf.Infinity;

	private void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	private void Update()
	{
		cooldownTimer += Time.deltaTime;
	}

	public void LookAtPlayer(Transform player)
	{
		Vector3 flipped = transform.localScale;

		// boss is left of player
		if (transform.position.x < player.position.x && isFlipped)
		{
			flipped.x *= -1f;
			transform.localScale = flipped;
			isFlipped = false;
		}
		// boss is right of player
		else if (transform.position.x > player.position.x && !isFlipped)
		{
			flipped.x *= -1f;
			transform.localScale = flipped;
			isFlipped = true;
		}
	}

	/// <summary>
	/// Finds an inactive projectile in the array and returns its index.
	/// </summary>
	/// <returns></returns>
	private int FindProjectile()
	{
		for (int i = 0; i < projectilesForward.Length; i++)
		{
			if (!projectilesForward[i].activeInHierarchy)
			{
				return i;
			}
		}
		return 0;
	}

	/// <summary>
	/// Finds n projectiles that are currently not in use
	/// </summary>
	/// <returns></returns>
	private List<GameObject> FindProjectilesUpward(int n)
	{
		List<GameObject> usableProjectilesUpward = new();
		for (int i = 0; i < n; i++)
		{
			if (!projectilesUpward[i].activeInHierarchy)
			{
				usableProjectilesUpward.Add(projectilesUpward[i]);
			}
		}

		return usableProjectilesUpward;
	}

	/// <summary>
	/// Executes the ranged attack of the vampire countess boss
	/// Spawns a projectile and sets the direction.
	/// Called by animation event in attack_01
	/// </summary>
	private void AttackRanged()
	{
		int projectileIndex = FindProjectile();
		projectilesForward[projectileIndex].transform.position = firePoint.position;

		int directionX = Math.Sign(transform.localScale.x);
		projectilesForward[projectileIndex].GetComponent<Projectile>().SetDirection(new Vector2(directionX, 0));
	}

	/// <summary>
	/// Executes the upward attack of the vampire countess boss
	/// Called by animation event in attack_02
	/// </summary>
	private void AttackUpwards()
	{
		if (cooldownTimer < upwardAttackCooldown)
		{
			return;
		}
		List<GameObject> availableProjectiles = FindProjectilesUpward(5);

		GameObject projectile_1 = availableProjectiles[0];
		GameObject projectile_2 = availableProjectiles[1];
		GameObject projectile_3 = availableProjectiles[2];
		GameObject projectile_4 = availableProjectiles[3];
		GameObject projectile_5 = availableProjectiles[4];

		projectile_1.transform.position = new Vector2(playerPosition.x - 0.5f, groundCoordinates);
		projectile_2.transform.position = new Vector2(playerPosition.x - 0.25f, groundCoordinates);
		projectile_3.transform.position = new Vector2(playerPosition.x, groundCoordinates);
		projectile_4.transform.position = new Vector2(playerPosition.x + 0.25f, groundCoordinates);
		projectile_5.transform.position = new Vector2(playerPosition.x + 0.5f, groundCoordinates);

		int directionY = Math.Sign(transform.localScale.y);
		projectile_1.GetComponent<Projectile>().SetDirection(new Vector2(0, directionY));
		projectile_2.GetComponent<Projectile>().SetDirection(new Vector2(0, directionY));
		projectile_3.GetComponent<Projectile>().SetDirection(new Vector2(0, directionY));
		projectile_4.GetComponent<Projectile>().SetDirection(new Vector2(0, directionY));
		projectile_5.GetComponent<Projectile>().SetDirection(new Vector2(0, directionY));

		cooldownTimer = 0;
	}

	/// <summary>
	/// Grabs the current player position
	/// Called by animation event in attack_02
	/// </summary>
	private void GrabPlayerPosition()
	{
		playerPosition = player.position;
	}

}