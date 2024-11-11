using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class CrouchMovement_Tests
{
    private static bool sceneLoaded = false;
    private GameObject player;
    private PlayerMovement playerMovement;
    private MockPlayerInput mockInput;

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

        // Initialize the Components
        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement, "PlayerMovement component not found on the Player GameObject.");

        // Initialize the Mock-Input
        mockInput = new MockPlayerInput();
        playerMovement.playerInput = mockInput;

        //Wait for the Player to drop on the Ground
        yield return new WaitForSeconds(1f);
    }

    [UnityTest]
    public IEnumerator CrouchChangesColliderSize()
    {
        // Ensure the player is grounded
        playerMovement.grounded = true;

        // Start with the initial height of the collider
        var initialColliderSize = playerMovement.GetComponent<BoxCollider2D>().size;
        var initialColliderOffset = playerMovement.GetComponent<BoxCollider2D>().offset;

        // Simulate crouch input
        mockInput.crouchInput = true;

        playerMovement.Update();

        // Call FixedUpdate to process physics and update the collider
        yield return new WaitForFixedUpdate();

         // Check if the player is in crouch mode
        Assert.IsTrue(playerMovement.isCrouching, "Player should be crouching.");

        // Check if the collider size and sprite have changed correctly
        var crouchedColliderSize = playerMovement.GetComponent<BoxCollider2D>().size;
        var crouchedColliderOffset = playerMovement.GetComponent<BoxCollider2D>().offset;
        Assert.AreNotEqual(initialColliderSize, crouchedColliderSize, "Collider size should be changed when crouching.");
        Assert.AreNotEqual(initialColliderOffset, crouchedColliderOffset, "Collider offset should be changed when crouching.");

        mockInput.crouchInput = false;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();

        // Check if the player is no longer crouching
        Assert.IsFalse(playerMovement.isCrouching, "Player should not be crouching anymore.");

        // Check if the collider size and sprite have been reset to their original values
        var finalColliderSize = playerMovement.GetComponent<BoxCollider2D>().size;
        var finalColliderOffset = playerMovement.GetComponent<BoxCollider2D>().offset;
        Assert.AreEqual(initialColliderSize, finalColliderSize, "Collider size should be reset after uncrouching.");
        Assert.AreEqual(initialColliderOffset, finalColliderOffset, "Collider offset should be reset after uncrouching.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator CrouchWalking()
    {
        // Ensure the player is grounded
        playerMovement.grounded = true;

        // Start with the initial horizontal position
        var initialPositionX = player.transform.position.x;

        // Simulate crouch input
        mockInput.crouchInput = true;

        // Simulate horizontal input to the right
        mockInput.horizontalInput = 1f;

        playerMovement.Update();
        yield return new WaitForFixedUpdate();

        // Wait briefly to give the player time to move
        yield return new WaitForSeconds(0.5f);

        // Reset the horizontal input
        mockInput.horizontalInput = 0f;

        // Capture the new horizontal position
        var newPositionX = player.transform.position.x;

        // Check if the player is in crouch mode
        Assert.IsTrue(playerMovement.isCrouching, "Player should be crouching.");
        // Check if the horizontal position has moved to the right
        Assert.Greater(newPositionX, initialPositionX, "Player should have moved to the right.");

        yield return new WaitForSeconds(0.5f);
        mockInput.crouchInput = true;

        playerMovement.Update();
        yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator CrouchJumping()
    {
        var initialPositionY = player.transform.position.y;
        mockInput.crouchInput = true;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.5f);

        // Attempt to jump while crouching
        mockInput.jumpInput = true;
        playerMovement.Update();

        yield return null; // Jump only for one frame
        mockInput.jumpInput = false; // Immediately deactivate jump input -> Trouble with overwriting or so o_O

        // Wait for physics processing of the jump
        yield return new WaitForFixedUpdate();

        var positionYAfterJump = player.transform.position.y;
        //yield return new WaitForSeconds(2f);
        Assert.Greater(positionYAfterJump, initialPositionY, "Player should be able to jump while crouching");
        Assert.IsTrue(playerMovement.isCrouching, "Player should crouch while jumping");

        // Reset inputs
        //mockInput.jumpInput = false;
        mockInput.crouchInput = false;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.5f);

        // Ensure the player is back in the standing position
        var finalPositionY = player.transform.position.y;
        Assert.GreaterOrEqual(finalPositionY, positionYAfterJump, "Player should not be in crouch position anymore.");

        yield return new WaitForSeconds(1f);
        yield return null;

    }

    [UnityTest]
    public IEnumerator Crouching_DoesNotAllow_Dash()
    {
        playerMovement.grounded = true;

        // Simulate crouch input to enter crouch mode
        mockInput.crouchInput = true;
        // Simuliere horizontalen Input nach rechts
        mockInput.horizontalInput = 1f;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();

        // Short wait to ensure crouch state is active
        yield return new WaitForSeconds(0.5f);

        mockInput.dashInput = true;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();

        //Ensure player is crouching
        Assert.IsTrue(playerMovement.isCrouching, "Player should  be crouching.");
        // Check if player is not dashing (isDashing should be false)
        Assert.IsFalse(playerMovement.isDashing, "Player should not be able to dash while crouching.");

        // Reset inputs
        mockInput.dashInput = false;
        mockInput.crouchInput = false;
        mockInput.horizontalInput = 0f;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();
    }
}