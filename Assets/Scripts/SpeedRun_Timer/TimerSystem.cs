using System.Collections.Generic;
using UnityEngine;

public class TimerSystem : MonoBehaviour
{
    public static TimerSystem Instance { get; private set; }

    // Laufende Zeit pro Level
    private Dictionary<string, float> currentTimeDict = new Dictionary<string, float>();

    // Leaderboard-Einträge pro Level: Zeit + Spielername
    private Dictionary<string, List<LeaderboardEntry>> bestTimesDict = 
        new Dictionary<string, List<LeaderboardEntry>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Vorhandene Daten laden, falls vorhanden
        LoadBestTimes();
        LoadCurrentTimes();
    }

    private void Update()
    {
        // Jeden Frame alle Levels hochzählen
        List<string> keys = new List<string>(currentTimeDict.Keys);
        foreach (string level in keys)
        {
            currentTimeDict[level] += Time.deltaTime;
        }
    }

    // ------------------- Timer-Methoden -------------------

    public void StartTimer(string levelName)
    {
        currentTimeDict[levelName] = 0f;
    }

    public float StopTimer(string levelName)
    {
        if (!currentTimeDict.ContainsKey(levelName))
            return 0f;

        float finalTime = currentTimeDict[levelName];
        return finalTime;
    }

    // ------------------- Leaderboard-Methoden -------------------

    /// <summary>
    /// Fügt dem Leaderboard (bestTimesDict) einen neuen Eintrag hinzu.
    /// Sortiert nach Zeit und behält nur die Top 5.
    /// </summary>
    public void AddLeaderboardEntry(string levelName, float time, string playerName)
    {
        if (!bestTimesDict.ContainsKey(levelName))
        {
            bestTimesDict[levelName] = new List<LeaderboardEntry>();
        }

        var newEntry = new LeaderboardEntry(time, playerName);
        bestTimesDict[levelName].Add(newEntry);

        // Sortieren nach Zeit aufsteigend
        bestTimesDict[levelName].Sort((a,b) => a.time.CompareTo(b.time));

        // Top 5
        if (bestTimesDict[levelName].Count > 5)
        {
            bestTimesDict[levelName].RemoveRange(5, bestTimesDict[levelName].Count - 5);
        }

        SaveBestTimes();
    }

    /// <summary>
    /// Gibt die Liste (Zeit+Name) für ein bestimmtes Level zurück.
    /// </summary>
    public List<LeaderboardEntry> GetBestEntries(string levelName)
    {
        if (!bestTimesDict.ContainsKey(levelName))
        {
            bestTimesDict[levelName] = new List<LeaderboardEntry>();
        }
        return bestTimesDict[levelName];
    }

    /// <summary>
    /// Falls du ALLE BestTimes brauchst (z. B. beim Speichern).
    /// </summary>
    public Dictionary<string, List<LeaderboardEntry>> GetAllBestTimes()
    {
        return bestTimesDict;
    }

    /// <summary>
    /// Falls du ALLE BestTimes vom SaveManager laden willst.
    /// </summary>
    public void SetAllBestTimes(Dictionary<string, List<LeaderboardEntry>> newBestTimes)
    {
        bestTimesDict = newBestTimes;
    }

    /// <summary>
    /// Liefert die aktuell laufende Zeit für das Level (nur 1 Wert).
    /// </summary>
    public float GetCurrentTime(string levelName)
    {
        if (!currentTimeDict.ContainsKey(levelName))
            return 0f;
        return currentTimeDict[levelName];
    }

    // ------------------- Laden/Speichern intern -------------------

    private void LoadBestTimes()
    {
        GameData gameData = SaveManager.Instance.LoadGameData();
        if (gameData != null && gameData.bestLevelTimes != null)
        {
            bestTimesDict = gameData.bestLevelTimes;
        }
        else
        {
            bestTimesDict = new Dictionary<string, List<LeaderboardEntry>>();
        }
    }

    private void SaveBestTimes()
    {
        GameData currentData = SaveManager.Instance.LoadGameData();
        if (currentData == null)
            currentData = new GameData();

        currentData.bestLevelTimes = bestTimesDict;

        SaveManager.Instance.SaveGameData(currentData);
    }

    // ------------------- Current Times (pro Level) -------------------

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

    private void SaveCurrentTimes()
    {
        GameData currentData = SaveManager.Instance.LoadGameData();
        if (currentData == null)
            currentData = new GameData();

        currentData.currentLevelTimes = currentTimeDict;
        SaveManager.Instance.SaveGameData(currentData);
    }

    // **Diese beiden Method**en rufen dein Save/Load an:
    public Dictionary<string, float> GetAllCurrentTimes()
    {
        return currentTimeDict;
    }

    public void SetAllCurrentTimes(Dictionary<string, float> newCurrentTimes)
    {
        currentTimeDict = newCurrentTimes;
    }
}


//_______VERSION 2_____________

// using System.Collections.Generic;
// using UnityEngine;

// public class TimerSystem : MonoBehaviour
// {
//     public static TimerSystem Instance { get; private set; }

//     /// <summary>
//     ///  Hält die aktuell laufende Zeit pro Level. (Wie gehabt)
//     /// </summary>
//     private Dictionary<string, float> currentTimeDict = new Dictionary<string, float>();

//     /// <summary>
//     ///  Hält eine Liste an Leaderboard-Einträgen (Zeit+Name) pro Level.
//     ///  Key: LevelName, Value: Liste mit LeaderboardEntry
//     /// </summary>
//     private Dictionary<string, List<LeaderboardEntry>> bestTimesDict 
//         = new Dictionary<string, List<LeaderboardEntry>>();

//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//         DontDestroyOnLoad(gameObject);

//         // Lade ggf. vorhandene Daten
//         LoadBestTimes(); 
//         LoadCurrentTimes();
//     }

//     private void Update()
//     {
//         // Pro Frame Zeit hochzählen
//         List<string> keys = new List<string>(currentTimeDict.Keys);
//         foreach (string level in keys)
//         {
//             currentTimeDict[level] += Time.deltaTime;
//         }
//     }

//     /// <summary>
//     /// Timer starten (setzt Zeit auf 0).
//     /// </summary>
//     public void StartTimer(string levelName)
//     {
//         currentTimeDict[levelName] = 0f;
//     }

//     /// <summary>
//     /// Timer stoppen und float finalTime zurückgeben,
//     /// ABER noch nicht ins Leaderboard schreiben – 
//     /// das macht man, sobald der Spieler seinen Namen eingetippt hat.
//     /// </summary>
//     public float StopTimer(string levelName)
//     {
//         if (!currentTimeDict.ContainsKey(levelName))
//             return 0f;

//         float finalTime = currentTimeDict[levelName];
//         return finalTime;
//     }

//     /// <summary>
//     /// Fügt dem Leaderboard (bestTimesDict) einen neuen Eintrag hinzu,
//     /// sortiert und kürzt ggf. auf Top 5.
//     /// </summary>
//     public void AddLeaderboardEntry(string levelName, float time, string playerName)
//     {
//         if (!bestTimesDict.ContainsKey(levelName))
//         {
//             bestTimesDict[levelName] = new List<LeaderboardEntry>();
//         }

//         // Eintrag hinzufügen
//         var newEntry = new LeaderboardEntry(time, playerName);
//         bestTimesDict[levelName].Add(newEntry);

//         // Sortieren nach Zeit
//         bestTimesDict[levelName].Sort((a,b) => a.time.CompareTo(b.time));

//         // Nur Top 5
//         if (bestTimesDict[levelName].Count > 5)
//         {
//             bestTimesDict[levelName].RemoveRange(5, bestTimesDict[levelName].Count - 5);
//         }

//         SaveBestTimes();
//     }

//     /// <summary>
//     /// Liefert die Liste der Leaderboard-Einträge für ein Level.
//     /// </summary>
//     public List<LeaderboardEntry> GetBestEntries(string levelName)
//     {
//         if (!bestTimesDict.ContainsKey(levelName))
//         {
//             bestTimesDict[levelName] = new List<LeaderboardEntry>();
//         }
//         return bestTimesDict[levelName];
//     }

//     /// <summary>
//     /// Gibt die zuletzt gemessene Zeit für das Level zurück (z.B. für UI-Anzeige)
//     /// </summary>
//     public float GetCurrentTime(string levelName)
//     {
//         if (!currentTimeDict.ContainsKey(levelName)) 
//             return 0f;
//         return currentTimeDict[levelName];
//     }

//     #region Laden/Speichern

//     private void LoadBestTimes()
//     {
//         GameData gameData = SaveManager.Instance.LoadGameData();
//         if (gameData != null && gameData.bestLevelTimes != null)
//         {
//             bestTimesDict = gameData.bestLevelTimes;
//         }
//         else
//         {
//             bestTimesDict = new Dictionary<string, List<LeaderboardEntry>>();
//         }
//     }

//     private void SaveBestTimes()
//     {
//         GameData currentData = SaveManager.Instance.LoadGameData();
//         if (currentData == null)
//             currentData = new GameData();

//         currentData.bestLevelTimes = bestTimesDict;

//         SaveManager.Instance.SaveGameData(currentData);
//     }

//     private void LoadCurrentTimes()
//     {
//         GameData gameData = SaveManager.Instance.LoadGameData();
//         if (gameData != null && gameData.currentLevelTimes != null)
//         {
//             currentTimeDict = gameData.currentLevelTimes;
//         }
//         else
//         {
//             currentTimeDict = new Dictionary<string, float>();
//         }
//     }

//     private void SaveCurrentTimes()
//     {
//         GameData currentData = SaveManager.Instance.LoadGameData();
//         if (currentData == null)
//             currentData = new GameData();

//         currentData.currentLevelTimes = currentTimeDict;

//         SaveManager.Instance.SaveGameData(currentData);
//     }

//     #endregion
// }




//_________VERSION 1________________


// using System.Collections.Generic;
// using UnityEngine;

// public class TimerSystem : MonoBehaviour
// {
//     public static TimerSystem Instance { get; private set; }

//     /// <summary>
//     ///  Hält die aktuell laufende Zeit pro Level.
//     ///  Key: LevelName, Value: Aktuell hochgezählte Zeit
//     /// </summary>
//     private Dictionary<string, float> currentTimeDict = new Dictionary<string, float>();

//     /// <summary>
//     ///  Hält eine Liste an Leaderboard-Einträgen (Zeit+Name) pro Level.
//     ///  Key: LevelName, Value: Liste mit LeaderboardEntry
//     /// </summary>
//     private Dictionary<string, List<LeaderboardEntry>> bestTimesDict = new Dictionary<string, List<LeaderboardEntry>>();
//     //private Dictionary<string, List<float>> bestTimesDict = new Dictionary<string, List<float>>();


//     private void Awake()
//     {
//         if (Instance != null && Instance != this)
//         {
//             Destroy(gameObject);
//             return;
//         }
//         Instance = this;
//         DontDestroyOnLoad(gameObject);

//         // Lade hier evtl. schon bestTimeDict aus dem SaveManager
//         LoadBestTimes();
//         LoadCurrentTimes();
//     }

//     /// <summary>
//     ///  Wird pro Frame aufgerufen, summiert Zeit für jedes Level
//     /// </summary>
//     private void Update()
//     {
//         // Für alle Levels, die getrackt werden, Zeit hochzählen
//         List<string> keys = new List<string>(currentTimeDict.Keys);
//         foreach (string level in keys)
//         {
//             currentTimeDict[level] += Time.deltaTime;
//         }
//     }

//     /// <summary>
//     ///  Timer für ein bestimmtes Level starten
//     ///  Setzt in currentTimeDict den Wert auf 0
//     /// </summary>
//     public void StartTimer(string levelName)
//     {
//         currentTimeDict[levelName] = 0f;
//     }

//     /// <summary>
//     /// Timer stoppen und float finalTime zurückgeben,
//     /// ABER noch nicht ins Leaderboard schreiben – 
//     /// das macht man, sobald der Spieler seinen Namen eingetippt hat.
//     /// </summary>
//     public float StopTimer(string levelName)
//     {
//         //Check if we actually measured the time
//         if (!currentTimeDict.ContainsKey(levelName))
//         {
//             return 0f;
//         }

//         float finalTime = currentTimeDict[levelName];
//         return finalTime;

//         // Falls für dieses Level noch keine Liste existiert, neu anlegen
//         // if (!bestTimesDict.ContainsKey(levelName))
//         // {
//         //     bestTimesDict[levelName] = new List<float>();
//         // }

//         // // Neue Zeit hinzufügen und sortieren
//         // bestTimesDict[levelName].Add(finalTime);
//         // bestTimesDict[levelName].Sort();

//         // //Nur die Top 5 behalten
//         // if (bestTimesDict[levelName].Count > 5)
//         // {
//         //     bestTimesDict[levelName].RemoveRange(5, bestTimesDict[levelName].Count - 5);
//         // }

//         // // Save nach jedem Update (oder seltener, je nach Bedarf)
//         // SaveBestTimes();
//     }

//     /// <summary>
//     /// Fügt dem Leaderboard (bestTimesDict) einen neuen Eintrag hinzu,
//     /// sortiert und kürzt ggf. auf Top 5.
//     /// </summary>
//     public void AddLeaderboardEntry(string levelName, float time, string playerName)
//     {
//         if (!bestTimesDict.ContainsKey(levelName))
//         {
//             bestTimesDict[levelName] = new List<LeaderboardEntry>();
//         }

//         // Eintrag hinzufügen
//         var newEntry = new LeaderboardEntry(time, playerName);
//         bestTimesDict[levelName].Add(newEntry);

//         // Sortieren nach Zeit
//         bestTimesDict[levelName].Sort((a, b) => a.time.CompareTo(b.time));

//         // Nur Top 5
//         if (bestTimesDict[levelName].Count > 5)
//         {
//             bestTimesDict[levelName].RemoveRange(5, bestTimesDict[levelName].Count - 5);
//         }

//         SaveBestTimes();
//     }

//     /// <summary>
//     /// Liefert die Liste der Leaderboard-Einträge für ein Level.
//     /// </summary>
//     public List<LeaderboardEntry> GetBestEntries(string levelName)
//     {
//         if (!bestTimesDict.ContainsKey(levelName))
//         {
//             bestTimesDict[levelName] = new List<LeaderboardEntry>();
//         }
//         return bestTimesDict[levelName];
//     }

//     /// <summary>
//     ///  Liefert die aktuell laufende Zeit für das gegebene Level
//     /// </summary>
//     public float GetCurrentTime(string levelName)
//     {
//         if (!currentTimeDict.ContainsKey(levelName))
//             return 0f;

//         return currentTimeDict[levelName];
//     }

//     /// <summary>
//     ///  Gibt die Liste der Bestzeiten für ein bestimmtes Level zurück
//     /// </summary>
//     // public List<float> GetBestTimes(string levelName)
//     // {
//     //     if (!bestTimesDict.ContainsKey(levelName))
//     //     {
//     //         // Falls noch keine Liste existiert, leere Liste anlegen
//     //         bestTimesDict[levelName] = new List<float>();
//     //     }
//     //     return bestTimesDict[levelName];
//     // }

//     private void LoadBestTimes()
//     {
//         GameData gameData = SaveManager.Instance.LoadGameData();
//         if (gameData != null && gameData.bestLevelTimes != null)
//         {
//             bestTimesDict = gameData.bestLevelTimes;
//         }
//         else
//         {
//             bestTimesDict = new Dictionary<string, List<LeaderboardEntry>>();
//         }
//     }

//     private void LoadCurrentTimes()
//     {
//         GameData gameData = SaveManager.Instance.LoadGameData();
//         if (gameData != null && gameData.currentLevelTimes != null)
//         {
//             currentTimeDict = gameData.currentLevelTimes;
//         }
//         else
//         {
//             currentTimeDict = new Dictionary<string, float>();
//         }
//     }

//     private void SaveCurrentTimes()
//     {
//         GameData currentData = SaveManager.Instance.LoadGameData();
//         if (currentData == null)
//             currentData = new GameData();

//         currentData.currentLevelTimes = currentTimeDict;

//         SaveManager.Instance.SaveGameData(currentData);
//     }

//     // private void SaveBestTimes()
//     // {
//     //     GameData currentData = SaveManager.Instance.LoadGameData();
//     //     if (currentData == null)
//     //         currentData = new GameData();

//     //     currentData.bestLevelTimes = bestTimesDict;

//     //     SaveManager.Instance.SaveGameData(currentData);
//     // }

//     // #region Laden und Speichern

//     // /// <summary>
//     // /// Lädt aus der GameData die Dictionarys "bestLevelTimes"
//     // /// und schreibt sie in bestTimesDict.
//     // /// </summary>
//     // private void LoadBestTimes()
//     // {
//     //     // Wir holen uns das GameData vom SaveManager
//     //     GameData gameData = SaveManager.Instance.LoadGameData();
//     //     if (gameData != null && gameData.bestLevelTimes != null)
//     //     {
//     //         bestTimesDict = gameData.bestLevelTimes;
//     //     }
//     //     else
//     //     {
//     //         bestTimesDict = new Dictionary<string, List<float>>();
//     //     }
//     // }

//     // /// <summary>
//     // /// Lädt aus der GameData die "currentLevelTimes"
//     // /// und schreibt sie in currentTimeDict.
//     // /// </summary>
//     // private void LoadCurrentTimes()
//     // {
//     //     GameData gameData = SaveManager.Instance.LoadGameData();
//     //     if (gameData != null && gameData.currentLevelTimes != null)
//     //     {
//     //         currentTimeDict = gameData.currentLevelTimes;
//     //     }
//     //     else
//     //     {
//     //         currentTimeDict = new Dictionary<string, float>();
//     //     }
//     // }

//     // /// <summary>
//     // /// Speichert die aktuellen Bestzeiten in GameData, ohne dabei andere Felder zu überschreiben.
//     // /// </summary>
//     // private void SaveBestTimes()
//     // {
//     //     // Aktuelles GameData holen, ggf. neu anlegen
//     //     GameData currentData = SaveManager.Instance.LoadGameData();
//     //     if (currentData == null)
//     //         currentData = new GameData();

//     //     currentData.bestLevelTimes = bestTimesDict;

//     //     // Im SaveManager abspeichern
//     //     SaveManager.Instance.SaveGameData(currentData);
//     // }

//     // /// <summary>
//     // /// Falls du die aktuellen Zeiten (currentTimeDict) separat speichern willst,
//     // /// kannst du hier analog zur Methode SaveBestTimes() vorgehen.
//     // /// </summary>
//     // private void SaveCurrentTimes()
//     // {
//     //     GameData currentData = SaveManager.Instance.LoadGameData();
//     //     if (currentData == null)
//     //         currentData = new GameData();

//     //     currentData.currentLevelTimes = currentTimeDict;

//     //     SaveManager.Instance.SaveGameData(currentData);
//     // }

//     //#endregion
// }