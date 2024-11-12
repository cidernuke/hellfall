using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Moq; //very important!!

public class HealthSystemTest
{
    private GameObject player;
    private HealthSystem playerHealthSystem;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Load the test scene
        SceneManager.LoadScene("Sample_Scene_DamageZone");
        yield return null; // Wait for the scene to load

        // Find the player GameObject
        player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Player GameObject not found in the scene.");

        // Get the PlayerMovement component
        playerHealthSystem = player.GetComponent<HealthSystem>();
        Assert.IsNotNull(playerHealthSystem, "playerHealthSystem component not found on the Player GameObject.");

        // Wait for the player to settle on the ground (if necessary)
        yield return new WaitForSeconds(1f);
    }

        [UnityTest]
        public IEnumerator PlayerTakesDamage()
        {
                //Ensure the player is not invincible
                playerHealthSystem.isInvincible = false;

                // Get the initial health of the player
                float initialHealth = playerHealthSystem.currentHealth;

                //Let Player take damage
                playerHealthSystem.TakeDamage(10);

                playerHealthSystem.Update();

                // Wait for 0.2 second
                yield return new WaitForSeconds(0.2f);

                // Assert that the player's health has decreased
                Assert.Greater(playerHealthSystem.currentHealth, initialHealth);

                // Assert that the player's health is now at 90
                Assert.Equals(playerHealthSystem.currentHealth, 90);
        }
}
