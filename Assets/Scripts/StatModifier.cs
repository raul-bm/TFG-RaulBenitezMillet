using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType {
    Damage,
    Defense,
    AttackRange,
    AttackSpeed,
}

[Serializable]
public class StatModifier
{
    public StatType stat;
    public float value;
}
