using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SwordSetScriptable))]
public class SwordSetEditor : Editor
{
    private StatType newStatTypeTwoParts;
    private float newStatValueTwoParts;

    private StatType newStatTypeThreeParts;
    private float newStatValueThreeParts;

    private StatType newStatTypeFourParts;
    private float newStatValueFourParts;

    public override void OnInspectorGUI()
    {
        SwordSetScriptable swordSet = (SwordSetScriptable)target;

        EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
        swordSet.swordSet = (SetOfSword)EditorGUILayout.EnumPopup("Sword Set", swordSet.swordSet);

        EditorGUILayout.Space(10);

        // TWO PARTS

        List<StatModifier> toRemoveTwoParts = new();

        EditorGUILayout.LabelField("Stat Modifiers for 2 Parts", EditorStyles.boldLabel);

        foreach (var stat in swordSet.statModifiersTwoParts)
        {
            EditorGUILayout.BeginHorizontal();
            stat.stat = (StatType)EditorGUILayout.EnumPopup(stat.stat);
            stat.value = EditorGUILayout.FloatField(stat.value);

            if (GUILayout.Button("X", GUILayout.Width(20))) toRemoveTwoParts.Add(stat);

            EditorGUILayout.EndHorizontal();
        }

        foreach (var remove in toRemoveTwoParts) swordSet.statModifiersTwoParts.Remove(remove);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Add New Stat", EditorStyles.boldLabel);

        newStatTypeTwoParts = (StatType)EditorGUILayout.EnumPopup("Stat", newStatTypeTwoParts);
        newStatValueTwoParts = EditorGUILayout.FloatField("Value", newStatValueTwoParts);

        if (GUILayout.Button("Add Stat for 2 Parts"))
        {
            bool exists = swordSet.statModifiersTwoParts.Exists(x => x.stat == newStatTypeTwoParts);

            if (!exists)
            {
                swordSet.statModifiersTwoParts.Add(new StatModifier { stat = newStatTypeTwoParts, value = newStatValueTwoParts });
            }
            else
            {
                Debug.LogWarning("Stat already exists!");
            }
        }

        // THREE PARTS

        EditorGUILayout.Space(10);

        List<StatModifier> toRemoveThreeParts = new();

        EditorGUILayout.LabelField("Stat Modifiers For 3 Parts", EditorStyles.boldLabel);

        foreach (var stat in swordSet.statModifiersThreeParts)
        {
            EditorGUILayout.BeginHorizontal();
            stat.stat = (StatType)EditorGUILayout.EnumPopup(stat.stat);
            stat.value = EditorGUILayout.FloatField(stat.value);

            if (GUILayout.Button("X", GUILayout.Width(20))) toRemoveThreeParts.Add(stat);

            EditorGUILayout.EndHorizontal();
        }

        foreach (var remove in toRemoveThreeParts) swordSet.statModifiersThreeParts.Remove(remove);

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Add New Stat", EditorStyles.boldLabel);

        newStatTypeThreeParts = (StatType)EditorGUILayout.EnumPopup("Stat", newStatTypeThreeParts);
        newStatValueThreeParts = EditorGUILayout.FloatField("Value", newStatValueThreeParts);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Add Stat for 3 Parts"))
        {
            bool exists = swordSet.statModifiersThreeParts.Exists(x => x.stat == newStatTypeThreeParts);

            if (!exists)
            {
                swordSet.statModifiersThreeParts.Add(new StatModifier { stat = newStatTypeThreeParts, value = newStatValueThreeParts });
            }
            else
            {
                Debug.LogWarning("Stat already exists!");
            }
        }

        // FOUR PARTS

        EditorGUILayout.Space(10);

        List<StatModifier> toRemoveFourParts = new();

        EditorGUILayout.LabelField("Stat Modifiers For 4 parts", EditorStyles.boldLabel);

        foreach (var stat in swordSet.statModifiersFourParts)
        {
            EditorGUILayout.BeginHorizontal();
            stat.stat = (StatType)EditorGUILayout.EnumPopup(stat.stat);
            stat.value = EditorGUILayout.FloatField(stat.value);

            if (GUILayout.Button("X", GUILayout.Width(20))) toRemoveFourParts.Add(stat);

            EditorGUILayout.EndHorizontal();
        }

        foreach (var remove in toRemoveFourParts) swordSet.statModifiersFourParts.Remove(remove);

        EditorGUILayout.LabelField("Add New Stat", EditorStyles.boldLabel);

        newStatTypeFourParts = (StatType)EditorGUILayout.EnumPopup("Stat", newStatTypeFourParts);
        newStatValueFourParts = EditorGUILayout.FloatField("Value", newStatValueFourParts);

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Add Stat for 3 Parts"))
        {
            bool exists = swordSet.statModifiersFourParts.Exists(x => x.stat == newStatTypeFourParts);

            if (!exists)
            {
                swordSet.statModifiersFourParts.Add(new StatModifier { stat = newStatTypeFourParts, value = newStatValueFourParts });
            }
            else
            {
                Debug.LogWarning("Stat already exists!");
            }
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}
