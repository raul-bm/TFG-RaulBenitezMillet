using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwordPart", menuName = "DungeonScriptableObjects/SwordPart")]
public class SwordPartScripteable : ScriptableObject
{
    public Sprite partImageGameObject;
    public Sprite partImageIcon;
    public string partName;
    [TextArea(3, 10)]
    public string description;
    public TypeSwordPart partType;
    public SetOfSword setOfSword;

    public List<StatModifier> statModifiers = new List<StatModifier>();
}
