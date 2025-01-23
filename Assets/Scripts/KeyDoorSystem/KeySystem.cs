using UnityEngine;
using UnityEngine.UI;

public class KeySystem : MonoBehaviour
{
    [SerializeField] public int currentKeyCount = 0;
    [SerializeField] private int totalKeysNeeded = 2;  // In each level player must find 3 keys

    [SerializeField] private Text keyCounterText;

    private void Awake()
    {
        // Ensure UI is correct if the scene starts with some key count
        UpdateKeyUI();
    }

    /// <summary>
    /// Sets the key count to a specific amount and updates the UI accordingly.
    /// </summary>
    public void SetKeyCount(int amount)
    {
        currentKeyCount = amount;
        Debug.Log("Key count set to: " + currentKeyCount);
        UpdateKeyUI();
    }


    /// <summary>
    /// Resets the key count to zero and updates the UI.
    /// </summary>
    public void ResetKeyCount()
    {
        SetKeyCount(0);
    }


    /// <summary>
    /// Returns the current number of keys the player holds.
    /// </summary>
    public int GetKeyCount()
    {
        return currentKeyCount;
    }

    /// <summary>
    /// Increments the key count by one and updates the UI.
    /// </summary>
    public void AddKey()
    {
        currentKeyCount++;
        Debug.Log("Key collected! Current: " + currentKeyCount);
        //Add UI-Update/Sound later here
        UpdateKeyUI();
    }

    /// <summary>
    /// Checks if the player has reached or exceeded the total keys needed.
    /// </summary>
    public bool HasAllKeys()
    {
        return currentKeyCount >= totalKeysNeeded;
    }

    /// <summary>
    /// Updates the text UI to reflect the current key count.
    /// </summary>
    private void UpdateKeyUI()
    {
        if (keyCounterText != null)
        {
            keyCounterText.text = currentKeyCount.ToString();
        }
    }
}
