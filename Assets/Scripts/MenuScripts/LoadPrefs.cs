using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPrefs : MonoBehaviour
{
    // General settings
    [Header("General Setting")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private MenuController menuController;

    // Volume settings
    [Header("Volume Setting")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;

    // Brightness settings
    [Header("Brightness Setting")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;

    // Quality level settings
    [Header("Quality Level Setting")]
    [SerializeField] private TMP_Dropdown qualityDropdown;

    // Fullscreen settings
    [Header("Fullscreen Setting")]
    [SerializeField] private Toggle fullScreenToggle;

    // Controller sensitivity settings
    [Header("Sensitivity Setting")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider controllerSenSlider = null;

    // Invert Y-axis setting
    [Header("Invert Y Setting")]
    [SerializeField] private Toggle invertYToggle = null;

    // Load player preferences for settings on Awake
    private void Awake()
    {
        if (canUse)
        {
            // Load volume setting or reset to default
            if (PlayerPrefs.HasKey("masterVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("masterVolume");
                volumeTextValue.text = localVolume.ToString("0.0");
                volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
            }
            else
            {
                menuController.ResetButton("Audio");
            }

            // Load quality setting or reset to default
            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int localQuality = PlayerPrefs.GetInt("masterQuality");
                qualityDropdown.value = localQuality;
                QualitySettings.SetQualityLevel(localQuality);
            }
            else
            {
                menuController.ResetButton("Graphics");
            }

            // Load fullscreen setting
            if (PlayerPrefs.HasKey("masterFullscreen"))
            {
                int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");
                Screen.fullScreen = (localFullscreen == 1);
                fullScreenToggle.isOn = (localFullscreen == 1);
            }

            // Load brightness setting
            if (PlayerPrefs.HasKey("masterBrightness"))
            {
                float localBrightness = PlayerPrefs.GetFloat("masterBrightness");
                brightnessTextValue.text = localBrightness.ToString("0.0");
                brightnessSlider.value = localBrightness;
                // Apply brightness in the game here
            }

            // Load controller sensitivity setting
            if (PlayerPrefs.HasKey("masterSen"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSen");
                controllerSenTextValue.text = localSensitivity.ToString("0.0");
                controllerSenSlider.value = localSensitivity;
                menuController.mainControllerSen = Mathf.RoundToInt(localSensitivity);
            }

            // Load invert Y setting
            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                invertYToggle.isOn = (PlayerPrefs.GetInt("masterInvertY") == 1);
            }
        }
    }
}
