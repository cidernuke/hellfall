using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour
{
	[HideInInspector] public bool isFlipped = false;
	[SerializeField] private Transform firePoint;
	[SerializeField] private GameObject[] projectilesForward;
	[SerializeField] private GameObject[] projectilesUpward;
	[SerializeField] private GameObject[] projectilesDownward;
	[SerializeField] private int projectilesCount;
	private Transform player;
	private Vector3 playerPosition;
	private readonly float groundCoordinates = 29.7f;
	public float upwardAttackCooldown;
	[HideInInspector] public float cooldownTimer = Mathf.Infinity;
	

	// [HideInInspector] public bool isInSecondPhase = false;
	public bool isInSecondPhase = false;

	private void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	private void Update()
	{
		cooldownTimer += Time.deltaTime;
		// playerPosition = player.position;

		if (gameObject.GetComponent<HealthSystem>().currentHealth == 10)
		{
			isInSecondPhase = true;
		}
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
	private List<GameObject> FindProjectilesUpward(int n, GameObject[] projectiles)
	{
		// print($"projectiles param contains {projectiles.Length} projectiles");
		List<GameObject> usableProjectiles = new();
		foreach (var projectile in projectiles)
		{
			if (!projectile.activeInHierarchy)
			{
				usableProjectiles.Add(projectile);

				// Stop searching once we've found n projectiles.
				if (usableProjectiles.Count == n)
				{
					// print($"found {usableProjectiles.Count} projectiles, exiting function, n is {n}");
					break;
				}
			}
		}

		return usableProjectiles;
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

	//? for second phase: projectiles dissapear and reappear again over the player --> forces double dodge
	/// <summary>
	/// Executes the upward attack of the vampire countess boss
	/// Called by animation event in attack_02
	/// </summary>
	private void AttackUpwards()
	{
		List<GameObject> availableProjectiles = FindProjectilesUpward(projectilesCount, projectilesUpward);
		int count = availableProjectiles.Count;
		// print($"found {count} projectiles");

		if (count == 0)
		{
			Debug.LogWarning("No projectiles available for upward attack.");
			return;
		}

		// Set spacing based on the number of projectiles
		float spacing = 0.3f; // Adjust this value to control how far apart the projectiles are
		float totalWidth = spacing * (count - 1); // Total width covered by projectiles
		Vector3 playerPositionLocal = player.position;
		float startX = playerPositionLocal.x - totalWidth / 2f; // Center the projectiles around the player

		float globalDelay = 0.5f; // Delay before all projectiles start moving
		// Loop through projectiles and position them dynamically
		for (int i = 0; i < count; i++)
		{
			float xPosition = startX + i * spacing;
			availableProjectiles[i].transform.position = new Vector2(xPosition, groundCoordinates);

			// Combine global delay with staggered delay based on index
			availableProjectiles[i].GetComponent<Projectile>().SetDirection(new Vector2(0, 1), globalDelay);
		}
	}

	private void AttackDownwards()
	{
		List<GameObject> availableProjectiles = FindProjectilesUpward(projectilesCount, projectilesDownward);
		int count = availableProjectiles.Count;
		// print($"found {count} projectiles");

		if (count == 0)
		{
			Debug.LogWarning("No projectiles available for upward attack.");
			return;
		}

		// Set spacing based on the number of projectiles
		float spacing = 0.3f; // Adjust this value to control how far apart the projectiles are
		float totalWidth = spacing * (count - 1); // Total width covered by projectiles
		Vector3 playerPositionLocal = player.position;
		float startX = playerPositionLocal.x - totalWidth / 2f; // Center the projectiles around the player
		float startY = 36f;

		// Loop through projectiles and position them dynamically
		for (int i = 0; i < count; i++)
		{
			float xPosition = startX + i * spacing;
			availableProjectiles[i].transform.position = new Vector2(xPosition, startY);

			availableProjectiles[i].GetComponent<Projectile>().SetDirection(new Vector2(0, -1));
		}
	}

	/// <summary>
	/// Grabs the current player position
	/// Called by animation event in attack_02
	/// </summary>
	private Vector3 GrabPlayerPosition()
	{
		// playerPosition = player.position;
		return player.position;
		// print($"grabbed player pos: {playerPosition}");
	}

	public void OnDeath()
	{
		gameObject.SetActive(false);
	}

	public void PrinterForBossRun(string message)
	{
		print(message);
	}
}