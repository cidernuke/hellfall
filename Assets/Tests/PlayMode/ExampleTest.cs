using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Moq; //very important!!

public class ExampleTest
{
   [UnityTest]
    public IEnumerator TestExample()
    {
        // Load the scene
        SceneManager.LoadScene("SampleScene");

        // Wait until the scene is loaded
        yield return null; // Wait for one frame to ensure the scene is loaded

        // Find the (player) GameObject
        GameObject player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Player GameObject not found in the scene.");

        // Get the (PlayerMovement) component
        var playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement, "PlayerMovement component not found on the Player GameObject.");

        // Create a mock input and assign it to the playerMovement
        var mockInput = new MockPlayerInput();
        playerMovement.playerInput = mockInput;

        // Ensure the player is grounded
        playerMovement.grounded = true;

        // Get the initial (vertical) position --> Depends on what you want to test
        var initialPositionY = player.transform.position.y;

        // Simulate jump input by setting the jumpInput property to true
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
