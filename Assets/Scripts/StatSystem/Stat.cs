public class Stat 
{
    /*
    Each Stat has three attributes:
     - baseValue: base Stat value, influenced by strength (closeRange) or intelligence (longRange)
     - modifier: modifier value, influenced by equipped Weapon (closeRange or longRange)
     - calcValue: calculated Value = baseValue * modifier

    Example:
     - Stat: damage
        - baseValue = 5   // Value is 5, as strength = 2
        - modifier = 10   // Value is 10 (in percent), as equipped CloseRangeWeapon has a damage modifier of +10%
        - calcValue = 5.5 // Value is 5.5 = 5 (baseValue) * 10% (modifier)
    */

    // baseValue: base Stat value, influenced by strength (closeRange) or intelligence (longRange)
    private float baseValue;    

    // modifier: modifier value, influenced by equipped Weapon (closeRange or longRange)
    private float modifier = 0f;

    // calculated Value = baseValue * modifier (in percent)
    private float calcValue = 0f;

    /// <summary>
    /// Initializes a new instance of the Stat class.
    /// </summary>
    /// <param name="baseValue">Base value of the stat.</param>
    /// <param name="modifier">Modifier value of the stat.</param>
    public Stat (float baseValue, float modifier = 0)
    {
        this.baseValue = baseValue;
        this.modifier = modifier;

        CalculateValue();
    }

    /// <summary>
    /// Returns the base value of the stat.
    /// </summary>
    /// <returns>Base value of the stat.</returns>
    public float GetBaseValue() 
    {
        return baseValue;
    }

    /// <summary>
    /// Sets baseValue equal to provided value (value)
    /// </summary>
    /// <param name="value">New base value.</param>
    public void SetBaseValue (float value)
    {
        baseValue = value;
        CalculateValue();
    }

    /// <summary>
    /// Returns the modifier value of the stat.
    /// </summary>
    /// <returns>Modifier value of the stat.</returns>
    public float GetModifier ()
    {
        return modifier;
    }

    /// <summary>
    /// Sets the modifier value of the stat to provided value (modifierInPercent).
    /// </summary>
    /// <param name="modifierInPercent">New modifier value in percent.</param>
    public void SetModifier (float modifierInPercent)
    {
        float modifier = modifierInPercent;
        CalculateValue();
    }

    /// <summary>
    /// Calculates and returns the calculated value of the stat.
    /// </summary>
    /// <returns>Calculated value of the stat.</returns>
    public float GetCalcValue()
    {
        CalculateValue();
        return calcValue;
    }

    /// <summary>
    /// Calculates calcValue and sets it
    /// </summary>
    public void CalculateValue() 
    {
        float onePercentOfBaseValue = baseValue / 100f;         // Calculate onePercentOfBaseValue
        float amount = onePercentOfBaseValue * modifier;        // Calculate Amount by multiplying onePercentOfBaseValue with modifier, which is in percent
        calcValue = baseValue + amount;                         // set CalcValue, by adding calaculated amount to baseValue
    
        /*
        Example Calculation:
        0.05 = 5 / 100
        0.5 = 0.05 * 10
        5.5 = 5 + 0.5
        */
    }
}