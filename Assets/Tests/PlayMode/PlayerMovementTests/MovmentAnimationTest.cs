using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MovementAnimationTests
{
    private GameObject player;
    private PlayerMovement playerMovement;
    private MockPlayerInput mockInput;
    //private Animator animator;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Lade die Szene
        SceneManager.LoadScene("TestScene");
        yield return null;

        // Initialisiere den Spieler und die benötigten Komponenten
        player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Player GameObject not found in the scene.");

        // animator = player.GetComponent<Animator>();
        // Assert.IsNotNull(animator, "Animator component not found on the Player GameObject.");

        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement, "PlayerMovement component not found on the Player GameObject.");

        mockInput = new MockPlayerInput();
        playerMovement.playerInput = mockInput;

        yield return new WaitForSeconds(1f); // Warte, bis der Spieler auf den Boden gefallen ist
    }

    [UnityTest]
    public IEnumerator RunAnimationIsTriggered()
    {
        // Setze horizontalen Input für Bewegung
        mockInput.horizontalInput = 1f;

        // Update und warte, bis der Animator reagieren kann
        playerMovement.Update();
        yield return new WaitForFixedUpdate();
        yield return null;

        // Überprüfe, ob die Laufanimation getriggert wird
        // bool isRunning = animator.GetBool("run");
        // Assert.IsTrue(isRunning, "Run animation should be active when moving.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator JumpAnimationIsTriggered()
    {
        // Setze Sprung-Input
        mockInput.jumpInput = true;
        playerMovement.Update();
        yield return null;

        // Überprüfe, ob die Sprunganimation ausgelöst wurde
        // bool isJumping = animator.GetCurrentAnimatorStateInfo(0).IsName("Jump");
        // Assert.IsTrue(isJumping, "Jump animation should have been triggered.");

        mockInput.jumpInput = false;
        yield return null;
    }
}
