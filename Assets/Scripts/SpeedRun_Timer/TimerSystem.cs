using System.Collections.Generic;
using UnityEngine;

public class TimerSystem : MonoBehaviour
{
    public static TimerSystem Instance { get; private set; }

    /// <summary>
    ///  Hält die aktuell laufende Zeit pro Level.
    ///  Key: LevelName, Value: Aktuell hochgezählte Zeit
    /// </summary>
    private Dictionary<string, float> currentTimeDict = new Dictionary<string, float>();

    /// <summary>
    ///  Hält eine Liste an Bestzeiten pro Level.
    ///  Key: LevelName, Value: Liste mit Zeiten (sortiert, aufsteigend)
    /// </summary>
    private Dictionary<string, List<float>> bestTimesDict = new Dictionary<string, List<float>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Lade hier evtl. schon bestTimeDict aus dem SaveManager
        LoadBestTimes();
        LoadCurrentTimes();
    }

    /// <summary>
    ///  Wird pro Frame aufgerufen, summiert Zeit für jedes Level
    /// </summary>
    private void Update()
    {
        // Für alle Levels, die getrackt werden, Zeit hochzählen
        List<string> keys = new List<string>(currentTimeDict.Keys);
        foreach (string level in keys)
        {
            currentTimeDict[level] += Time.deltaTime;
        }
    }

    /// <summary>
    ///  Timer für ein bestimmtes Level starten
    ///  Setzt in currentTimeDict den Wert auf 0
    /// </summary>
    public void StartTimer(string levelName)
    {
        // if (!currentTimeDict.ContainsKey(levelName))
        //     currentTimeDict[levelName] = 0f;
        currentTimeDict[levelName] = 0f;
    }

    /// <summary>
    ///  Timer für ein bestimmtes Level stoppen und in Bestzeiten eintragen
    /// </summary>
    public void StopTimer(string levelName)
    {
        //Check if we actually measured the time
        if (!currentTimeDict.ContainsKey(levelName)) return;

        float finalTime = currentTimeDict[levelName];

        // Falls für dieses Level noch keine Liste existiert, neu anlegen
        if (!bestTimesDict.ContainsKey(levelName))
        {
            bestTimesDict[levelName] = new List<float>();
        }

        // Neue Zeit hinzufügen und sortieren
        bestTimesDict[levelName].Add(finalTime);
        bestTimesDict[levelName].Sort();

        //Nur die Top 5 behalten
        if (bestTimesDict[levelName].Count > 5)
        {
            bestTimesDict[levelName].RemoveRange(5, bestTimesDict[levelName].Count - 5);
        }

        // Save nach jedem Update (oder seltener, je nach Bedarf)
        SaveBestTimes();
    }

    /// <summary>
    ///  Liefert die aktuell laufende Zeit für das gegebene Level
    /// </summary>
    public float GetCurrentTime(string levelName)
    {
        if (!currentTimeDict.ContainsKey(levelName)) return 0f;
        return currentTimeDict[levelName];
    }

    /// <summary>
    ///  Gibt die Liste der Bestzeiten für ein bestimmtes Level zurück
    /// </summary>
    public List<float> GetBestTimes(string levelName)
    {
        if (!bestTimesDict.ContainsKey(levelName))
        {
            // Falls noch keine Liste existiert, leere Liste anlegen
            bestTimesDict[levelName] = new List<float>();
        }
        return bestTimesDict[levelName];
    }

#region Getter/Setter für SaveManager

    /// <summary>
    /// Wird vom SaveManager aufgerufen, um alle aktuellen Zeiten
    /// (currentTimeDict) ins GameData zu speichern.
    /// </summary>
    public Dictionary<string, float> GetAllCurrentTimes()
    {
        return currentTimeDict;
    }

    /// <summary>
    /// Wird vom SaveManager aufgerufen, um alle vorhandenen aktuellen Zeiten
    /// zu überschreiben/setzen.
    /// </summary>
    public void SetAllCurrentTimes(Dictionary<string, float> newCurrentTimes)
    {
        currentTimeDict = newCurrentTimes;
    }

    /// <summary>
    /// Wird vom SaveManager aufgerufen, um alle Bestzeiten ins GameData zu speichern.
    /// </summary>
    public Dictionary<string, List<float>> GetAllBestTimes()
    {
        return bestTimesDict;
    }

    /// <summary>
    /// Wird vom SaveManager aufgerufen, um alle vorhandenen Bestzeiten
    /// zu überschreiben/setzen.
    /// </summary>
    public void SetAllBestTimes(Dictionary<string, List<float>> newBestTimes)
    {
        bestTimesDict = newBestTimes;
    }

    #endregion

    #region Laden und Speichern

    /// <summary>
    /// Lädt aus der GameData die Dictionarys "bestLevelTimes"
    /// und schreibt sie in bestTimesDict.
    /// </summary>
    private void LoadBestTimes()
    {
        // Wir holen uns das GameData vom SaveManager
        GameData gameData = SaveManager.Instance.LoadGameData();
        if (gameData != null && gameData.bestLevelTimes != null)
        {
            bestTimesDict = gameData.bestLevelTimes;
        }
        else
        {
            bestTimesDict = new Dictionary<string, List<float>>();
        }
    }

    /// <summary>
    /// Lädt aus der GameData die "currentLevelTimes"
    /// und schreibt sie in currentTimeDict.
    /// </summary>
    private void LoadCurrentTimes()
    {
        GameData gameData = SaveManager.Instance.LoadGameData();
        if (gameData != null && gameData.currentLevelTimes != null)
        {
            currentTimeDict = gameData.currentLevelTimes;
        }
        else
        {
            currentTimeDict = new Dictionary<string, float>();
        }
    }

    /// <summary>
    /// Speichert die aktuellen Bestzeiten in GameData, ohne dabei andere Felder zu überschreiben.
    /// </summary>
    private void SaveBestTimes()
    {
        // Aktuelles GameData holen, ggf. neu anlegen
        GameData currentData = SaveManager.Instance.LoadGameData();
        if (currentData == null)
            currentData = new GameData();

        currentData.bestLevelTimes = bestTimesDict;

        // Im SaveManager abspeichern
        SaveManager.Instance.SaveGameData(currentData);
    }

    /// <summary>
    /// Falls du die aktuellen Zeiten (currentTimeDict) separat speichern willst,
    /// kannst du hier analog zur Methode SaveBestTimes() vorgehen.
    /// </summary>
    private void SaveCurrentTimes()
    {
        GameData currentData = SaveManager.Instance.LoadGameData();
        if (currentData == null)
            currentData = new GameData();

        currentData.currentLevelTimes = currentTimeDict;

        SaveManager.Instance.SaveGameData(currentData);
    }

    #endregion
}