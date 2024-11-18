using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WallSlideMovement_Tests : PlayerMovement_Test_SetUp
{
    [UnityTest]
    public IEnumerator BasicWallSlide()
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

        // Überprüfe, ob der Spieler wall sliding ist
        Assert.IsTrue(playerMovement.IsWallSliding, "Der Spieler sollte beim Bewegen in Richtung einer Wand in der Luft wall sliding sein.");

        // Überprüfe, ob die vertikale Geschwindigkeit des Spielers auf die Wall-Slide-Geschwindigkeit begrenzt ist
        Assert.AreEqual(-playerMovement.WallSlideSpeed, playerMovement.Body.velocity.y, 0.1f, "Die vertikale Geschwindigkeit sollte auf die Wall-Slide-Geschwindigkeit begrenzt sein.");

        yield return null;
    }


    [UnityTest]
    public IEnumerator WallSlideEndsWhenGrounded()
    {
        // Positioniere den Spieler in der Luft neben einer Wand
        player.transform.position = new Vector3(-8, 6, 0);

        // Simuliere horizontale Eingabe in Richtung der Wand
        mockInput.horizontalInput = -1f;

        playerMovement.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        // Überprüfe, ob der Spieler wall sliding ist
        Assert.IsTrue(playerMovement.IsWallSliding, "Der Spieler sollte wall sliding sein.");

        // Simuliere, dass der Spieler den Boden erreicht
        //player.transform.position = new Vector3(0, 0, 0); // Spieler auf den Boden setzen
        yield return new WaitForSeconds(1.5f);
        
        playerMovement.Update();

        yield return new WaitForFixedUpdate();

        // Überprüfe, ob der Spieler nicht mehr wall sliding ist
        Assert.IsFalse(playerMovement.IsWallSliding, "Der Spieler sollte nicht mehr wall sliding sein, wenn er den Boden berührt.");

        yield return null;
    }


    [UnityTest]
    public IEnumerator WallSlideDoesNotOccurWhenGrounded()
    {
        // Positioniere den Spieler neben einer Wand auf dem Boden
        player.transform.position = new Vector3(-8, -2.9f, 0);

        // Simuliere horizontale Eingabe in Richtung der Wand
        mockInput.horizontalInput = -1f;

        // Rufe Update auf, um die Eingabe zu verarbeiten
        playerMovement.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);


        // Überprüfe, ob der Spieler nicht wall sliding ist
        Assert.IsTrue(playerMovement.Grounded, "Der Spieler sollte grounded sein.");
        Assert.IsFalse(playerMovement.IsWallSliding, "Der Spieler sollte nicht wall sliding sein, wenn er geerdet ist.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator WallSlideRequiresHorizontalInput()
    {
        // Positioniere den Spieler in der Luft neben einer Wand
        player.transform.position = new Vector3(-8f, 6, 0);

        // Simuliere horizontale Eingabe
        mockInput.horizontalInput = -1f;

        // Rufe Update auf, um die Eingabe zu verarbeiten
        playerMovement.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        Assert.IsTrue(playerMovement.IsWallSliding, "Der Spieler sollte beim Bewegen in Richtung einer Wand in der Luft wall sliding sein.");

        mockInput.horizontalInput = 0f;
        playerMovement.Update();

        yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        // Überprüfe, ob der Spieler nicht wall sliding ist
        Assert.IsFalse(playerMovement.IsWallSliding, "Der Spieler sollte nicht wall sliding sein, wenn keine horizontale Eingabe erfolgt.");

        yield return null;
    }
}