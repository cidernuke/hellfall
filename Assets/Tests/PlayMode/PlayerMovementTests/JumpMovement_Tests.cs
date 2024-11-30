using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class JumpMovement_Tests: PlayerMovement_Test_SetUp
{
    [UnityTest]
    public IEnumerator BasicJump()
    {
        // Get the initial vertical position
        var initialPositionY = player.transform.position.y;

        // Simulate jump input by setting the jumpInput property to true
        mockInput.jumpInput = true;

        // Call Update to process the input
        playerMovement.Update();

        yield return null;

        // Wait for FixedUpdate to process physics
        yield return new WaitForFixedUpdate();

        // Reset jump input
        mockInput.jumpInput = false;

        // Wait a few frames to allow the player to move upwards
        yield return new WaitForSeconds(0.5f);

        // Get the new vertical position after jumping
        var newPositionY = player.transform.position.y;

        // Assert that the vertical position has increased due to jump
        Assert.Greater(newPositionY, initialPositionY, "Player should have moved upwards due to jump.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator DoubleJump()
    {
        // //First Jump
        // playerMovement.Grounded = true;
        var initialPositionY = player.transform.position.y;
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();
        mockInput.jumpInput = false;
        yield return new WaitForSeconds(0.4f);

        // Get the new vertical position after jumping
        var afterJumpPositionY = player.transform.position.y;

        // Assert that the vertical position has increased due to jump
        Assert.Greater(afterJumpPositionY, initialPositionY, "Player should have moved upwards due to jump.");

        //Assert that the Player is in the air after jump
        Assert.IsFalse(playerMovement.Grounded, "Player should be in the air after first jump.");

        //Seconde Jump
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();
        mockInput.jumpInput = false;
        yield return new WaitForSeconds(0.4f);
        var afterDoubleJumpPositionY = player.transform.position.y;

        // Assert that the vertical position has increased due to jump
        Assert.Greater(afterDoubleJumpPositionY, afterJumpPositionY, "Player should have moved upwards due to double jump.");
        Assert.IsFalse(playerMovement.Grounded, "Player should be in the air after seconde jump.");

        yield return null;

        //Try a third jump -> Should not be possible
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return new WaitForFixedUpdate();
        mockInput.jumpInput = false;
        yield return new WaitForSeconds(1f);
        var afterThirdJumpPositionY = player.transform.position.y;

        Assert.LessOrEqual(afterThirdJumpPositionY, afterDoubleJumpPositionY, "Player should have not moved upwards after third jump");
    }

}
