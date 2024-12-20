using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPrefs : MonoBehaviour
{
    // General settings
    [Header("General Setting")]
    [SerializeField] private bool canUse = false; // Flag to determine if settings should be loaded
    [SerializeField] private MenuController menuController; // Reference to MenuController for resetting settings

    // Volume settings
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null; // UI Text to display volume
    [SerializeField] private Slider volumeSlider = null; // UI Slider for adjusting volume

    // Quality level settings
    [Header("Quality Level Setting")]
    [SerializeField] private TMP_Dropdown qualityDropdown; // Dropdown UI to select quality level

    // Fullscreen settings
    [Header("Fullscreen Setting")]
    [SerializeField] private Toggle fullScreenToggle; // Toggle for fullscreen setting

    // Load player preferences for settings on Awake
    private void Awake()
    {
        // Check if settings should be applied
        if (canUse)
        {
            // Load volume setting from PlayerPrefs (or reset to default if not set)
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume"); // Get the saved volume
                volumeTextValue.text = localVolume.ToString("0.0"); // Display the volume value
                volumeSlider.value = localVolume; // Set the volume slider
                AudioListener.volume = localVolume; // Apply the volume to the game
            }
            else
            {
                menuController.ResetButton("Audio"); // Reset to default volume if not set
            }

            // Load quality setting from PlayerPrefs (or reset to default if not set)
            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int localQuality = PlayerPrefs.GetInt("masterQuality"); // Get the saved quality level
                qualityDropdown.value = localQuality; // Set the dropdown to the saved quality level
                QualitySettings.SetQualityLevel(localQuality); // Apply the quality level
            }
            else
            {
                menuController.ResetButton("Graphics"); // Reset to default quality if not set
            }

            // Load fullscreen setting from PlayerPrefs
            if (PlayerPrefs.HasKey("masterFullscreen"))
            {
                int localFullscreen = PlayerPrefs.GetInt("masterFullscreen"); // Get fullscreen setting
                Screen.fullScreen = (localFullscreen == 1); // Set fullscreen based on saved preference
                fullScreenToggle.isOn = (localFullscreen == 1); // Update the fullscreen toggle UI
            }
        }
    }
}
