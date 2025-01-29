using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LeaderboardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text bestTimesText;
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Other UI to hide")]
    private GameObject uiBottomRight;

    [Header("Level Info")]
    [SerializeField] private string levelName = "Scene_01"; 
    //[SerializeField] private string nextLevelScene = "Scene_02"; // For "Continue"
    [SerializeField] private string mainMenuScene = "Menu";     // For "MainMenu"

    private float finalTime; // will be saved once the trigger is activated
    private bool isNewTimeAdded = false;

    void Start()
    {
        // If the UI should be invisible at the start:
        gameObject.SetActive(false);

        // Dynamic assignment
        var uiBottom = GameObject.Find("UI Bottom right");
        if(uiBottom != null){
            uiBottomRight = uiBottom;
        }
        else{
            print("uiBottomRight missing");
        }
    }

    /// <summary>
    /// Called by the LeaderboardTrigger (or LevelEndTrigger) 
    /// when the level is completed. 
    /// We get the finalTime here.
    /// </summary>
    public void ShowLeaderboard(float finalTimeFromTrigger)
    {
        // Pause the game
        Time.timeScale = 0f;

        // Hide other UI
        InventorySystem.Instance.HideInventoryUI();
        if (uiBottomRight != null) uiBottomRight.SetActive(false);

        // Activate UI
        gameObject.SetActive(true);

        // Activate cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        finalTime = finalTimeFromTrigger;
        isNewTimeAdded = false; // haven't added an entry to the leaderboard yet

        // Format time
        currentTimeText.text = "Your time was: " + FormatTime(finalTime);

        // Display list (without the new entry?)
        UpdateBestTimesDisplay();
    }

    /// <summary>
    /// Called when the player clicks the "Confirm" button 
    /// after entering their name.
    /// </summary>
    public void OnConfirmName()
    {
        if (isNewTimeAdded == false)
        {
            string enteredName = nameInputField.text;
            if (string.IsNullOrEmpty(enteredName)) enteredName = "Unknown";

            // Add new entry to TimerSystem
            TimerSystem.Instance.AddLeaderboardEntry(levelName, finalTime, enteredName);
            isNewTimeAdded = true;
            
            // Display list again
            UpdateBestTimesDisplay();
        }
    }
    
    /// <summary>
    /// Updates the bestTimesText field based on the current list 
    /// from the TimerSystem.
    /// </summary>
    private void UpdateBestTimesDisplay()
    {
        List<LeaderboardEntry> entries = TimerSystem.Instance.GetBestEntries(levelName);

        string text = "Record Times:\n";
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            text += $"{(i+1)}. {entry.playerName} - {FormatTime(entry.time)}\n";
        }

        bestTimesText.text = text;
    }

    /// <summary>
    /// Called when the player clicks the "Continue" button.
    /// </summary>
    public void OnClickContinue()
    {
        // Resume the game
        Time.timeScale = 1f;

        gameObject.SetActive(false);

        uiBottomRight.SetActive(true);

        // Load next level
        //SceneManager.LoadScene(nextLevelScene);
    }

    /// <summary>
    /// Called when the player clicks the "Main Menu" button.
    /// </summary>
    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        SceneManager.LoadScene(mainMenuScene);
    }

    /// <summary>
    /// Formats the given time in seconds to a string in the format "minutes:seconds".
    /// </summary>
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:0}:{secs:00}";
    }
}
