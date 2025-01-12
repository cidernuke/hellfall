using UnityEngine;

public class AltarGUI : MonoBehaviour
{
    private bool active = false;
    [SerializeField] private CanvasGroup guiElement;  
     

    public void toggleAltarGUI()
    {
        if(active)
        {
            Time.timeScale = 1;
            guiElement.alpha = 0;
            active = false;
        }
        else
        {
            Time.timeScale = 0;
            guiElement.alpha = 1;
            active = true;
        }
    }
    
    

}
