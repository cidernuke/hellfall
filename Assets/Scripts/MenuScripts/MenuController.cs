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

    // Change the screen resolution based on the selected dropdown index
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex]; // Get the selected resolution
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen); // Apply the resolution
    }

    // Load game scene
    public void LoadGame()
    {
        SceneManager.LoadScene("Menu"); // Replace with the actual game scene name
    }

    // Start a new game, loading a specified level
    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(_newGameLevel); // Load the new game level
    }

    // Load the saved game if a saved level exists
    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel"); // Get the saved level
            SceneManager.LoadScene(levelToLoad); // Load the saved level
        }
        else
        {
            noSavedGameDialog.SetActive(true); // Show the "no saved game" dialog
        }
    }

    // Exit the game
    public void ExitButton()
    {
        Application.Quit(); // Quit the application
    }

    // Adjust the volume and update the display
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // Set the volume
        volumeTextValue.text = volume.ToString("0.0"); // Display the volume value
    }

    // Toggle fullscreen mode
    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen; // Set fullscreen mode
    }

    // Set the quality level of graphics
    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex; // Set the quality level
    }

    // Apply graphics settings like brightness, quality, and fullscreen mode
    public void GraphicsApply()
    {
        PlayerPrefs.SetInt("masterQuality", _qualityLevel); // Save quality setting
        QualitySettings.SetQualityLevel(_qualityLevel); // Apply quality settings

        PlayerPrefs.SetInt("masterFullscreen", (_isFullScreen ? 1 : 0)); // Save fullscreen mode
        Screen.fullScreen = _isFullScreen; // Apply fullscreen mode

        StartCoroutine(ConfirmationBox()); // Show confirmation box
    }

    // Save and apply volume settings
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume); // Save volume
        StartCoroutine(ConfirmationBox()); // Show confirmation box
    }

    // Reset specific settings (Audio, Gameplay, Graphics) to default values
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

    // Show confirmation prompt for 2 seconds
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true); // Show confirmation prompt
        yield return new WaitForSeconds(2); // Wait for 2 seconds
        confirmationPrompt.SetActive(false); // Hide confirmation prompt
    }
}
