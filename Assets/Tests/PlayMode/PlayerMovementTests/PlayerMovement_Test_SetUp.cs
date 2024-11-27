using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public abstract class PlayerMovement_Test_SetUp
{
    protected static bool sceneLoaded = false;
    protected GameObject player;
    protected PlayerMovement playerMovement;
    protected MockPlayerInput mockInput;

    [UnitySetUp]
    public virtual IEnumerator SetUp()
    {
        // Load scene only once
        // if (!sceneLoaded)
        // {
            SceneManager.LoadScene("TestScene");
            yield return null; // Wait for scene being loaded
            sceneLoaded = true;
        //}

        // Initialize the Player
        player = GameObject.Find("Player");
        Assert.IsNotNull(player, "Player GameObject not found in the scene.");

        // Initialize the Components
        playerMovement = player.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement, "PlayerMovement component not found on the Player GameObject.");

        // Initialize the Mock Input
        mockInput = new MockPlayerInput();
        playerMovement.playerInput = mockInput;

        // Wait for the Player to settle on the ground
        yield return new WaitForSeconds(1f);
    }
}
