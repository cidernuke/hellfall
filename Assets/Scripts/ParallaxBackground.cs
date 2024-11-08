using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform[] backgrounds;   // Array of all the background layers
    public float[] parallaxScales;    // The proportion of the camera's movement to move the backgrounds by
    public float smoothing = 1f;      // How smooth the parallax is going to be. Set above 0.

    private Vector3 previousCamPos;   // Position of the camera in the previous frame

    // Called before Start(). Great for references.
    private void Awake()
    {
        previousCamPos = Camera.main.transform.position;
    }

    private void Start()
    {
        // Setting parallax scales based on Z position, if not set manually
        if (parallaxScales.Length != backgrounds.Length)
        {
            parallaxScales = new float[backgrounds.Length];
            for (int i = 0; i < backgrounds.Length; i++)
                parallaxScales[i] = backgrounds[i].position.z * -1;
        }
    }

    private void Update()
    {
        // Loop through each background
        for (int i = 0; i < backgrounds.Length; i++)
        {
            // Calculate parallax effect
            float parallax = (previousCamPos.x - Camera.main.transform.position.x) * parallaxScales[i];

            // Set a target position which is the background's current position plus the parallax effect
            float targetPosX = backgrounds[i].position.x + parallax;

            // Create the target position for the background
            Vector3 targetPosition = new Vector3(targetPosX, backgrounds[i].position.y, backgrounds[i].position.z);

            // Smoothly transition between positions
            backgrounds[i].position = Vector3.Lerp(backgrounds[i].position, targetPosition, smoothing * Time.deltaTime);
        }

        // Update the previous camera position
        previousCamPos = Camera.main.transform.position;

        // Debug.Log("Camera position: " + Camera.main.transform.position);
        // Debug.Log("Background Layer 0 position: " + backgrounds[0].position);
        // Debug.Log("Background Layer 1 position: " + backgrounds[1].position);
        // Debug.Log("Background Layer 2 position: " + backgrounds[2].position);



    }
}

