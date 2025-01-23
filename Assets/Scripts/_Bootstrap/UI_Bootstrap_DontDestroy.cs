using UnityEngine;

public class UI_Bootstrap_DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        // Prevent Bootstrap from existing multiple times
        var existingUI = FindObjectsOfType<UI_Bootstrap_DontDestroy>();
        if (existingUI.Length > 1)
        {
            // Destroy if a Bootstrap already exists
            Destroy(gameObject);
            return;
        }

        // Mark this GameObject (and its children) as "dont destroy"
        DontDestroyOnLoad(gameObject);
    }
}