using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	[SerializeField] private Transform firePoint;
	private Transform player;
	[SerializeField] private GameObject[] projectilesForward;
	[SerializeField] private GameObject[] projectilesUpward;
	[SerializeField] private GameObject[] projectilesDownward;
	[SerializeField] private int projectilesCount;

	public float upwardAttackCooldown;
	[HideInInspector] public float cooldownTimer = Mathf.Infinity;
	private readonly float groundCoordinates = 29.7f; // Spawn coordinates for blood_bullets_up
	[HideInInspector] public bool isFlipped = false;
	public bool isInSecondPhase = false;

	public BossHealthBar healthBar;
	// public GameObject healthBarUI;
    public HealthSystem healthSystem;

	private void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		healthSystem = gameObject.GetComponent<HealthSystem>();

        healthBar.SetMaxHealth((int)healthSystem.startingHealth);
	}

	private void Update()
	{
		cooldownTimer += Time.deltaTime;

		if (healthSystem.currentHealth < healthSystem.startingHealth)
        {
            healthBar.SetHealth((int)healthSystem.currentHealth);
        }

		if (gameObject.GetComponent<HealthSystem>().currentHealth == 10)
		{
			isInSecondPhase = true;
		}
	}

	/// <summary>
	/// Flips the boss sprite to face the player.
	/// </summary>
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
	/// Finds the index of the active projectile in the array.
	/// </summary>
	private int FindActiveProjectile()
	{
		for (int i = 0; i < projectilesForward.Length; i++)
		{
			if (projectilesForward[i].activeInHierarchy)
			{
				return i;
			}
		}
		return 0;
	}

	/// <summary>
	/// Finds n projectiles that are currently not in use.
	/// </summary>
	/// <returns></returns>
	private List<GameObject> FindProjectilesUpward(int n, GameObject[] projectiles)
	{
		List<GameObject> usableProjectiles = new();
		foreach (var projectile in projectiles)
		{
			if (!projectile.activeInHierarchy)
			{
				usableProjectiles.Add(projectile);

				// Stop searching once we've found n projectiles.
				if (usableProjectiles.Count == n)
				{
					break;
				}
			}
		}

		return usableProjectiles;
	}

	/// <summary>
	/// Executes the ranged-froward attack of the vampire countess boss.
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
	/// Executes the upward attack of the vampire countess boss.
	/// Called by animation event in attack_02.
	/// </summary>
	private void AttackUpwards()
	{
		List<GameObject> availableProjectiles = FindProjectilesUpward(projectilesCount, projectilesUpward);
		int count = availableProjectiles.Count;

		if (count == 0)
		{
			Debug.LogWarning("No projectiles available for upward attack.");
			return;
		}

		// Set spacing based on the number of projectiles
		float spacing = 0.3f;
		float totalWidth = spacing * (count - 1); // Total width covered by projectiles
		Vector3 playerPositionLocal = player.position;
		float startX = playerPositionLocal.x - totalWidth / 2f; // Center the projectiles around the player

		float globalDelay = 0.5f; // Delay before all projectiles start moving. Gives the player a chance to register the attack
		// Loop through projectiles and position them dynamically
		for (int i = 0; i < count; i++)
		{
			float xPosition = startX + i * spacing;
			availableProjectiles[i].transform.position = new Vector2(xPosition, groundCoordinates);

			availableProjectiles[i].GetComponent<Projectile>().SetDirection(new Vector2(0, 1), globalDelay);
		}
	}

	/* 
	When hit, dont play hit anim, but rather color the boss white for miliseconds --> boss still attacks
	*/

	/// <summary>
	/// Executes the downward attack of the vampire countess boss.
	/// Called by animation event in attack_02.5
	/// </summary>
	private void AttackDownwards()
	{
		List<GameObject> availableProjectiles = FindProjectilesUpward(projectilesCount, projectilesDownward);
		int count = availableProjectiles.Count;

		if (count == 0)
		{
			Debug.LogWarning("No projectiles available for upward attack.");
			return;
		}

		// Set spacing based on the number of projectiles
		float spacing = 0.3f;
		float totalWidth = spacing * (count - 1); // Total width covered by projectiles
		Vector3 playerPositionLocal = player.position;
		float startX = playerPositionLocal.x - totalWidth / 2f; // Center the projectiles around the player
		float startY = 36f; // Hardcoded, as its only for this boss

		// Loop through projectiles and position them dynamically
		for (int i = 0; i < count; i++)
		{
			float xPosition = startX + i * spacing;
			availableProjectiles[i].transform.position = new Vector2(xPosition, startY);

			availableProjectiles[i].GetComponent<Projectile>().SetDirection(new Vector2(0, -1));
		}
	}

	/// <summary>
	/// Called by the HealthSystem when the boss dies.
	/// </summary>
	public void OnDeath()
	{
		healthBar.gameObject.SetActive(false);
		print("healthbar should deactivate");
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Prints a message to the console.
	/// </summary>
	public void PrinterForBossRun(string message)
	{
		print(message);
	}
}