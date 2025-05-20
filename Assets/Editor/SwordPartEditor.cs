using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwordPartScripteable))]
public class SwordPartEditor : Editor
{
    private StatType newStatType;
    private float newStatValue;

    public override void OnInspectorGUI()
    {
        SwordPartScripteable part = (SwordPartScripteable)target;

        EditorGUILayout.LabelField("Basic Info", EditorStyles.boldLabel);
        part.partName = EditorGUILayout.TextField("Part Name", part.partName);
        part.partImageIcon = (Sprite)EditorGUILayout.ObjectField("Image Icon", part.partImageIcon, typeof(Sprite), false);
        part.partImageGameObject = (Sprite)EditorGUILayout.ObjectField("Image GameObject", part.partImageGameObject, typeof(Sprite), false);

        EditorGUILayout.LabelField("Description");
        part.description = EditorGUILayout.TextArea(part.description, GUILayout.Height(16f * 10));
        part.partType = (TypeSwordPart)EditorGUILayout.EnumPopup("Part Type", part.partType);
        part.setOfSword = (SetOfSword)EditorGUILayout.EnumPopup("Set of Sword", part.setOfSword);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Stat Modifiers", EditorStyles.boldLabel);

        List<StatModifier> toRemove = new();

        foreach(var stat in part.statModifiers)
        {
            EditorGUILayout.BeginHorizontal();
            stat.stat = (StatType)EditorGUILayout.EnumPopup(stat.stat);
            stat.value = EditorGUILayout.FloatField(stat.value);

            if (GUILayout.Button("X", GUILayout.Width(20))) toRemove.Add(stat);

            EditorGUILayout.EndHorizontal();
        }

        foreach (var remove in toRemove) part.statModifiers.Remove(remove);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Add New Stat", EditorStyles.boldLabel);

        newStatType = (StatType)EditorGUILayout.EnumPopup("Stat", newStatType);
        newStatValue = EditorGUILayout.FloatField("Value", newStatValue);

        if(GUILayout.Button("Add Stat"))
        {
            bool exists = part.statModifiers.Exists(x => x.stat == newStatType);

            if(!exists)
            {
                part.statModifiers.Add(new StatModifier { stat = newStatType, value = newStatValue });
            }
            else
            {
                Debug.LogWarning("Stat already exists!");
            }
        }

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}
