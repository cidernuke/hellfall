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

    private GameObject background_1;
    private GameObject background_2;
    private GameObject background_3;
    private GameObject background_4;


    private void Start()
    {
        
        background_1 = GameObject.Find("background1");
        background_2 = GameObject.Find("background2");
        background_3 = GameObject.Find("background3");
        background_4 = GameObject.Find("background4b");


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
        background_1.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y+5, 5);
        background_2.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y+5, 4);
        background_3.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y+5, 3);
        background_4.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y+5, 2);


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
