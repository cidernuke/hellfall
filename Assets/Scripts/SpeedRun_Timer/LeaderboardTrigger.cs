using UnityEngine;

public class LeaderboardTrigger : MonoBehaviour
{
    [SerializeField] private string levelName = "Scene_01";
    [SerializeField] private LeaderboardUI leaderboardUI; 

    private bool triggered = false;
    [SerializeField] private EndBossMain endBossMain;

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this object (2D physics only).
    /// </summary>
    /// <param name="collision">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (endBossMain.GetComponent<HealthSystem>().isDead)
        {
            if (!triggered && collision.CompareTag("Player"))
            {
                triggered = true;
    
                // Stop timer, get time
                float finalTime = TimerSystem.Instance.StopTimer(levelName);
    
                // Show leaderboard
                if (leaderboardUI != null)
                {
                    leaderboardUI.ShowLeaderboard(finalTime);
                }
                else
                {
                    Debug.LogWarning("LeaderboardTrigger: leaderboardUI not assigned!");
                }
            }
        }
    }
}
