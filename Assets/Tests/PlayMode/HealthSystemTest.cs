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
    //for setup
    private bool sceneLoaded = false;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (!sceneLoaded)
        {
            // Load scene
            SceneManager.LoadScene("Sample_Scene_DamageZone");
            // Wait for scene being loaded
            yield return null; 
            sceneLoaded = true;
        }
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
                Assert.Less(playerHealthSystem.currentHealth, initialHealth,"Player health did not decrease as expected.");

                // Assert that the player's health is now at 90
                Assert.AreEqual(90, playerHealthSystem.currentHealth);
        }
}
