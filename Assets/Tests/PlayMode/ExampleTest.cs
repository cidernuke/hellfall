// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.TestTools;
// using Moq; //very important!!

// public class ExampleTest
// {
//         [UnityTest]
//         public IEnumerator PlayerCanJump()
//         {
//                 // Load the scene
//                 SceneManager.LoadScene("SampleScene");
//                 // Wait for 3 seconds to allow the player to load and hit the ground
//                 yield return new WaitForSeconds(3);
//                 // Find the player GameObject
//                 GameObject player = GameObject.Find("Player");

//                 // Get the PlayerMovment component attached to the player GameObject
//                 PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

//                 // Get the initial position of the player
//                 Vector3 initialPosition = player.transform.position;

//                 //Mocking is the heart and soul of our tests, please look into this or just use chat lol
//                 // Mock PlayerMovement and override GetJumpInput to return true
//                 var mockPlayerMovement = new Mock<PlayerMovement>() { CallBase = true };
//                 mockPlayerMovement.Setup(pm => pm.GetJumpInput()).Returns(true);

//                 // Assign mockPlayerMovement to the player GameObject
//                 player.GetComponent<PlayerMovement>().enabled = false;
//                 player.AddComponent(mockPlayerMovement.Object.GetType());

//                 // Set the grounded status to true to allow jumping
//                 mockPlayerMovement.Object.grounded = true;

//                 // Capture the initial vertical velocity
//                 var initialVelocity = mockPlayerMovement.Object.GetComponent<Rigidbody2D>().velocity;

//                 // Call HandleJumpInput to simulate the jump
//                 //    mockPlayerMovement.Object.HandleJumpInput();

//                 // Wait for 1 second to allow the jump to occur
//                 yield return new WaitForSeconds(0.2f);

//                 // Assert that the player's y position has increased, indicating a jump
//                 Assert.Greater(player.transform.position.y, initialPosition.y);

//         }
// }
