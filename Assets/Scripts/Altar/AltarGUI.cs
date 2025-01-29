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
    private Image[] progressBar;
    private int defaultSoulShardCost = 5;

    // Separate costs and multiplier counters for each attribute
    public int vitalityCost = 5;
    public int strengthCost = 5;
    public int intelligenceCost = 5;
    public int vitalityMultiplierCount = 0;
    public int strengthMultiplierCount = 0;
    public int intelligenceMultiplierCount = 0;

    // Arrays zum Speichern der ursprünglichen Preise
    public int[] vitalityCosts = new int[11];
    public int[] strengthCosts = new int[11];
    public int[] intelligenceCosts = new int[11];

    // Modfier values
    private float damageModifier = 10f;
    private float cooldownModifier = 5f;
    private float rangeModifier = 5f;

    //Max Values

    private void Start()
    {
        // Initialize the GUI elements and player controller
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        textValueElements = GetComponentsInChildren<TextMeshProUGUI>();
        progressBar = GetComponentsInChildren<Image>();
        buttons = GetComponentsInChildren<Button>();
        SetBaseValues();
        setBaseProgressBarValues();


        foreach (Button button in buttons)
        {
            if (button.name == "PlusButton1" || button.name == "PlusButton2" || button.name == "PlusButton3")
            {
                button.onClick.AddListener(() => OnPlusButtonClick(button));
            }
            if (button.name == "MinusButton1" || button.name == "MinusButton2" || button.name == "MinusButton3")
            {
                button.onClick.AddListener(() => OnMinusButtonClick(button));
            }
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

    /// <summary>
    /// Updates the soul shard cost for the corresponding attribute
    /// </summary>
    /// <param name="shValueText"></param>
    /// <param name="shCost"></param>
    /// <param name="multiplierCount"></param>
    /// <param name="isIncrement"></param>
    /// <param name="costArray"></param>   
    private void updateSoulShardCost(string shValueText, ref int shCost, ref int multiplierCount, bool isIncrement, int[] costArray)
    {
        if (isIncrement)
        {
            if (multiplierCount < 10)
            {
                costArray[multiplierCount] = shCost; //Save the current price
                shCost += 15;
                multiplierCount++;
            }
        }
        else
        {
            if (multiplierCount > 0)
            {
                multiplierCount--;
                shCost = costArray[multiplierCount]; // Use the saved price
            }
        }

        foreach (TextMeshProUGUI textElement in textValueElements)
        {
            if (textElement.name == shValueText)
            {
                textElement.text = shCost.ToString();
            }
        }
    }

    private bool HasEnoughSoulShards(int cost)
    {
        int currentShards = GameManager.Instance.soulShardSystem.GetSoulShardCount();
        if (currentShards < cost)
        {
            Debug.LogError("Not enough soul shards. Cost is " + cost + ". You have " + currentShards + ".");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Event handler for the plus buttons in the Altar GUI
    /// Increments the value of the corresponding text element by 1
    /// </summary>
    /// <param name="button"></param>
    private void OnPlusButtonClick(Button button)
    {
        foreach (TextMeshProUGUI textElement in textValueElements)
        {
            if (button.name == "PlusButton1")
            {
                if (playerController.playerStats.vitality.GetBaseValue() >= 10)
                {
                    Debug.LogError("Base value is already 10. Cannot increase further.");
                    return;
                }

                if (textElement.name == "VitalityValueText")
                {
                    //Check for enough soul shards
                    if (!HasEnoughSoulShards(vitalityCost)) return;
                    GameManager.Instance.soulShardSystem.decreaseSoulShard(null, vitalityCost);
                    incrementValueText(textElement);
                    updateSoulShardCost("SHValueText1", ref vitalityCost, ref vitalityMultiplierCount, true, vitalityCosts);
                }
            }
            if (button.name == "PlusButton2")
            {
                if (playerController.playerStats.strength.GetBaseValue() >= 10)
                {
                    Debug.LogError("Base value is already 10. Cannot increase further.");
                    return;
                }
                if (textElement.name == "StrengthValueText")
                {
                    //Check for enough soul shards
                    if (!HasEnoughSoulShards(strengthCost)) return;
                    GameManager.Instance.soulShardSystem.decreaseSoulShard(null, strengthCost);
                    incrementValueText(textElement);
                    updateSoulShardCost("SHValueText2", ref strengthCost, ref strengthMultiplierCount, true, strengthCosts);
                }
            }
            if (button.name == "PlusButton3")
            {
                if (playerController.playerStats.intelligence.GetBaseValue() >= 10)
                {
                    Debug.LogError("Base value is already 10. Cannot increase further.");
                    return;
                }
                if (textElement.name == "IntelligenceValueText")
                {
                    //Check for enough soul shards
                    if (!HasEnoughSoulShards(intelligenceCost)) return;
                    GameManager.Instance.soulShardSystem.decreaseSoulShard(null, intelligenceCost);
                    incrementValueText(textElement);
                    updateSoulShardCost("SHValueText3", ref intelligenceCost, ref intelligenceMultiplierCount, true, intelligenceCosts);
                }
            }
        }
    }

    /// <summary>
    /// Event handler for the minus buttons in the Altar GUI   
    /// </summary>
    /// <param name="button"></param>
    private void OnMinusButtonClick(Button button)
    {

        if (playerController.playerStats.vitality.GetBaseValue() > 0 ||
            playerController.playerStats.strength.GetBaseValue() > 0 ||
            playerController.playerStats.intelligence.GetBaseValue() > 0)
        {

            foreach (TextMeshProUGUI textElement in textValueElements)
            {
                if (button.name == "MinusButton1")
                {

                    if (textElement.name == "VitalityValueText")
                    {
                        GameManager.Instance.soulShardSystem.inreaseSoulShard(null, vitalityCosts[vitalityMultiplierCount - 1]);
                        decreaseValueText(textElement);
                        updateSoulShardCost("SHValueText1", ref vitalityCost, ref vitalityMultiplierCount, false, vitalityCosts);

                    }
                }
                if (button.name == "MinusButton2")
                {
                    if (textElement.name == "StrengthValueText")
                    {
                        GameManager.Instance.soulShardSystem.inreaseSoulShard(null, strengthCosts[strengthMultiplierCount - 1]);
                        decreaseValueText(textElement);
                        updateSoulShardCost("SHValueText2", ref strengthCost, ref strengthMultiplierCount, false, strengthCosts);
                    }
                }
                if (button.name == "MinusButton3")
                {
                    if (textElement.name == "IntelligenceValueText")
                    {
                        GameManager.Instance.soulShardSystem.inreaseSoulShard(null, intelligenceCosts[intelligenceMultiplierCount - 1]);
                        decreaseValueText(textElement);
                        updateSoulShardCost("SHValueText3", ref intelligenceCost, ref intelligenceMultiplierCount, false, intelligenceCosts);
                    }
                }

            }

        }
        else
        {
            Debug.Log("You cannot decrease below 0");
        }

    }

    /// <summary>
    /// Increments the value of the text element by 1
    /// </summary>
    /// <param name="textElement"></param>
    private void incrementValueText(TextMeshProUGUI textElement)
    {
        if (textElement == null)
        {
            Debug.Log("textElement is null");
        }
        switch (textElement.name)
        {
            case "VitalityValueText":
                playerController.playerStats.IncrementVitality();
                textElement.text = playerController.playerStats.vitality.GetBaseValue().ToString();
                UpdateVITstats();
                updateProgressBar("Health", true);
                break;
            case "StrengthValueText":
                playerController.playerStats.IncrementStrength();
                textElement.text = playerController.playerStats.strength.GetBaseValue().ToString();
                UpdateSTRstats();
                updateProgressBar("Strength", true);
                break;
            case "IntelligenceValueText":
                playerController.playerStats.IncrementIntelligence();
                textElement.text = playerController.playerStats.intelligence.GetBaseValue().ToString();
                UpdateINTstats();
                updateProgressBar("Intelligence", true);
                break;
            default:
                Debug.Log("No matching textElement found");
                break;
        }

    }

    /// <summary>
    /// Decreases the value of the text element 
    /// </summary>
    /// <param name="textElement"></param>
    private void decreaseValueText(TextMeshProUGUI textElement)
    {

        if (textElement == null)
        {
            Debug.Log("textElement is null");
        }
        switch (textElement.name)
        {
            case "VitalityValueText":
                playerController.playerStats.DecreaseVitality();
                textElement.text = playerController.playerStats.vitality.GetBaseValue().ToString();
                UpdateVITstats();
                updateProgressBar("Health", false);
                break;
            case "StrengthValueText":
                playerController.playerStats.DecreaseStrength();
                textElement.text = playerController.playerStats.strength.GetBaseValue().ToString();
                UpdateSTRstats();
                updateProgressBar("Strength", false);
                break;
            case "IntelligenceValueText":
                playerController.playerStats.DecreaseIntelligence();
                textElement.text = playerController.playerStats.intelligence.GetBaseValue().ToString();
                UpdateINTstats();
                updateProgressBar("Intelligence", false);
                break;
            default:
                Debug.Log("No matching textElement found");
                break;
        }

    }

    /// <summary>
    /// Updates all values in the Altar GUI that are related to the vitality attribute
    /// </summary>
    private void UpdateVITstats()
    {
        foreach (TextMeshProUGUI textElement in textValueElements)
        {
            if (textElement.name == "maxHealthValueText")
            {
                textElement.text = playerController.playerStats.maxHealth.GetBaseValue().ToString();
            }
            if (textElement.name == "healthPBValueText")
            {
                textElement.text = playerController.playerStats.maxHealth.GetBaseValue().ToString();
            }
        }
    }

    /// <summary>
    /// Updates all values in the Altar GUI that are related to the strength attribute
    /// </summary>
    private void UpdateSTRstats()
    {
        foreach (TextMeshProUGUI textElement in textValueElements)
        {
            if (textElement.name == "damageValueText")
            {
                textElement.text = playerController.playerStats.closeDamage.GetBaseValue().ToString();

            }
            if (textElement.name == "damagePBValueText")
            {
                textElement.text = (playerController.playerStats.closeDamage.GetBaseValue() * 1.1f).ToString();

            }

        }
    }


    /// <summary>
    /// Updates all values in the Altar GUI that are related to the intelligence attribute
    /// </summary>
    private void UpdateINTstats()
    {
        foreach (TextMeshProUGUI textElement in textValueElements)
        {
            if (textElement.name == "rangeDamageValueText")
            {
                textElement.text = playerController.playerStats.rangedDamage.GetBaseValue().ToString();

            }
            if (textElement.name == "cooldownValueText")
            {
                textElement.text = Math.Round(playerController.playerStats.rangedCooldown.GetBaseValue(), 2).ToString();

            }
            if (textElement.name == "rangedRangeValueText")
            {
                textElement.text = Math.Round(playerController.playerStats.rangedRange.GetBaseValue(), 2).ToString();

            }
            if (textElement.name == "r_damagePBValueText")
            {
                textElement.text = (playerController.playerStats.rangedDamage.GetBaseValue() * 1.1f).ToString();

            }
            if (textElement.name == "r_cooldownPBValueText")
            {

                textElement.text = Math.Round(playerController.playerStats.rangedCooldown.GetBaseValue() * 0.95f, 2).ToString();

            }
            if (textElement.name == "r_rangePBValueText")
            {
                textElement.text = Math.Round(playerController.playerStats.rangedRange.GetBaseValue() * 1.05f, 2).ToString();

            }

        }
    }



    /// <summary>
    /// Set the base values of the player stats to the text elements in the Altar GUI
    /// </summary>
    private void SetBaseValues()
    {
        foreach (TextMeshProUGUI textElement in textValueElements)
        {
            /// Health values                
            if (textElement.name == "VitalityValueText")
            {
                textElement.text = playerController.playerStats.vitality.GetBaseValue().ToString();
            }
            if (textElement.name == "maxHealthValueText")
            {
                textElement.text = playerController.playerStats.maxHealth.GetBaseValue().ToString();
            }
            if (textElement.name == "healthPBValueText")
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
            if (textElement.name == "damagePBValueText")
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
            if (textElement.name == "r_damagePBValueText")
            {
                textElement.text = playerController.playerStats.rangedDamage.GetBaseValue().ToString();
            }
            if (textElement.name == "r_cooldownPBValueText")
            {
                textElement.text = playerController.playerStats.rangedCooldown.GetBaseValue().ToString();
            }
            if (textElement.name == "r_rangePBValueText")
            {
                textElement.text = playerController.playerStats.rangedRange.GetBaseValue().ToString();
            }

            /// Soul Shard values
            if (textElement.name == "SHValueText1" || textElement.name == "SHValueText2" || textElement.name == "SHValueText3")
            {
                textElement.text = defaultSoulShardCost.ToString();
            }
            /// Modifier values
            if (textElement.name == "damageModifierText")
            {
                textElement.text = damageModifier.ToString();
            }
            if (textElement.name == "cooldownModifierText")
            {
                textElement.text = cooldownModifier.ToString();
            }
            if (textElement.name == "rangeModifierText")
            {
                textElement.text = rangeModifier.ToString();
            }
            if (textElement.name == "rangeDamageModifierText")
            {
                textElement.text = damageModifier.ToString();
            }

        }
    }

    /// <summary>
    /// Sets the base values of the progress bars to 0 or 1
    /// </summary>
    private void setBaseProgressBarValues()
    {
        foreach (Image image in progressBar)
        {
            switch (image.name)
            {
                case "healthPBValue":
                    image.fillAmount = 0f;
                    break;
                case "damagePBValue":
                    image.fillAmount = 0f;
                    break;
                case "rangeDamagePBValue":
                    image.fillAmount = 0f;
                    break;
                case "cooldownPBValue":
                    image.fillAmount = 1f;
                    break;
                case "rangePBValue":
                    image.fillAmount = 0f;
                    break;
            }
        }
    }

    /// <summary>
    /// Fills the image of the progress bar by 0.1f
    /// </summary>
    /// <param name="image"></param>
    /// <param name="isIncrement"></param>
    private void fillImage(Image image, bool isIncrement)
    {
        if (!isIncrement)
        {
            image.fillAmount -= 0.1f;
        }
        else
        {
            image.fillAmount += 0.1f;
        }
    }

    /// <summary>
    /// Updates the progress bar values based on the attribute
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="isIncrement"></param>
    private void updateProgressBar(string attribute, bool isIncrement)
    {
        foreach (Image image in progressBar)
        {
            switch (attribute)
            {
                case "Health":
                    if (image.name == "healthPBValue")
                    {
                        fillImage(image, isIncrement);
                    }
                    break;
                case "Strength":
                    if (image.name == "damagePBValue")
                    {
                        fillImage(image, isIncrement);

                    }
                    break;
                case "Intelligence":
                    if (image.name == "rangeDamagePBValue")
                    {
                        fillImage(image, isIncrement);
                    }
                    if (image.name == "cooldownPBValue")
                    {
                        if (!isIncrement)
                        {
                            image.fillAmount += 0.1f;
                        }
                        else
                        {
                            image.fillAmount -= 0.1f;
                        }
                    }
                    if (image.name == "rangePBValue")
                    {
                        fillImage(image, isIncrement);
                    }
                    break;

            }
        }
    }

    public void updateAltarGUI()
    {
        SetBaseValues();
        setBaseProgressBarValues();
    }

    /// <summary>
    /// Sets the costs for the attributes and the multiplier counts
    /// <param name="vitCost"></param>
    /// <param name="strCost"></param>
    /// <param name="intCost"></param>
    /// <param name="vitMult"></param>
    /// <param name="strMult"></param>
    /// <param name="intMult"></param>
    /// <param name="vitArray"></param>
    /// <param name="strArray"></param>
    /// <param name="intArray"></param>
    /// </summary>
    public void SetAltarCostsFromData(int vitCost, int strCost, int intCost, int vitMult, int strMult, int intMult, int[] vitArray, int[] strArray, int[] intArray)
    {
        this.vitalityCost = vitCost;
        this.strengthCost = strCost;
        this.intelligenceCost = intCost;

        this.vitalityMultiplierCount = vitMult;
        this.strengthMultiplierCount = strMult;
        this.intelligenceMultiplierCount = intMult;

        this.vitalityCosts = vitArray;
        this.strengthCosts = strArray;
        this.intelligenceCosts = intArray;

        // Nun noch die GUI aktualisieren
        updateAltarGUI();
        UpdateCostTexts();
        RefreshProgressBars();
    }

    /// <summary>
    /// Refreshes the progress bars in the Altar GUI
    /// </summary>
    public void RefreshProgressBars()
    {
        // Vitality
        float vit = playerController.playerStats.vitality.GetBaseValue();
        SetProgressBarAbsolute("Health", vit / 10f);

        // Strength
        float str = playerController.playerStats.strength.GetBaseValue();
        SetProgressBarAbsolute("Strength", str / 10f);

        // Intelligence
        float intel = playerController.playerStats.intelligence.GetBaseValue();
        SetProgressBarAbsolute("Intelligence", intel / 10f);
    }

    /// <summary>
    /// Updates the cost texts in the Altar GUI
    /// </summary>
    public void UpdateCostTexts()
    {
        foreach (var textElement in textValueElements)
        {
            if (textElement.name == "SHValueText1")
                textElement.text = vitalityCost.ToString();
            if (textElement.name == "SHValueText2")
                textElement.text = strengthCost.ToString();
            if (textElement.name == "SHValueText3")
                textElement.text = intelligenceCost.ToString();
        }
    }

    /// <summary>
    /// Sets the progress bar to a specific value
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="fraction"></param>
    private void SetProgressBarAbsolute(string attribute, float fraction)
    {
        foreach (Image image in progressBar)
        {
            switch (attribute)
            {
                case "Health":
                    if (image.name == "healthPBValue")
                    {
                        image.fillAmount = fraction;
                    }
                    break;

                case "Strength":
                    if (image.name == "damagePBValue")
                    {
                        image.fillAmount = fraction;
                    }
                    break;

                case "Intelligence":
                    if (image.name == "rangeDamagePBValue")
                    {
                        image.fillAmount = fraction;
                    }

                    if (image.name == "cooldownPBValue")
                    {
                        image.fillAmount = 1 - fraction;
                    }

                    if (image.name == "rangePBValue")
                    {
                        image.fillAmount = fraction;
                    }
                    break;
            }
        }
    }
}
