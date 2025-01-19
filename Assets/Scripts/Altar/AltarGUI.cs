using System;
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

    private String[] textElementNames = new string[] 
    { "VitalityValueText", "maxHealtValueText", "StrengthValueText",
      "damageValueText", "IntelligenceValueText", "rangeDamageValueText",
      "cooldownValueText", "rangedRangeValueText" 
    };
    

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

    /// <summary>
    /// Toggles the Altar GUI on and off.
    /// Sets time scale to 0 when active.
    /// Sets time scale to 1 when inactive
    /// </summary>
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
        if(button.name == "PlusButton")
        {
            Debug.Log("PlusButton clicked: " + button.name);
            if(GameManager.Instance.soulShardSystem.decreaseSoulShard(null,1))
            {
                foreach (TextMeshProUGUI textElement in textValueElements)
                {                    

                    if (textElement.name == "VitalityValueText")
                    {
                        
                        updateValueText(textElement);            
                        
                    }
                    if (textElement.name == "StrengthValueText")
                    {
                        updateValueText(textElement);            
                        
                    }
                    if (textElement.name == "IntelligenceValueText")
                    {
                        updateValueText(textElement);
                    }                   
                                 
                    
                }

            }
            
        }
        else if(button.name == "MinusButton")
        {
            Debug.Log("MinusButton clicked: " + button.name);            
        }
    }

    /// <summary>
    /// Updates the value of the text element
    /// </summary>
    /// <param name="textElement"></param>
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
        switch (textElement.name)
        {
            case "VitalityValueText":
                playerController.playerStats.vitality.SetBaseValue(1);
                textElement.text = playerController.playerStats.vitality.GetBaseValue().ToString();
                break;
            case "StrengthValueText":
                playerController.playerStats.strength.SetBaseValue(1);
                textElement.text = playerController.playerStats.strength.GetBaseValue().ToString();
                break;
            case "IntelligenceValueText":
                playerController.playerStats.intelligence.SetBaseValue(1);
                textElement.text = playerController.playerStats.intelligence.GetBaseValue().ToString();
                break;
            default:
                Debug.Log("No matching textElement found");
                break;
        }                
            
    }

    /// <summary>
    /// Set the base values of the player stats to the text elements in the Altar GUI
    /// </summary>
    private void SetBaseValues(){
        foreach (TextMeshProUGUI textElement in textValueElements)
        {         
            /// Health values                
            if (textElement.name == "VitalityValueText")
            {                
                textElement.text = playerController.playerStats.vitality.GetBaseValue().ToString();
            }
            if(textElement.name == "maxHealtValueText")
            {
                textElement.text = playerController.playerStats.maxHealth.GetBaseValue().ToString();
            }

            /// Closerange attack values
            if (textElement.name == "StrengthValueText")
            {                
                textElement.text = playerController.playerStats.strength.GetBaseValue().ToString();
            }
            if (textElement.name == "damageValueText")
            {                
                textElement.text = playerController.playerStats.closeDamage.GetBaseValue().ToString();
            }

            /// Ranged attack values
            if (textElement.name == "IntelligenceValueText")
            {                
                textElement.text = playerController.playerStats.intelligence.GetBaseValue().ToString();
            }            
            if (textElement.name == "rangeDamageValueText")
            {                
                textElement.text = playerController.playerStats.rangedDamage.GetBaseValue().ToString();
            }            
            if (textElement.name == "cooldownValueText")
            {                
                textElement.text = playerController.playerStats.rangedCooldown.GetBaseValue().ToString();
            }
            if (textElement.name == "rangedRangeValueText")
            {                
                textElement.text = playerController.playerStats.rangedRange.GetBaseValue().ToString();
            }
        }
    }

    


}
