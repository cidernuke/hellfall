using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    // [System.Serializable]
    // public class ParallaxLayer
    // {
    //     public Transform layer;       // The layer object to scroll
    //     public Vector2 parallaxSpeed; // Speed for X and Y scrolling
    // }

    // public ParallaxLayer[] layers;   // Array of layers for the parallax
    public Camera mainCamera;        // Reference to the main camera

    private Vector3 cameraStartPos;  // The camera's starting position

    private GameObject background;

    private void Start()
    {
        background = GameObject.Find("Space_Background");
        // If no camera is assigned, use the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Store the initial camera position
        cameraStartPos = mainCamera.transform.position;
    }

    private void Update()
    {
        background.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 3);

        // Vector3 cameraOffset = mainCamera.transform.position - cameraStartPos;

        // // Loop through each layer and update its position relative to the camera
        // foreach (ParallaxLayer layer in layers)
        // {
        //     if (layer.layer != null)
        //     {
        //         // Calculate the new position based on the camera's movement
        //         Vector3 layerPosition = cameraStartPos + new Vector3(
        //             cameraOffset.x * layer.parallaxSpeed.x,
        //             cameraOffset.y * layer.parallaxSpeed.y,
        //             layer.layer.position.z // Maintain original Z position
        //         );

        //         layer.layer.position = layerPosition;
        //     }
        // }
    }
}
