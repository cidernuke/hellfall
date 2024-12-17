using UnityEngine;
using TMPro;

public class DeathUIManager : MonoBehaviour
{
    public static DeathUIManager Instance { get; private set; }

    [SerializeField] private TMP_Text checkpointMessageText;
    [SerializeField] private TMP_Text deathMessageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowCheckpointMessage(string message, float duration)
    {
        if (checkpointMessageText != null)
        {
            checkpointMessageText.text = message;
            checkpointMessageText.gameObject.SetActive(true);
            StartCoroutine(HideAfterDelay(checkpointMessageText, duration, false));
        }
    }

    public void ShowDeathMessage(string message, float duration)
    {
        if (deathMessageText != null)
        {
            deathMessageText.text = message;
            deathMessageText.gameObject.SetActive(true);
            StartCoroutine(HideAfterDelay(deathMessageText, duration, true));
        }
    }

    private System.Collections.IEnumerator HideAfterDelay(TMP_Text textElement, float delay, bool dead)
    {
        yield return new WaitForSeconds(delay);
        if (textElement != null)
            textElement.gameObject.SetActive(false);
    }
}
