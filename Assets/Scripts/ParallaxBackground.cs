using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform player;               // Reference to the player transform
    public Transform[] backgroundLayers;   // Array of background layers
    public float[] parallaxScales;         // Scales to determine each layer's movement speed (0 = far, 1 = close)
    public float smoothing = 0.5f;         // Smoothing factor for parallax effect

    private Vector3[] startPositions;      // Store the initial positions of each background layer

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned in ParallaxBackground.");
            return;
        }

        // Initialize start positions for each layer
        startPositions = new Vector3[backgroundLayers.Length];
        for (int i = 0; i < backgroundLayers.Length; i++)
        {
            startPositions[i] = backgroundLayers[i].position;

            // Auto-assign default parallax scales if not set
            if (parallaxScales.Length != backgroundLayers.Length)
            {
                Debug.LogWarning("Parallax scales array length doesn't match background layers array length. Assigning default scales.");
                parallaxScales = new float[backgroundLayers.Length];
                for (int j = 0; j < backgroundLayers.Length; j++)
                {
                    parallaxScales[j] = 0.1f * (j + 1); // Layer farther back moves slower
                }
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < backgroundLayers.Length; i++)
        {
            // Calculate how far the player has moved relative to the start position
            Vector3 playerOffset = player.position - startPositions[i];

            // Calculate the new position for the layer based on parallax scales
            Vector3 layerTargetPos = startPositions[i] + playerOffset * parallaxScales[i];

            // Smoothly move the background layer to the target position
            backgroundLayers[i].position = Vector3.Lerp(backgroundLayers[i].position, layerTargetPos, smoothing * Time.deltaTime);
        }
    }
}
