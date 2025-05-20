using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType {
    HealthPoints,
    Damage,
    Defense,
    AttackRange,
    AttackSpeed,
    MoveSpeed,
    CriticalChance,
    CriticalDamage,
    LifeStealChance,
    LifeSteal
}

[Serializable]
public class StatModifier
{
    public StatType stat;
    public float value;
}
