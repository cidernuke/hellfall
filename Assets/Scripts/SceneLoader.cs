using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Name of the scene to load
    [SerializeField] private GameObject loadingScreen; // Reference to loading screen object (optional)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the trigger
        {
            print("starting coroutine");
            StartCoroutine(LoadScene());
        }
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
        SceneManager.LoadScene(sceneToLoad); // Load the specified scene
    }
}
