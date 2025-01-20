using UnityEngine;

public class LevelEndTrigger : MonoBehaviour
{
    [SerializeField] private string levelName; 
    [SerializeField] private GameObject levelEndUI; 

    private bool levelEnded = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prüfen, ob Player und ob wir nicht schon ausgelöst haben
        if (other.CompareTag("Player") && !levelEnded)
        {
            levelEnded = true;
            
            // Stop timer
            TimerSystem.Instance.StopTimer(levelName);

            // 2) Hier könnte man zusätzlich die Bestzeit aktualisieren oder in TimerSystem.Instance.StopTimer(...) integrieren.

            // 3) Activate UI & show time
            if (levelEndUI != null)
            {
                levelEndUI.SetActive(true);

                float finalTime = TimerSystem.Instance.GetCurrentTime(levelName);

                // Referenz auf das UI-Script holen
                //Get reference of the UI-Script
                LevelEndUI uiScript = levelEndUI.GetComponent<LevelEndUI>();
                if (uiScript != null)
                {
                    uiScript.ShowResults(levelName, finalTime);
                }
                else
                {
                    Debug.LogWarning("LevelEndTrigger: Couldn't find a 'LevelEndUI' script on the UI-Object");
                }
            }
            else
            {
                //Debug.LogWarning("LevelEndTrigger: levelEndUI ist nicht zugewiesen.");
                Debug.LogWarning("LevelEndTrigger: Couldn't find the game-obejct levelEndUI.");
            }
        }
    }
}

