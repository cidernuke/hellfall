using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    public float time;
    public string playerName;

    public LeaderboardEntry(float t, string n)
    {
        time = t;
        playerName = n;
    }
}