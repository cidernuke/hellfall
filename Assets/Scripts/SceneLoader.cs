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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
        SceneManager.LoadScene(sceneToLoad);
    }
}
