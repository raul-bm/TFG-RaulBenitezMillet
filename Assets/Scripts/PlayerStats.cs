using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private Dictionary<StatType, float> baseStats = new Dictionary<StatType, float>();
    private Dictionary<StatType, float> currentStats = new Dictionary<StatType, float>();

    private Dictionary<SetOfSword, int> countPartsOfSet = new Dictionary<SetOfSword, int>();

    [SerializeField] private List<SwordSetScriptable> swordSetScriptables;

    public void InitBaseStats(Dictionary<StatType, float> anotherBaseStats = null)
    {
        foreach(StatType stat in Enum.GetValues(typeof(StatType)))
        {
            if (anotherBaseStats != null && anotherBaseStats.ContainsKey(stat))
            {
                baseStats[stat] = anotherBaseStats[stat];
                currentStats[stat] = anotherBaseStats[stat];
            }
            else
            {
                switch (stat)
                {
                    case (StatType.Damage):
                        currentStats[stat] = baseStats[stat] = 10;
                        break;

                    case (StatType.AttackSpeed):
                        currentStats[stat] = baseStats[stat] = 1;
                        break;

                    case (StatType.AttackRange):
                        currentStats[stat] = baseStats[stat] = 1;
                        break;

                    case (StatType.Defense):
                        currentStats[stat] = baseStats[stat] = 0;
                        break;
                }
            }
        }

        ChangeCurrentStatsText();
    }
    
    // Falta set bonus

    public void ApplySwordParts(List<SwordPartInventory> equippedParts)
    {
        // Reset stats
        foreach (StatType stat in baseStats.Keys) currentStats[stat] = baseStats[stat];

        // Apply modifiers of each part
        foreach(var part in equippedParts)
        {
            foreach(var modifier in part.statModifiers)
            {
                SaveStatValue(modifier);
            }
        }

        // Apply modifiers of bonus set (if there is any)
        ApplyBonusSet(equippedParts);

        ChangeCurrentStatsText();
    }

    public void ApplyBonusSet(List<SwordPartInventory> equippedParts)
    {
        countPartsOfSet = new Dictionary<SetOfSword, int>();

        foreach (var part in equippedParts)
        {
            if (countPartsOfSet.ContainsKey(part.setOfSword)) countPartsOfSet[part.setOfSword]++;
            else countPartsOfSet.Add(part.setOfSword, 1);
        }

        foreach(var swordSet in swordSetScriptables)
        {
            if(countPartsOfSet.ContainsKey(swordSet.swordSet) && countPartsOfSet[swordSet.swordSet] > 1)
            {
                if (countPartsOfSet[swordSet.swordSet] == 2)
                {
                    foreach(var modifier in swordSet.statModifiersTwoParts)
                    {
                        SaveStatValue(modifier);
                    }
                }
                else if (countPartsOfSet[swordSet.swordSet] == 3)
                {
                    foreach(var modifier in swordSet.statModifiersThreeParts)
                    {
                        SaveStatValue(modifier);
                    }
                }
                else if (countPartsOfSet[swordSet.swordSet] == 4)
                {
                    foreach (var modifier in swordSet.statModifiersFourParts)
                    {
                        SaveStatValue(modifier);
                    }
                }
            }
        }
    }

    private void SaveStatValue(StatModifier modifier)
    {
        currentStats[modifier.stat] += modifier.value;

        if (modifier.stat == StatType.AttackRange || modifier.stat == StatType.AttackSpeed)
            currentStats[modifier.stat] = Mathf.Min(3.0f, Mathf.Max(1.0f, currentStats[modifier.stat]));
    }

    public float GetStat(StatType type)
    {
        return currentStats.TryGetValue(type, out float value) ? value : baseStats[type];
    }

    // FALTA MOSTRAR LOS BONOS DEL SET EN LA UI

    private void ChangeCurrentStatsText()
    {
        string textStats = "";

        foreach(var stat in currentStats)
        {
            textStats += GetFormattedStatText(stat.Key);
        }

        string textSets = "";

        foreach(var countSet in countPartsOfSet)
        {
            if(countSet.Value > 1) textSets += GetFormattedSetText(countSet.Key, countSet.Value);
        }

        UI.Instance.ChangeCurrentStatsText(textStats);
        UI.Instance.ChangeBonusSetText(textSets);
    }

    private string GetFormattedStatText(StatType type)
    {
        float baseValue = baseStats[type];
        float currentValue = currentStats[type];

        float difference = currentValue - baseValue;

        string differenceText = "";
        
        if (difference > 0) differenceText = "<color=green>(+" + difference + ")</color>";
        else if (difference < 0) differenceText = "<color=red>(" + difference + ")</color>";

        string color = UI.Instance.GetColorHexForEachStat(type);

        string text = "";

        text = "<color=" + color + ">" + UI.Instance.GetFormattedStatName(type) + ":</color> " + currentValue + differenceText + "\n";

        return text;
    }

    private string GetFormattedSetText(SetOfSword set, int count)
    {
        SwordSetScriptable setScriptable;

        string text = "<b>";
        text += "<color=" + UI.Instance.GetColorHexForEachSet(set) + ">" + set.ToString() + "</color> Sword Set (" + count + "/4)\n";

        int i = 0;

        do
        {
            setScriptable = swordSetScriptables[i];
            i++;
        } while (setScriptable.swordSet != set && i < swordSetScriptables.Count);

        List<StatModifier> statModifiers = null;

        if (count == 2) statModifiers = setScriptable.statModifiersTwoParts;
        else if (count == 3) statModifiers = setScriptable.statModifiersThreeParts;
        else if (count == 4) statModifiers = setScriptable.statModifiersFourParts;

        foreach (StatModifier statModifier in statModifiers)
        {
            string valueText = "";

            if (statModifier.value >= 0) valueText = "<color=green>+" + statModifier.value + "</color>";
            else valueText = "<color=red>" + statModifier.value + "</color>";

            string color = UI.Instance.GetColorHexForEachStat(statModifier.stat);

            text += "<color=" + color + ">" + UI.Instance.GetFormattedStatName(statModifier.stat) + ":</color> " + valueText + "\n";
        }

        text += "\n";

        return text;
    }
}
