using UnityEngine;

public class PlayerBootstrap : MonoBehaviour
{
    private static bool playerExists = false;

    void Awake()
    {
        //Check if a player already exist
        if (!playerExists)
        {
            playerExists = true;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            //In case there is a second player -> destroy
            Destroy(gameObject);
        }
    }
}
