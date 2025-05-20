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

    private void Start()
    {
        InitBaseStats();
    }

    public void InitBaseStats(Dictionary<StatType, float> anotherBaseStats = null)
    {
        foreach(StatType stat in Enum.GetValues(typeof(StatType)))
        {
            if(anotherBaseStats != null && anotherBaseStats.ContainsKey(stat))
            {
                baseStats[stat] = anotherBaseStats[stat];
                currentStats[stat] = anotherBaseStats[stat];
            }
            else
            {
                if(stat == StatType.CriticalChance)
                {
                    baseStats[stat] = 10;
                    currentStats[stat] = 10;
                } else if(stat == StatType.LifeStealChance)
                {
                    baseStats[stat] = 5;
                    currentStats[stat] = 5;
                } else
                {
                    baseStats[stat] = 1f;
                    currentStats[stat] = 1f;
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
                currentStats[modifier.stat] += modifier.value;
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
                        currentStats[modifier.stat] += modifier.value;
                    }
                }
                else if (countPartsOfSet[swordSet.swordSet] == 3)
                {
                    foreach(var modifier in swordSet.statModifiersThreeParts)
                    {
                        currentStats[modifier.stat] += modifier.value;
                    }
                }
                else if (countPartsOfSet[swordSet.swordSet] == 4)
                {
                    foreach (var modifier in swordSet.statModifiersFourParts)
                    {
                        currentStats[modifier.stat] += modifier.value;
                    }
                }
            }
        }
    }

    public float GetStat(StatType type)
    {
        return currentStats.TryGetValue(type, out float value) ? value : baseStats[type];
    }

    // FALTA MOSTRAR LOS BONOS DEL SET EN LA UI

    private void ChangeCurrentStatsText()
    {
        string text = "";

        foreach(var stat in currentStats)
        {
            text += GetFormattedStatText(stat.Key);
        }

        UI.Instance.ChangeCurrentStatsText(text);
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

        if (type == StatType.LifeStealChance || type == StatType.CriticalChance) text = "<color=" + color + ">" + UI.Instance.GetFormattedStatName(type) + ":</color> " + currentValue + "%" + differenceText + "\n";
        else text = "<color=" + color + ">" + UI.Instance.GetFormattedStatName(type) + ":</color> " + currentValue + differenceText + "\n";

        return text;
    }
}
