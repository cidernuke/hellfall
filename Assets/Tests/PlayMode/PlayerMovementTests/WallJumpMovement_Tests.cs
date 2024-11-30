using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class WallJump_Tests : PlayerMovement_Test_SetUp
{
    [UnityTest]
    public IEnumerator PlayerWallJumpsWhenSlidingOnWall()
    {
       // Positioniere den Spieler in der Luft neben einer Wand
        player.transform.position = new Vector3(-8, 6, 0); // Spieler in die Luft setzen

        // Simuliere horizontale Eingabe in Richtung der Wand
        mockInput.horizontalInput = -1f;
        playerMovement.Update();

        // Warte auf FixedUpdate, um die Physik zu verarbeiten
        yield return new WaitForFixedUpdate();

        // Warte einige Frames, um das Wall Sliding zu ermöglichen
        yield return new WaitForSeconds(1f);

        // Assert player is wall sliding
        Assert.IsTrue(playerMovement.IsWallSliding, "Player should be wall sliding.");

        // Simulate jump input
        mockInput.jumpInput = true;
        mockInput.horizontalInput = 0f;
        playerMovement.Update();

        yield return null;

        // Assert player is wall jumping
        Assert.IsTrue(playerMovement.IsWallJumping, "Player should be wall jumping.");
        Assert.Greater(playerMovement.Body.velocity.y, 0,"Player should gain upward velocity from wall jump.");
        Assert.Greater(playerMovement.Body.velocity.x, 0, "Player should gain horizontal velocity away from the wall.");

        yield return new WaitForSeconds(0.5f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator WallJumpOnlyWorksOncePerWall()
    {
        // Positioniere den Spieler in der Luft neben einer Wand
        player.transform.position = new Vector3(-8, 6, 0); // Spieler in die Luft setzen

        // Simuliere horizontale Eingabe in Richtung der Wand
        mockInput.horizontalInput = -1f;
        playerMovement.Update();

        // Warte auf FixedUpdate, um die Physik zu verarbeiten
        yield return new WaitForFixedUpdate();

        // Warte einige Frames, um das Wall Sliding zu ermöglichen
        yield return new WaitForSeconds(0.5f);

        Assert.IsTrue(playerMovement.IsWallSliding, "Player should be wall sliding.");

        // Simulate wall jump
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return null;

        // Assert wall jump was performed
        Assert.IsTrue(playerMovement.IsWallJumping, "Player should have performed a wall jump.");

        // Reset jump input
        mockInput.jumpInput = false;
        playerMovement.Update();
        yield return new WaitForSeconds(0.3f);

        // Try to wall jump again without leaving the wall
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return null;

        // Assert wall jump is not allowed again
        Assert.IsTrue(playerMovement.IsWallSliding, "Player should be wall sliding.");
        Assert.IsFalse(playerMovement.IsWallJumping, "Player should not be able to wall jump on the same wall without leaving.");

        yield return new WaitForSeconds(0.3f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerCanWallJumpAfterLanding()
    {
        // Positioniere den Spieler in der Luft neben einer Wand
        player.transform.position = new Vector3(-8, 2, 0); // Spieler in die Luft setzen

        // Simuliere horizontale Eingabe in Richtung der Wand
        mockInput.horizontalInput = -1f;
        playerMovement.Update();

        // Warte auf FixedUpdate, um die Physik zu verarbeiten
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.5f);

        Assert.IsTrue(playerMovement.IsWallSliding, "Player should be wall sliding.");

        // Simulate wall jump
        mockInput.jumpInput = true;
        mockInput.horizontalInput = 0f;

        playerMovement.Update();
        //yield return new WaitForSeconds(0.1f);
        yield return null;

        // Assert wall jump was performed
        Assert.IsTrue(playerMovement.IsWallJumping, "Player should have performed a wall jump.");

        mockInput.jumpInput = false;
        playerMovement.Update();
        yield return new WaitForSeconds(1.5f);

        // Assert the player can wall jump again after landing
        mockInput.horizontalInput = -1f;
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return null;

        mockInput.jumpInput = false;
        playerMovement.Update();
        yield return new WaitForSeconds(0.1f);
        Assert.IsTrue(playerMovement.IsWallSliding, "Player should be wall sliding.");

        //mockInput.horizontalInput = 0f;
        yield return null;
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return null;
        Assert.IsTrue(playerMovement.IsWallJumping, "Player should be able to wall jump again after landing.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator WallJumpResetsOnSwitchingWalls()
    {
        // Positioniere den Spieler in der Luft neben einer Wand
        player.transform.position = new Vector3(-8, 2, 0); // Spieler in die Luft setzen

        // Simuliere horizontale Eingabe in Richtung der Wand
        mockInput.horizontalInput = -1f;
        playerMovement.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(0.5f);

        Assert.IsTrue(playerMovement.IsWallSliding, "Player should be wall sliding.");

        // Simulate wall jump
        mockInput.jumpInput = true;
        mockInput.horizontalInput = 0f;

        playerMovement.Update();
        //yield return new WaitForSeconds(0.1f);
        yield return null;

        // Assert wall jump was performed
        Assert.IsTrue(playerMovement.IsWallJumping, "Player should have performed a wall jump.");

        // Player moving to the other wall
        mockInput.horizontalInput = 1f;
        mockInput.jumpInput = false;

        playerMovement.Update();
        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        // Simulate wall jump on the second wall
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return null;

        // Assert wall jump was allowed on the second wall
        Assert.IsTrue(playerMovement.IsWallJumping, "Player should be able to wall jump on a different wall.");

        yield return null;
    }
}
