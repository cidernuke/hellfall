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

        // Hide all Hint-Bubbles (Edge-Case)
        // (Set Tag on your UI hint-bubbles to "HintUI")
        var allHints = GameObject.FindGameObjectsWithTag("HintUI");
        foreach (var hint in allHints)
        {
            // if it's active, turn it off
            if (hint.activeSelf)
            {
                hint.SetActive(false);
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
        DeactivateMenu();
    }

    public void RespawnYes()
    {
        GameManager.Instance.RespawnPlayer();
        // DeactivateMenu();
        var enemyObject = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObject != null)
        {
            foreach (var enemy in enemyObject)
            {
                //print("we da champs");
                enemy.GetComponent<HealthSystem>().ResetEnemySliderToFullHealth();
            }
            DeactivateMenu();
        }
        else
        {
            Debug.Log("HealthSystem not found on enemyObject");
        }

    }
}
