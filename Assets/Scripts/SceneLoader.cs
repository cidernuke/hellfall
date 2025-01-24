using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the loading screen shown when transitioning to a new scene
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject loadingScreen; // Reference to loading screen object
    public bool isScene;
    private GameObject player;
    [SerializeField] private Transform bossRoomEntrance;
    public GameObject bossHealthBar;
    [SerializeField] private Boss vampireCountess;
    // private var bossHealthBar;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        // bossHealthBar = GameObject.FindGameObjectWithTag("BossHealthBar");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isScene)
        {
            if (other.CompareTag("Player"))
            {
                print("starting coroutine");
                StartCoroutine(LoadScene());
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(TeleportToBossRoom());
            }
        }
    }

    private IEnumerator TeleportToBossRoom()
    {
        
        print(", bossHealthBar: "+bossHealthBar);
        vampireCountess.gameObject.SetActive(true);
        bossHealthBar.SetActive(true);
        if (loadingScreen != null)
        {
            print("Loading screen activated");
            loadingScreen.SetActive(true); // Activate the loading screen
        }

        yield return new WaitForSeconds(1); // Simulate loading time
        
        if (player != null && bossRoomEntrance != null)
        {
            print("Teleporting player");
            player.transform.position = bossRoomEntrance.position; // Teleport the player
        }

        yield return new WaitForSeconds(0.5f); // Small delay before hiding the loading screen

        if (loadingScreen != null)
        {
            print("Loading screen deactivated");
            loadingScreen.SetActive(false); // Deactivate the loading screen
        }

        gameObject.SetActive(false);
    }

    private IEnumerator LoadScene()
    {
        if (loadingScreen != null)
        {
            print("loadingscreen activated");
            loadingScreen.SetActive(true); // Activate the loading screen
        }

        yield return new WaitForSeconds(1f); // Simulate loading time (optional)
        print("loading scene");
        SceneManager.LoadScene(sceneToLoad);
    }
}
