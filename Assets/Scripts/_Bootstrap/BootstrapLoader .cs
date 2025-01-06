using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BootstrapLoader : MonoBehaviour
{
    //Für direkten Zugriff auf die Manager
    // private InventorySystem inventorySystem = InventorySystem.Instance;
    // private GameManager gameManager = GameManager.Instance;
    // private SaveManager saveManager = SaveManager.Instance;

    [Header("Scenes to Load")]
    public string sceneToLoad = "Menu";

    private void Awake()
    {
        //Verhindern, dass Bootstrap mehrfach existiert
        var existingBootstrap = FindObjectsOfType<BootstrapLoader>();
        if (existingBootstrap.Length > 1)
        {
            // Zerstören, falls schon ein Bootstrap da ist
            Destroy(gameObject);
            return;
        }

        //Markiert dieses GameObject (und seine Kinder) als "dont destroy"
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // if (!playerSpawned && playerPrefab != null)
        // {
        //     GameObject player = Instantiate(playerPrefab);
        //     DontDestroyOnLoad(player);     
        //     playerSpawned = true;
        // }

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