using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class Example_Test
{

    private static bool sceneLoaded = false;
    private GameObject player;
    private PlayerMovement playerMovement;
    private MockPlayerInput mockInput;

    //[UnitySetUp] is kind of like before each
    [UnitySetUp]
    //SetUp can be copyed and reused, maybe you just need to change the scene
    //If there are any problems checkout: https://app.clickup.com/9012367417/v/dc/8cjvm1t-112
    public IEnumerator SetUp()
    {
        // Load scene only once
        if (!sceneLoaded)
        {
            // Load scene
            SceneManager.LoadScene("TestScene");

            // Wait for scene being loaded
            yield return null; 
            sceneLoaded = true;
        }

        // Initialize the Player
        player = GameObject.Find("Player");
        //Ensure the Object is not Null to avoid Exceptions
        Assert.IsNotNull(player, "Player GameObject not found in the scene.");

        // Initialize the Components
        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement, "PlayerMovement component not found on the Player GameObject.");

        // Initialize the Mock Input
        mockInput = new MockPlayerInput();
        playerMovement.playerInput = mockInput;

        //Wait for the Player to drop on the Ground 
        //-> Just needed because in my Test scene the player doesn't start at the ground
        yield return new WaitForSeconds(1f);
    }

    [UnityTest]
    public IEnumerator HorizontalMovement()
    {
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

        //...

        yield return null;
    }
}