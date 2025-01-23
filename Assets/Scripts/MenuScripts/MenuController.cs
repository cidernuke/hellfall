using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    // Volume settings
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null; // UI text to display volume value
    [SerializeField] private Slider volumeSlider = null; // Slider to control volume
    [SerializeField] private float defaultVolume = 1.0f; // Default volume value

    // Graphics settings for brightness, quality, and fullscreen
    [SerializeField] private TMP_Dropdown qualityDropdown; // Dropdown for quality settings
    [SerializeField] private Toggle fullScreenToggle; // Toggle for fullscreen mode

    private int _qualityLevel; // Holds the quality setting
    private bool _isFullScreen; // Tracks if fullscreen is enabled
    private float _brightnessLevel; // Holds the current brightness level

    // Confirmation prompt for setting changes
    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt;

    // Scene loading settings
    [Header("Levels To Load")]
    public string _newGameLevel; // Level to load for a new game
    private string levelToLoad; // Level to load for a saved game
    [SerializeField] private GameObject noSavedGameDialog = null; // Dialog when no saved game is found

    // Resolution dropdown options
    [Header("Resolution Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions; // List of available screen resolutions

    // Initialize resolution dropdown and set the current resolution
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions(); // Clear existing options

        List<string> options = new List<string>(); // List to hold resolution options
        int currentResolutionIndex = 0; // Default index for current resolution

        // Loop through resolutions and add them to the dropdown
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i; // Set the current resolution index
            }
        }

        resolutionDropdown.AddOptions(options); // Add options to the dropdown
        resolutionDropdown.value = currentResolutionIndex; // Set the current resolution in the dropdown
    }

    /// <summary>
    /// Change the screen resolution based on the selected dropdown index.
    /// </summary>
    /// <param name="resolutionIndex">Index of the selected resolution.</param>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex]; // Get the selected resolution
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen); // Apply the resolution
    }

    /// <summary>
    /// Load the main menu scene.
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Start a new game, loading a specified level.
    /// </summary>
    public void NewGameDialogYes()
    {
        //Deactivate MenuPause if activ
        Time.timeScale = 1f;
        var pauseMenuController = FindObjectOfType<PauseMenuController>();
        if (pauseMenuController != null)
        {
            pauseMenuController.DeactivateMenu();
        }

        // Rest values
        GameManager.Instance.StartNewGame();

        // Load the new game level
        SceneManager.LoadScene(_newGameLevel);
    }

    /// <summary>
    /// Load the saved game if a saved level exists.
    /// </summary>
    public void LoadGameDialogYes()
    {
        var data = SaveManager.Instance.LoadPlayerData();
        if (data == null)
        {
            print("No saved game found!");
            noSavedGameDialog.SetActive(true);
            return;
        }
        SaveManager.Instance.LoadSceneFromSave();

        //Update EnemyHealthUI
        var enemyObject = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObject != null)
        {
            foreach (var enemy in enemyObject)
            {
                //print("we da champs");
                enemy.GetComponent<HealthSystem>().ResetEnemySliderToFullHealth();
            }
        }
        else
        {
            Debug.Log("HealthSystem not found on enemyObject");
        }
    }

    /// <summary>
    /// Exit the game.
    /// </summary>
    public void ExitButton()
    {
        Application.Quit(); // Quit the application
    }

    /// <summary>
    /// Adjust the volume and update the display.
    /// </summary>
    /// <param name="volume">Volume level to set.</param>
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // Set the volume
        volumeTextValue.text = volume.ToString("0.0"); // Display the volume value
    }

    /// <summary>
    /// Toggle fullscreen mode.
    /// </summary>
    /// <param name="isFullScreen">True to enable fullscreen, false to disable.</param>
    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen; // Set fullscreen mode
    }

    /// <summary>
    /// Set the quality level of graphics.
    /// </summary>
    /// <param name="qualityIndex">Index of the quality level to set.</param>
    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex; // Set the quality level
    }

    /// <summary>
    /// Apply graphics settings like brightness, quality, and fullscreen mode.
    /// </summary>
    public void GraphicsApply()
    {
        PlayerPrefs.SetInt("masterQuality", _qualityLevel); // Save quality setting
        QualitySettings.SetQualityLevel(_qualityLevel); // Apply quality settings

        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0)); // Save fullscreen mode
        Screen.fullScreen = _isFullScreen; // Apply fullscreen mode

        StartCoroutine(ConfirmationBox()); // Show confirmation box
    }

    /// <summary>
    /// Save and apply volume settings.
    /// </summary>
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume); // Save volume
        StartCoroutine(ConfirmationBox()); // Show confirmation box
    }

    /// <summary>
    /// Reset specific settings (Audio, Gameplay, Graphics) to default values.
    /// </summary>
    /// <param name="MenuType">Type of menu to reset (Audio, Graphics).</param>
    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume; // Reset volume
            volumeSlider.value = defaultVolume; // Reset volume slider
            volumeTextValue.text = defaultVolume.ToString("0.0"); // Reset volume display
            VolumeApply(); // Apply volume settings
        }

        if (MenuType == "Graphics")
        {
            qualityDropdown.value = 1; // Reset quality dropdown
            QualitySettings.SetQualityLevel(1); // Apply default quality level

            fullScreenToggle.isOn = false; // Reset fullscreen toggle
            Screen.fullScreen = false; // Apply fullscreen mode

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen); // Reset resolution
            resolutionDropdown.value = resolutions.Length; // Set resolution dropdown
            GraphicsApply(); // Apply graphics settings
        }
    }

    /// <summary>
    /// Show confirmation prompt for 2 seconds.
    /// </summary>
    /// <returns>IEnumerator for coroutine.</returns>
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true); // Show confirmation prompt
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        confirmationPrompt.SetActive(false); // Hide confirmation prompt
    }
}
