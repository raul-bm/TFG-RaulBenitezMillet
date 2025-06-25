using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class SwordPartShop : MonoBehaviour
{
    public SwordPartScripteable partScripteable;

    public string textToTooltip = "";

    #region Stats
    public string partName { get; private set; }
    public string description { get; private set; }
    public TypeSwordPart partType { get; private set; }
    public SetOfSword setOfSword { get; private set; }
    public List<StatModifier> statModifiers { get; private set; }
    #endregion

    private void Start()
    {
        partName = partScripteable.partName;
        description = partScripteable.description;
        partType = partScripteable.partType;
        setOfSword = partScripteable.setOfSword;
        statModifiers = partScripteable.statModifiers;

        ChangeText();
    }

    private void ChangeText()
    {
        textToTooltip += "<b>" + partName + "</b>\n\n";
        textToTooltip += description + "\n\n";

        textToTooltip += "<b><color=" + GetColorHexForEachSet(setOfSword) + ">" + setOfSword.ToString() + " Sword</color> Set</b>\n\n";

        foreach (var modifier in statModifiers)
        {
            textToTooltip += "<b>";
            textToTooltip += "<color=" + GetColorHexForEachStat(modifier.stat) + ">" + GetFormattedStatName(modifier.stat) + ":</color> " + (modifier.value >= 0 ? "+" : "");
            textToTooltip += modifier.value;
            textToTooltip += "</b>\n";
        }
    }

    // Returns the equivalent hex color of the stat type for showing it on UI
    public string GetColorHexForEachStat(StatType type)
    {
        string hexColor = "";

        switch (type)
        {
            case (StatType.Damage):
                hexColor = "#e67e22";
                break;

            case (StatType.Defense):
                hexColor = "#2980b9";
                break;

            case (StatType.AttackRange):
                hexColor = "#9b59b6";
                break;

            case (StatType.AttackSpeed):
                hexColor = "#f1c40f";
                break;

            default:
                hexColor = "#FFFFFF";
                break;
        }

        return hexColor;
    }

    // Returns the equivalent hex color of the set of sword for showing it on UI
    public string GetColorHexForEachSet(SetOfSword set)
    {
        string hexColor = "";

        switch (set)
        {
            case (SetOfSword.Ice):
                hexColor = "#bddeec";
                break;

            case (SetOfSword.Blood):
                hexColor = "#8a0303";
                break;

            case (SetOfSword.Light):
                hexColor = "#b2b2b2";
                break;

            case (SetOfSword.Dark):
                hexColor = "#842ec4";
                break;
        }

        return hexColor;
    }

    // Returns the string with the words with spaces
    public string GetFormattedStatName(StatType type)
    {
        return Regex.Replace(type.ToString(), "(\\B[A-Z])", " $1");
    }
}
