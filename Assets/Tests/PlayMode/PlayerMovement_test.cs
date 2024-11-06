using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerMovement_test
{
    [UnityTest]
    public IEnumerator PlayerCanJump()
    {
        // Load the scene
        SceneManager.LoadScene("SampleScene");

        // Wait until the scene is loaded
        yield return new WaitForSeconds(0.1f);

        // Find the player GameObject
        GameObject player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Player GameObject not found in the scene.");

        // Get the PlayerMovement component
        var playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement, "PlayerMovement component not found on the Player GameObject.");

        // Create a mock input and assign it to the playerMovement
        var mockInput = new MockPlayerInput();
        playerMovement.playerInput = mockInput;

        // Ensure the player is grounded
        playerMovement.grounded = true;

        // Get the initial vertical position
        var initialPositionY = player.transform.position.y;

        // Simulate jump input
        mockInput.jumpInput = true;

        // Call Update to process the input
        playerMovement.Update();

        // Wait for FixedUpdate to process physics
        yield return new WaitForFixedUpdate();

        // Reset jump input
        mockInput.jumpInput = false;

        // Wait a few frames to allow the player to move upwards
        yield return new WaitForSeconds(0.2f);

        // Get the new vertical position after jumping
        var newPositionY = player.transform.position.y;

        // Assert that the vertical position has increased due to jump
        Assert.Greater(newPositionY, initialPositionY, "Player should have moved upwards due to jump.");

        yield return null;
    }
}
