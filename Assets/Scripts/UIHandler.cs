using UnityEngine;
using UnityEngine.UIElements;


public class UIHandler : MonoBehaviour
{
    private VisualElement m_Healthbar;
    private TextField m_HealthText;
    public static UIHandler instance { get; private set; }


    // Awake is called when the script instance is being loaded (in this situation, when the game scene loads)
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("Could not find UIDocument component");
            return;
        }
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        m_HealthText = uiDocument.rootVisualElement.Q<TextField>("HealthText");
        SetHealthValue(1.0f);
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    public void SetHealthText(string text)
    {
        m_HealthText.SetValueWithoutNotify(text);
    }
}