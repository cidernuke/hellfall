using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AltarGUI : MonoBehaviour
{
    private bool active = false;
    [SerializeField] private CanvasGroup guiElement;
    private PlayerController playerController;
        
    private Button[] buttons;
    private TextMeshProUGUI[] textValueElements;
    

    private void Start()
    {       
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        textValueElements = GetComponentsInChildren<TextMeshProUGUI>();        
        buttons = GetComponentsInChildren<Button>();
        SetBaseValues();        

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }

        
        
    }


    public void toggleAltarGUI()
    {
        if (active)
        {
            Time.timeScale = 1;
            guiElement.alpha = 0;
            active = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }
        else
        {
            Time.timeScale = 0;
            guiElement.alpha = 1;
            active = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnButtonClick(Button button)
    {
        Debug.Log("Button clicked: " + button.name);
        if(GameManager.Instance.soulShardSystem.decreaseSoulShard(null,1))
        {
            foreach (TextMeshProUGUI textElement in textValueElements)
            {
                if (textElement.name == "LevelValueText")
                {
                    updateValueText(textElement);
                }
                
            }

        }
    }

   private void updateValueText(TextMeshProUGUI textElement)
    {

        /*
        Das Text-Element des Stat Levels wird geupdated.
        das Text-Element der zum Stat gehörigen werte wird geupdated.
        */
        if(textElement == null)
        {
            Debug.Log("textElement is null");
        }        
            textElement.text = playerController.playerStats.vitality.GetBaseValue().ToString();
            //textElement.text = playerController.playerStats.maxHealth.GetBaseValue().ToString();
    }

    private void SetBaseValues(){
        foreach (TextMeshProUGUI textElement in textValueElements)
        {
            
                            
            if (textElement.name == "VitalityValueText")
            {
                Debug.Log("VitalityValueText : " + playerController.playerStats.vitality.GetBaseValue());
                textElement.text = playerController.playerStats.vitality.GetBaseValue().ToString();
            }
            if(textElement.name == "maxHealtValueText")
            {
                textElement.text = playerController.playerStats.maxHealth.GetBaseValue().ToString();
            }
        }
    }

    


}
