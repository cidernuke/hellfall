using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LevelEndUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text bestTimesText; // Zeigt die ganze Liste an

    public void ShowResults(string levelName, float currentTime)
    {
        currentTimeText.text = "Deine Zeit: " + FormatTime(currentTime);

        // Bestzeiten-Liste holen
        List<float> times = TimerSystem.Instance.GetBestTimes(levelName);

        // Eine schöne Ausgabe zusammenbauen
        string bestTimesInfo = "Bestzeiten:\n";
        for (int i = 0; i < times.Count; i++)
        {
            bestTimesInfo += $"{i+1}. {FormatTime(times[i])}\n";
        }

        bestTimesText.text = bestTimesInfo;
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:0}:{secs:00}";
    }
}
