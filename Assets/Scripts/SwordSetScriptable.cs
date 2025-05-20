using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordSet", menuName = "DungeonScriptableObjects/SwordSet")]
public class SwordSetScriptable : ScriptableObject
{
    public SetOfSword swordSet;

    public List<StatModifier> statModifiersTwoParts = new List<StatModifier>();
    public List<StatModifier> statModifiersThreeParts = new List<StatModifier>();
    public List<StatModifier> statModifiersFourParts = new List<StatModifier>();
}
