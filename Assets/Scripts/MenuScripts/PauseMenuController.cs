using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // The pause menu UI prefab that will be shown when the game is paused
    [SerializeField] private GameObject pauseMenuUI;

    // Tracks whether the game is paused or not
    [SerializeField] private bool isPaused;

    // Reference to the background music audio source that won't be paused
    [SerializeField] private AudioSource musicAudioSource;

    // Reference to the main menu scene name for the load confirmation button
    [Header("MainMenu")]
    public string _mainMenu;

    // Checks for input during every frame
    private void Update()
    {
        // Toggle pause status when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            // Make the cursor visible and unlock it
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Activate or deactivate the menu depending on the pause state
        if (isPaused)
        {
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
        }
    }

    // Activates the pause menu
    void ActivateMenu()
    {
        Time.timeScale = 0; // Pause the game by stopping time

        // Show the pause menu UI
        pauseMenuUI.SetActive(true);

        // Unlock and show the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Pause all audio sources except the background music
        foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
        {
            if (audio != musicAudioSource)
            {
                audio.Pause();
            }
        }
    }

    // Deactivates the pause menu
    public void DeactivateMenu()
    {
        Time.timeScale = 1; // Resume the game by resuming time

        // Hide the pause menu UI
        pauseMenuUI.SetActive(false);
        isPaused = false;

        // Lock and hide the cursor again
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Resume all audio sources except the background music
        foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
        {
            if (audio != musicAudioSource)
            {
                audio.UnPause();
            }
        }
    }

    // On confirming, load the main menu scene
    public void LoadMainMenuYes()
    {
        SceneManager.LoadScene(_mainMenu); // Replace with the actual name of the main menu scene
    }

    // public void LoadGameYes()
    // {
    //     // Zuerst sicherstellen, dass das Spiel nicht mehr pausiert ist.
    //     Time.timeScale = 1f;
    //     DeactivateMenu();

    //     // Prüfen, ob ein Spielstand existiert
    //     var data = SaveManager.Instance.LoadPlayerData();
    //     if (data == null)
    //     {
    //         Debug.LogWarning("Kein gespeicherter Spielstand gefunden!");
    //         // Ggf. Dialog anzeigen oder Meldung ausgeben
    //         return;
    //     }

    //     // Die gespeicherte Szene auslesen
    //     string sceneName = data.lastSceneName;
    //     if (string.IsNullOrEmpty(sceneName))
    //     {
    //         Debug.LogWarning("Der gespeicherte Szenenname ist leer. Kein Szenenwechsel möglich.");
    //         return;
    //     }

    //     // Szene laden, in der zuletzt gespeichert wurde
    //     SceneManager.LoadScene(sceneName);

    //     // Der Rest (ApplyLoadedData) erfolgt entweder über:
    //     // a) OnSceneLoaded im GameManager, oder
    //     // b) eine kleine Coroutine nach dem Szenenwechsel
    //     //    z.B. StartCoroutine(LoadGameCoroutine())
    //     //    -> SaveManager.Instance.LoadGame(...)
    // }
}
