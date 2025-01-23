using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [Header("Scenes to Load")]
    public string sceneToLoad = "Menu";

    private void Awake()
    {
        // Prevent Bootstrap from existing multiple times
        var existingBootstrap = FindObjectsOfType<BootstrapLoader>();
        if (existingBootstrap.Length > 1)
        {
            // Destroy if a Bootstrap already exists
            Destroy(gameObject);
            return;
        }

        // Mark this GameObject (and its children) as "dont destroy"
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Load Scene
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogWarning("sceneToLoad is empty - not loading any additional scene.");
        }
    }
}