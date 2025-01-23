using UnityEngine;
using TMPro;

public class DeathUIManager : MonoBehaviour
{
    public static DeathUIManager Instance { get; private set; }

    [SerializeField] private TMP_Text checkpointMessageText;
    [SerializeField] private TMP_Text deathMessageText;

    /// <summary>
    /// Singleton pattern for the DeathUIManager.
    /// </summary>
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

    /// <summary>
    /// Shows a message when the player reaches a checkpoint.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="duration"></param>
    public void ShowCheckpointMessage(string message, float duration)
    {
        if (checkpointMessageText != null)
        {
            checkpointMessageText.text = message;
            checkpointMessageText.gameObject.SetActive(true);
            StartCoroutine(HideAfterDelay(checkpointMessageText, duration, false));
        }
    }

    /// <summary>
    /// Shows a message when the player dies.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="duration"></param>
    public void ShowDeathMessage(string message, float duration)
    {
        if (deathMessageText != null)
        {
            deathMessageText.text = message;
            deathMessageText.gameObject.SetActive(true);
            StartCoroutine(HideAfterDelay(deathMessageText, duration, true));
        }
    }

    /// <summary>
    /// Hides the message after a certain delay.
    /// </summary>
    /// <param name="textElement"></param>
    /// <param name="delay"></param>
    /// <param name="dead"></param>
    /// <returns></returns>
    private System.Collections.IEnumerator HideAfterDelay(TMP_Text textElement, float delay, bool dead)
    {
        yield return new WaitForSeconds(delay);
        if (textElement != null)
            textElement.gameObject.SetActive(false);
    }
}
