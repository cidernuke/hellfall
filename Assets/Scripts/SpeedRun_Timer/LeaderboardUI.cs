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
    [SerializeField] private string nextLevelScene = "Scene_02"; // Für "Continue"
    [SerializeField] private string mainMenuScene = "Menu";     // Für "MainMenu"

    private float finalTime; // wird gespeichert, sobald Trigger ausgelöst wird
    private bool isNewTimeAdded = false;

    void Start()
    {
        // Falls das UI anfangs unsichtbar sein soll:
        gameObject.SetActive(false);

        //Dynamische zuweisung
        var uiBottom = GameObject.Find("UI Bottom right");
        if(uiBottom != null){
            uiBottomRight = uiBottom;
        }
        else{
            print("uiBottomRight missing");
        }
    }

    /// <summary>
    /// Wird vom LeaderboardTrigger (oder LevelEndTrigger) aufgerufen, 
    /// sobald man das Level beendet hat. 
    /// Wir kriegen hier die finalTime.
    /// </summary>
    public void ShowLeaderboard(float finalTimeFromTrigger)
    {
        // Spiel pausieren
        Time.timeScale = 0f;

        // Andere UI ausblenden
        InventorySystem.Instance.HideInventoryUI();
        if (uiBottomRight != null) uiBottomRight.SetActive(false);

        // UI aktivieren
        gameObject.SetActive(true);

        //Cursor aktivieren
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        finalTime = finalTimeFromTrigger;
        isNewTimeAdded = false; // wir haben noch keinen Eintrag fürs Leaderboard angelegt

        // Zeit formatieren
        currentTimeText.text = "Your time was: " + FormatTime(finalTime);

        // Liste anzeigen (ohne den neuen Eintrag?)
        UpdateBestTimesDisplay();
    }

    /// <summary>
    /// Wird aufgerufen, wenn der Spieler auf den "Bestätigen"-Button klickt, 
    /// nachdem er den Namen eingegeben hat.
    /// </summary>
    public void OnConfirmName()
    {
        if (isNewTimeAdded == false)
        {
            string enteredName = nameInputField.text;
            if (string.IsNullOrEmpty(enteredName)) enteredName = "Unknown";

            // Neuen Eintrag ins TimerSystem
            TimerSystem.Instance.AddLeaderboardEntry(levelName, finalTime, enteredName);
            isNewTimeAdded = true;
            
            // Liste erneut anzeigen
            UpdateBestTimesDisplay();
        }
    }
    
    /// <summary>
    /// Aktualisiert das Textfeld bestTimesText anhand der aktuellen Liste 
    /// aus dem TimerSystem.
    /// </summary>
    private void UpdateBestTimesDisplay()
    {
        List<LeaderboardEntry> entries = TimerSystem.Instance.GetBestEntries(levelName);

        //string text = "Bestzeiten:\n";
        string text = "Record Times:\n";
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            text += $"{(i+1)}. {entry.playerName} - {FormatTime(entry.time)}\n";
        }

        bestTimesText.text = text;
    }

    public void OnClickContinue()
    {
        // Hier das Spiel fortsetzen
        Time.timeScale = 1f;

        gameObject.SetActive(false);

        // Option 1: direkt SceneManager
        // SceneManager.LoadScene(nextLevelScene);

        // Option 2: 
        // Oder du rufst GameManager.Instance.LoadNextLevel() o.ä.
        SceneManager.LoadScene(nextLevelScene);
    }

    public void OnClickMainMenu()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        SceneManager.LoadScene(mainMenuScene);
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:0}:{secs:00}";
    }
}
