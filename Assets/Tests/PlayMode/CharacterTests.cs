// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.TestTools;
// using Moq;

// public class CharacterTests
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
//                 //add the movement
//                 var playerMovement = player.AddComponent<PlayerMovement>();

//                 // Get the initial position of the player
//                 Vector3 initialPosition = player.transform.position;

//                 // Mock the input to simulate jump input
//                 var mockPlayerMovement = new Mock<PlayerMovement>();
//                 mockPlayerMovement.Setup(m => m.GetJumpInput()).Returns(true);

//                 // Replace the original PlayerMovement component with the mocked one
//                 player.AddComponent<PlayerMovement>();
//                 playerMovement = mockPlayerMovement.Object;

//                 // Set the grounded status to true to allow jumping
//                 mockPlayerMovement.Object.grounded = true;

//                 // Capture the initial vertical velocity
//                 var initialVelocity = mockPlayerMovement.Object.GetComponent<Rigidbody2D>().velocity;

//                 // Call HandleJumpInput to simulate the jump
//                 // playerMovement.HandleJumpInput();

//                 // Wait for 1 second to allow the jump to occur
//                 yield return new WaitForSeconds(0.2f);

//                 // Get the new vertical velocity after calling HandleJumpInput
//                 var newVelocity = mockPlayerMovement.Object.GetComponent<Rigidbody2D>().velocity;

//                 // Assert that the vertical velocity has increased by jumpPower
//                 Assert.AreEqual(initialVelocity.x, newVelocity.x, "Horizontal velocity should not change.");
//                 Assert.Greater(newVelocity.y, initialVelocity.y, "Vertical velocity should increase due to jump.");


//                 yield return null;

//         }
//         /*[UnityTest]
//          public IEnumerator PlayerMovesLeftWhenAPressed()
//          {
//              // Load the scene
//              SceneManager.LoadScene("SampleScene");
//              // Wait for 3 seconds to allow the player to load and hit the ground
//              yield return new WaitForSeconds(3);

//              // Find the player GameObject
//              GameObject player = GameObject.Find("Player");
//              PlayerMovment playerMovment = player.GetComponent<PlayerMovment>();

//              // Get the initial position of the player
//              Vector3 initialPosition = player.transform.position;

//              // Simulate "A" key press
//              var keyboard = InputSystem.AddDevice<Keyboard>();
//              Press(keyboard.aKey);
//              // Wait for 0.5 seconds to allow the player to move
//              yield return new WaitForSeconds(0.5f);
//              Release(keyboard.aKey);

//              // Wait for a frame
//              yield return null;

//              // Get the new position of the player
//              Vector3 newPosition = player.transform.position;

//              // Assert that the new position is to the left of the initial position
//              Assert.Less(newPosition.x, initialPosition.x);
//          }
//         [UnityTest]
//         public IEnumerator PlayerMovesRightWhenDPressed()
//         {
//              // Load the scene
//              SceneManager.LoadScene("SampleScene");
//              // Wait for 3 seconds to allow the player to load and hit the ground
//              yield return new WaitForSeconds(3);

//              // Find the player GameObject
//              GameObject player = GameObject.Find("Player");
//              PlayerMovment playerMovment = player.GetComponent<PlayerMovment>();

//              // Get the initial position of the player
//              Vector3 initialPosition = player.transform.position;

//              // Simulate "D" key press
//              var keyboard = InputSystem.AddDevice<Keyboard>();
//              Press(keyboard.dKey);
//              // Wait for 0.5 seconds to allow the player to move
//              yield return new WaitForSeconds(0.5f);
//              Release(keyboard.dKey);

//              // Wait for a frame
//              yield return null;

//              // Get the new position of the player
//              Vector3 newPosition = player.transform.position;

//              // Assert that the new position is to the right of the initial position
//              Assert.Greater(newPosition.x, initialPosition.x);
//          }*/
// }
