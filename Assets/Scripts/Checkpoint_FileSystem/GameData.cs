using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public PlayerData playerData;
    // public EnviromentData enviromentData;
    
    public Dictionary<string, List<LeaderboardEntry>> bestLevelTimes;
    public Dictionary<string, float> currentLevelTimes;
}