using UnityEngine;

public class KeySystem : MonoBehaviour
{
    [SerializeField] private int currentKeyCount = 0;
    [SerializeField] private int totalKeysNeeded = 3;  // In each level player must find 3 keys

    // Save the keys persistent
    public void SetKeyCount(int amount)
    {
        currentKeyCount = amount;
        Debug.Log("Key count set to: " + currentKeyCount);
    }

    public void ResetKeyCount()
    {
        SetKeyCount(0);
    }

    public int GetKeyCount()
    {
        return currentKeyCount;
    }

    // Increase key-counter by 1
    public void AddKey()
    {
        currentKeyCount++;
        Debug.Log("Key collected! Current: " + currentKeyCount);
        //Add UI-Update/Sound later here
    }

    public bool HasAllKeys()
    {
        return currentKeyCount >= totalKeysNeeded;
    }
}
