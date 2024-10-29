using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Moq;

public class CharacterTests
{
   [UnityTest]
   public IEnumerator PlayerCanJump()
   {
        // Load the scene
        SceneManager.LoadScene("SampleScene");
        // Wait for 3 seconds to allow the player to load and hit the ground
        yield return new WaitForSeconds(3);
        // Find the player GameObject
        GameObject player = GameObject.Find("Player");

        // Get the PlayerMovment component attached to the player GameObject
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

        // Get the initial position of the player
        Vector3 initialPosition = player.transform.position;

        //set grounded = true and crouching = false, so the player can jump
        playerMovement.grounded = true;
        playerMovement.isCrouching = false;

        // Mock the input to simulate jump input
        var mockPlayerMovement = new Mock<PlayerMovement>();
        mockPlayerMovement.Setup(m => m.GetJumpInput()).Returns(true);

        // Make the player jump
        playerMovement.HandleJumpInput();

        // Wait for 1 second to allow the jump to occur
        yield return new WaitForSeconds(0.2f);

        // Assert that the player's y position has increased, indicating a jump
        Assert.Greater(player.transform.position.y, initialPosition.y);

   }
   /*[UnityTest]
    public IEnumerator PlayerMovesLeftWhenAPressed()
    {
        // Load the scene
        SceneManager.LoadScene("SampleScene");
        // Wait for 3 seconds to allow the player to load and hit the ground
        yield return new WaitForSeconds(3);

        // Find the player GameObject
        GameObject player = GameObject.Find("Player");
        PlayerMovment playerMovment = player.GetComponent<PlayerMovment>();

        // Get the initial position of the player
        Vector3 initialPosition = player.transform.position;

        // Simulate "A" key press
        var keyboard = InputSystem.AddDevice<Keyboard>();
        Press(keyboard.aKey);
        // Wait for 0.5 seconds to allow the player to move
        yield return new WaitForSeconds(0.5f);
        Release(keyboard.aKey);

        // Wait for a frame
        yield return null;

        // Get the new position of the player
        Vector3 newPosition = player.transform.position;

        // Assert that the new position is to the left of the initial position
        Assert.Less(newPosition.x, initialPosition.x);
    }
   [UnityTest]
   public IEnumerator PlayerMovesRightWhenDPressed()
   {
        // Load the scene
        SceneManager.LoadScene("SampleScene");
        // Wait for 3 seconds to allow the player to load and hit the ground
        yield return new WaitForSeconds(3);

        // Find the player GameObject
        GameObject player = GameObject.Find("Player");
        PlayerMovment playerMovment = player.GetComponent<PlayerMovment>();

        // Get the initial position of the player
        Vector3 initialPosition = player.transform.position;

        // Simulate "D" key press
        var keyboard = InputSystem.AddDevice<Keyboard>();
        Press(keyboard.dKey);
        // Wait for 0.5 seconds to allow the player to move
        yield return new WaitForSeconds(0.5f);
        Release(keyboard.dKey);

        // Wait for a frame
        yield return null;

        // Get the new position of the player
        Vector3 newPosition = player.transform.position;

        // Assert that the new position is to the right of the initial position
        Assert.Greater(newPosition.x, initialPosition.x);
    }*/
}
