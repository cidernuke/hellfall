using UnityEngine;

public class DontDestroy_Bootstrap : MonoBehaviour
{
    private static bool objectExist = false;

    void Awake()
    {
        //Check if the game object already exist
        if (!objectExist)
        {
            objectExist = true;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            //In case there is exitsting already one -> destroy
            Destroy(gameObject);
        }
    }
}
