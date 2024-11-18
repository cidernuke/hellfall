using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class DashMovement_Tests: PlayerMovement_Test_SetUp
{
    [UnityTest]
    public IEnumerator BasicDashTest()
    {
        //var initialPositionX = player.transform.position.x;

        // Simulate horizontal input to the right
        mockInput.horizontalInput = 1f;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();

        // Record position after starting dash
        var positionXAfterDashStart = player.transform.position.x;

        // Wait  to allow the player to start moving
        yield return new WaitForSeconds(0.1f);

        // Simulate dash input
        mockInput.dashInput = true;
        playerMovement.Update();
        yield return null; // Wait a frame to process the input
        mockInput.dashInput = false; // Ensure dash input is only active for one frame

        // Check that the player has started dashing
        Assert.IsTrue(playerMovement.IsDashing, "Player should be dashing after dash input.");

        // Wait for the dash duration to elapse
        //yield return new WaitForSeconds(playerMovement.dashDuration + 0.1f);
        yield return new WaitForSeconds(1f);

        // Check that the player has stopped dashing
        Assert.IsFalse(playerMovement.IsDashing, "Player should have stopped dashing after dash duration.");

        // Record position after dash ends
        var positionXAfterDashEnd = player.transform.position.x;

        // Ensure that the player has moved significantly during the dash
        Assert.Greater(positionXAfterDashEnd, positionXAfterDashStart + 1f, "Player should have moved significantly during dash.");

        mockInput.horizontalInput = 0f;

        yield return null;
    }

    // [UnityTest]
    // public IEnumerator DashDurationTest()
    // {
    //     // Set horizontal input to simulate movement direction
    //     mockInput.horizontalInput = 1f;

    //     // Start the dash
    //     mockInput.dashInput = true;
    //     playerMovement.Update();
    //     yield return null; // Process the input
    //     mockInput.dashInput = false; // Reset dash input

    //     // Verify that the player has started dashing
    //     Assert.IsTrue(playerMovement.IsDashing, "Player should be dashing after dash input.");

    //     // Calculate the number of frames the dash should last
    //     int dashFrames = Mathf.CeilToInt(playerMovement.DashDuration / Time.fixedDeltaTime);

    //     // Advance frames while checking that the player is still dashing
    //     for (int i = 0; i < dashFrames; i++)
    //     {
    //         yield return new WaitForFixedUpdate(); // Advance one physics frame
    //         Assert.IsTrue(playerMovement.IsDashing, $"Player should still be dashing at frame {i + 1}.");
    //     }

    //     // After the expected dash duration, the player should stop dashing
    //     yield return new WaitForFixedUpdate(); // Advance one more frame
    //     Assert.IsFalse(playerMovement.IsDashing, "Player should have stopped dashing after dash duration.");

    //     // Check the actual dash duration
    //     float expectedDuration = playerMovement.DashDuration;
    //     float actualDuration = dashFrames * Time.fixedDeltaTime;

    //     // Allow a small margin of error
    //     float allowedError = 0.01f;

    //     Assert.IsTrue(Mathf.Abs(actualDuration - expectedDuration) <= allowedError,
    //         $"Dash duration should be approximately {expectedDuration}s, but was {actualDuration}s.");

    //     yield return new WaitForSeconds(1f);

    //     // Reset horizontal input
    //     mockInput.horizontalInput = 0f;
    //     yield return null;
    // }

    // [UnityTest]
    // public IEnumerator DashCooldownTest()
    // {
    //     // Start with initial dash
    //     mockInput.horizontalInput = 1f;  // Set direction for dash
    //     mockInput.dashInput = true;
    //     playerMovement.Update();
    //     yield return new WaitForFixedUpdate();
    //     Assert.IsTrue(playerMovement.IsDashing, "Player should be dashing initially.");

    //     // Wait for dash to complete
    //     yield return new WaitForSeconds(playerMovement.DashDuration + 0.1f);
    //     Assert.IsFalse(playerMovement.IsDashing, "Player should have stopped dashing after dash duration.");

    //     // Try to dash again immediately, which should fail due to cooldown
    //     mockInput.dashInput = true;
    //     playerMovement.Update();
    //     yield return new WaitForFixedUpdate();
    //     Assert.IsFalse(playerMovement.IsDashing, "Player should not be able to dash during cooldown.");

    //     // Wait for cooldown to complete
    //     yield return new WaitForSeconds(playerMovement.DashCooldown);

    //     // Try to dash again after cooldown
    //     mockInput.dashInput = true;
    //     playerMovement.Update();
    //     yield return new WaitForFixedUpdate();
    //     Assert.IsTrue(playerMovement.IsDashing, "Player should be able to dash again after cooldown.");

    //     // Reset inputs
    //     mockInput.dashInput = false;
    //     mockInput.horizontalInput = 0f;
    //     yield return new WaitForSeconds(playerMovement.DashDuration + 1f);
    //     yield return null;
    // }

    [UnityTest]
    public IEnumerator DashDirectionTest()
    {
        // Ensure the player is grounded
        //playerMovement.grounded = true;

        // Record the initial position
        var initialPositionX = player.transform.position.x;

        // Simulate horizontal input to the right
        mockInput.horizontalInput = 1f;

        // Simulate dash input
        mockInput.dashInput = true;
        playerMovement.Update();
        yield return null; // Process the input

        mockInput.dashInput = false; // Reset dash input

        // Wait for the dash duration to elapse
        yield return new WaitForSeconds(playerMovement.DashDuration + 0.1f);

        mockInput.horizontalInput = 0f;

        // Record the final position
        var finalPositionX = player.transform.position.x;

        // Verify that the player has moved to the right
        Assert.Greater(finalPositionX, initialPositionX + 0.5f, "Player should have dashed to the right.");
        yield return new WaitForSeconds(1f);


        //Dash to the left:
        initialPositionX = finalPositionX;

        // Simulate horizontal input to the left
        mockInput.horizontalInput = -1f;

        // Simulate dash input
        mockInput.dashInput = true;
        playerMovement.Update();
        yield return null; // Process the input

        mockInput.dashInput = false; // Reset dash input

        // Wait for the dash duration to elapse
        yield return new WaitForSeconds(playerMovement.DashDuration + 0.1f);

        // Record the final position
        finalPositionX = player.transform.position.x;

        // Verify that the player has moved to the left
        Assert.Less(finalPositionX, initialPositionX - 0.5f, "Player should have dashed to the left.");
        yield return new WaitForSeconds(1f);

        mockInput.horizontalInput = 0f;
        yield return null;
    }
    [UnityTest]
    public IEnumerator DashNotActivated_WhenNoHorizontalInput()
    {
        // Ensure the player is grounded
        //playerMovement.grounded = true;

        // Record the initial position
        var initialPositionX = player.transform.position.x;

        // Set horizontal input to zero
        mockInput.horizontalInput = 0f;

        // Simulate dash input
        mockInput.dashInput = true;
        playerMovement.Update();
        yield return null; // Process the input

        mockInput.dashInput = false; // Reset dash input

        // Verify that the dash was not activated
        Assert.IsFalse(playerMovement.IsDashing, "Player should not dash when horizontal input is zero.");

        // Wait for a short duration to confirm no movement occurs
        yield return new WaitForSeconds(0.5f);

        // Record the final position
        var finalPositionX = player.transform.position.x;

        // Verify that the player's position hasn't changed
        Assert.AreEqual(initialPositionX, finalPositionX, "Player should not have moved horizontally when dash input is pressed without horizontal input.");

        // Reset inputs
        yield return null;
    }

    [UnityTest]
    public IEnumerator DashWhileAirborne()
    {

        //player.transform.position = new Vector3(player.transform.position.x, 5f, player.transform.position.z);

        // Record the initial position
        var initialPositionX = player.transform.position.x;

        // Ensure the player is not grounded (simulate jumping)
        mockInput.jumpInput = true;
        //playerMovement.grounded = false;

        yield return new WaitForSeconds(0.1f);

        // Simulate horizontal input to the right
        mockInput.horizontalInput = 1f;

        // Simulate dash input
        mockInput.dashInput = true;
        playerMovement.Update();
        yield return null; // Process the input

        mockInput.dashInput = false; // Reset dash input
        mockInput.jumpInput = false; // Reset jump input

        // Verify that the player has started dashing
        Assert.IsTrue(playerMovement.IsDashing, "Player should be dashing while airborne.");

        // Wait for the dash duration
        //yield return new WaitForSeconds(playerMovement.dashDuration + 0.1f);
        yield return new WaitForSeconds(1f);

        // Verify that the player has stopped dashing
        Assert.IsFalse(playerMovement.IsDashing, "Player should have stopped dashing after dash duration.");

        // Verify that the player has moved horizontally
        var finalPositionX = player.transform.position.x;
        Assert.Greater(finalPositionX, initialPositionX + 0.5f, "Player should have moved horizontally while airborne.");

        // Reset inputs
        mockInput.horizontalInput = 0f;
        yield return null;
    }
}