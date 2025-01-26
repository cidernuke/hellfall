
using UnityEngine;

public class Audio_Bootstrap_DontDestroy : MonoBehaviour
{
   private void Awake()
    {
        //Verhindern, dass Bootstrap mehrfach existiert
        var existingUI = FindObjectsOfType<Audio_Bootstrap_DontDestroy>();
        if (existingUI.Length > 1)
        {
            // Zerstören, falls schon ein Bootstrap da ist
            Destroy(gameObject);
            return;
        }

        //Markiert dieses ganze GameObject (und seine Kinder) als "dont destroy"
        DontDestroyOnLoad(gameObject);
    }
}
