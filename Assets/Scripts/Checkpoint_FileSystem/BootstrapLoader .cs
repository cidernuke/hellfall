// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class BootstrapLoader : MonoBehaviour
// {
//     void Start()
//     {
//         // Annahme: Die Hauptmenü-Szene heißt "MainMenuScene"
//         SceneManager.LoadScene("Menu", LoadSceneMode.Single);

//         void Start()
//         {
//             if (player == null)
//             {
//                 var playerObj = GameObject.FindWithTag("Player");
//                 if (playerObj != null)
//                 {
//                     player = playerObj;
//                     playerMov = player.GetComponent<PlayerMovement>();
//                     playerHealth = player.GetComponent<HealthSystem>();
//                 }
//                 else
//                 {
//                     Debug.LogWarning("No Player found in scene.");
//                 }
//             }

//             if (uiDocument == null)
//             {
//                 uiDocument = FindObjectOfType<UIDocument>();
//                 if (uiDocument == null)
//                 {
//                     Debug.LogError("No UIDocument found in this scene!");
//                     return;
//                 }
//             }

//             root = uiDocument.rootVisualElement;
//         }
//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class BootstrapLoader : MonoBehaviour
{
    // Optional: Falls du hier direkt Zugriff auf deine Manager willst
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

        //Markiert dieses ganze GameObject (und seine Kinder) als "dont destroy"
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 3) Hier kannst du optional Manager initialisieren, z.B. 
        //    gameManager.Init();
        //    inventorySystem.Init();
        //    etc.

        // 4) Szene laden ("Menu")
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