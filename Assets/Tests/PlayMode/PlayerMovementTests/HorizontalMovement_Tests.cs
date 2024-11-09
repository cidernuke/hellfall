using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MovementTests
{

    private static bool sceneLoaded = false;
    private GameObject player;
    private PlayerMovement playerMovement;
    private MockPlayerInput mockInput;
    //private Animator animator;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Load scene only once
        if (!sceneLoaded)
        {
            SceneManager.LoadScene("TestScene");
            yield return null; // Wait for scene being loaded
            sceneLoaded = true;
        }

        // Initialize the Player
        player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Player GameObject not found in the scene.");

        // animator = player.GetComponent<Animator>();
        // Assert.IsNotNull(animator, "Animator component not found on the Player GameObject.");

        // Initialize the Components
        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement, "PlayerMovement component not found on the Player GameObject.");

        // Initialize the Mock Input
        mockInput = new MockPlayerInput();
        playerMovement.playerInput = mockInput;

        //Wait for the Player to drop on the Ground
        yield return new WaitForSeconds(1f);
    }

    [UnityTest]
    public IEnumerator HorizontalMovement()
    {
        // Ensure the player is grounded
        playerMovement.grounded = true;

        // Start with horizontal position
        var initialPositionX = player.transform.position.x;

        // Simulate horizontal input to the right
        mockInput.horizontalInput = 1f;

        // Call Update to process the input
        playerMovement.Update();

        // Wait for FixedUpdate to process physics and update the Animator
        yield return new WaitForFixedUpdate();

        // Wait to give the player time to move
        yield return new WaitForSeconds(0.5f);

        // Reset the horizontal input
        mockInput.horizontalInput = 0f;

        // Capture the new horizontal position
        var newPositionX = player.transform.position.x;

        // Check if the horizontal position has moved to the right
        Assert.Greater(newPositionX, initialPositionX, "Player should have moved to the right.");

        // Repeat the test for movement to the left
        initialPositionX = newPositionX;

        mockInput.horizontalInput = -1f; // Move to the left
        playerMovement.Update();
        yield return new WaitForFixedUpdate();
        yield return null;
        yield return new WaitForSeconds(0.5f);
        mockInput.horizontalInput = 0f;
        newPositionX = player.transform.position.x;
        Assert.Less(newPositionX, initialPositionX, "Player should have moved to the left.");
        yield return null;
    }
}