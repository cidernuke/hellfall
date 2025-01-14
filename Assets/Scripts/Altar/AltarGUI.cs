using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AltarGUI : MonoBehaviour
{
    private bool active = false;
    [SerializeField] private CanvasGroup guiElement;    
    private Button[] buttons;
    private TextMeshProUGUI[] textValueElements;

    private void Start()
    {
        // Setzen Sie den Button-Listener
        buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
        
        textValueElements = GetComponentsInChildren<TextMeshProUGUI>();         
        
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
                if (textElement.name == button.name + "ValueText")
                {
                    updateValueText(textElement);
                }
            }

        }
    }

   private void updateValueText(TextMeshProUGUI textElement)
    {
        int count = int.Parse(textElement.text);
        count++;
        textElement.text = count.ToString();
    }

    


}
