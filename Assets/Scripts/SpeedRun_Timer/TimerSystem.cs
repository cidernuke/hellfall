using System.Collections.Generic;
using UnityEngine;

public class TimerSystem : MonoBehaviour
{
    public static TimerSystem Instance { get; private set; }

    // Running time per level
    private Dictionary<string, float> currentTimeDict = new Dictionary<string, float>();

    // Leaderboard entries per level: time + player name
    private Dictionary<string, List<LeaderboardEntry>> bestTimesDict = new Dictionary<string, List<LeaderboardEntry>>();

    [Header("Put level for example values here")]
    [SerializeField] string level = "Scene_02";
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load existing data if available
        LoadBestTimes();
        LoadCurrentTimes();
    }

    private void Update()
    {
        // Increment all levels every frame
        List<string> keys = new List<string>(currentTimeDict.Keys);
        foreach (string level in keys)
        {
            currentTimeDict[level] += Time.deltaTime;
        }
    }

    // ------------------- Timer Methods -------------------

    /// <summary>
    /// Starts the timer for a specific level.
    /// </summary>
    public void StartTimer(string levelName)
    {
        currentTimeDict[levelName] = 0f;
    }

    /// <summary>
    /// Stops the timer for a specific level and returns the final time.
    /// </summary>
    public float StopTimer(string levelName)
    {
        if (!currentTimeDict.ContainsKey(levelName))
            return 0f;

        float finalTime = currentTimeDict[levelName];
        return finalTime;
    }

    // ------------------- Leaderboard Methods -------------------

    /// <summary>
    /// Adds a new entry to the leaderboard (bestTimesDict).
    /// Sorts by time and keeps only the top 5.
    /// </summary>
    public void AddLeaderboardEntry(string levelName, float time, string playerName)
    {
        if (!bestTimesDict.ContainsKey(levelName))
        {
            bestTimesDict[levelName] = new List<LeaderboardEntry>();
        }

        var newEntry = new LeaderboardEntry(time, playerName);
        bestTimesDict[levelName].Add(newEntry);

        // Sort by time ascending
        bestTimesDict[levelName].Sort((a,b) => a.time.CompareTo(b.time));

        // Top 5
        if (bestTimesDict[levelName].Count > 5)
        {
            bestTimesDict[levelName].RemoveRange(5, bestTimesDict[levelName].Count - 5);
        }

        SaveBestTimes();
    }

    /// <summary>
    /// Returns the list (time + name) for a specific level.
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
    /// Returns all best times (e.g., for saving).
    /// </summary>
    public Dictionary<string, List<LeaderboardEntry>> GetAllBestTimes()
    {
        return bestTimesDict;
    }

    /// <summary>
    /// Sets all best times from the SaveManager.
    /// </summary>
    public void SetAllBestTimes(Dictionary<string, List<LeaderboardEntry>> newBestTimes)
    {
        bestTimesDict = newBestTimes;
    }

    /// <summary>
    /// Returns the currently running time for the level (only 1 value).
    /// </summary>
    public float GetCurrentTime(string levelName)
    {
        if (!currentTimeDict.ContainsKey(levelName))
            return 0f;
        return currentTimeDict[levelName];
    }

    // ------------------- Internal Load/Save -------------------

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

        if (!bestTimesDict.ContainsKey(level) || bestTimesDict[level].Count == 0)
        {
            bestTimesDict[level] = GetDefaultEntries();
        }
    }

    private List<LeaderboardEntry> GetDefaultEntries()
    {
        List<LeaderboardEntry> defaultEntries = new List<LeaderboardEntry>();
        defaultEntries.Add(new LeaderboardEntry(12.34f, "Markus"));
        defaultEntries.Add(new LeaderboardEntry(14.10f, "Otis"));
        defaultEntries.Add(new LeaderboardEntry(15.89f, "Marinus"));
        defaultEntries.Add(new LeaderboardEntry(20.05f, "Lukas"));
        defaultEntries.Add(new LeaderboardEntry(25.50f, "Christoph"));
        return defaultEntries;
    }

    private void SaveBestTimes()
    {
        GameData currentData = SaveManager.Instance.LoadGameData();
        if (currentData == null)
            currentData = new GameData();

        currentData.bestLevelTimes = bestTimesDict;

        SaveManager.Instance.SaveGameData(currentData);
    }

    // ------------------- Current Times (per level) -------------------

    /// <summary>
    /// Loads the current times for all levels from the SaveManager.
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
    /// Saves the current times for all levels to the SaveManager.
    /// </summary>
    private void SaveCurrentTimes()
    {
        GameData currentData = SaveManager.Instance.LoadGameData();
        if (currentData == null)
            currentData = new GameData();

        currentData.currentLevelTimes = currentTimeDict;
        SaveManager.Instance.SaveGameData(currentData);
    }

    /// <summary>
    /// Returns all current times for all levels.
    /// </summary>
    public Dictionary<string, float> GetAllCurrentTimes()
    {
        return currentTimeDict;
    }

    /// <summary>
    /// Sets all current times for all levels.
    /// </summary>
    public void SetAllCurrentTimes(Dictionary<string, float> newCurrentTimes)
    {
        currentTimeDict = newCurrentTimes;
    }
}