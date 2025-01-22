using UnityEngine;

public class LeaderboardTrigger : MonoBehaviour
{
    [SerializeField] private string levelName = "Scene_01";
    [SerializeField] private LeaderboardUI leaderboardUI; 

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            triggered = true;

            // Timer stoppen, Zeit abholen
            float finalTime = TimerSystem.Instance.StopTimer(levelName);

            // Leaderboard anzeigen
            if (leaderboardUI != null)
            {
                leaderboardUI.ShowLeaderboard(finalTime);
            }
            else
            {
                Debug.LogWarning("LeaderboardTrigger: leaderboardUI not assigned!");
            }

            //TimerSystem.Instance.AddLeaderboardEntry(levelName, finalTime, enteredName);

        }
    }
}
