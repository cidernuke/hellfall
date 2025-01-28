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
    [SerializeField] private UndeadExecutioner undeadExecutioner;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isScene)
        {
            if (other.CompareTag("Player"))
            {
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
        if (vampireCountess != null)
        {
            vampireCountess.didBossKillPlayer = false;
            vampireCountess.gameObject.SetActive(true);
        }
        else if (undeadExecutioner != null)
        {
            // if (GameObject.Find("UndeadExecutioner_Minions").TryGetComponent<Explosion>(out var minion))
            // {
            //     minion.didMinionKillPlayer = false;
            // }
            undeadExecutioner.didBossKillPlayer = false;
            undeadExecutioner.gameObject.SetActive(true);
        }
        bossHealthBar.SetActive(true);
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true); // Activate the loading screen
        }

        yield return new WaitForSeconds(1); // Simulate loading time

        if (player != null && bossRoomEntrance != null)
        {
            player.transform.position = bossRoomEntrance.position; // Teleport the player
        }

        yield return new WaitForSeconds(0.5f); // Small delay before hiding the loading screen

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false); // Deactivate the loading screen
        }

        //! move to respective boss scripts
        // gameObject.SetActive(false); 
    }

    private IEnumerator LoadScene()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true); // Activate the loading screen
        }

        yield return new WaitForSeconds(1f); // Simulate loading time (optional)
        SceneManager.LoadScene(sceneToLoad);
    }
}
