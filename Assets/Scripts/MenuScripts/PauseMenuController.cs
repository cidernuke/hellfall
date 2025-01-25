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

    /// <summary>
    /// Checks for input during every frame and toggles the pause status when the Escape key is pressed.
    /// </summary>
    private void Update()
    {        
        // Prevent pause menu from opening if the main menu is active
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            return;
        }
        
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

    /// <summary>
    /// Activates the pause menu, pauses the game, and handles UI and audio adjustments.
    /// </summary>
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

    /// <summary>
    /// Deactivates the pause menu, resumes the game, and handles UI and audio adjustments.
    /// </summary>
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

    /// <summary>
    /// Loads the main menu scene and deactivates the pause menu.
    /// </summary>
    public void LoadMainMenuYes()
    {
        DeactivateMenu();
        SceneManager.LoadScene(_mainMenu);
        Cursor.visible = true; // Ensure cursor is visible
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
    }

    /// <summary>
    /// Respawns the player and resets the health of all enemies.
    /// </summary>
    public void RespawnYes()
    {
        GameManager.Instance.RespawnPlayer();
        var enemyObject = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObject != null)
        {
            foreach (var enemy in enemyObject)
            {
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
