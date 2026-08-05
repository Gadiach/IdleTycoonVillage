using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MissionData))]
public class MissionDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawIdentification();
        DrawMission();
        DrawTarget();
        DrawReward();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawIdentification()
    {
        EditorGUILayout.LabelField("Identification", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("missionName"));

        EditorGUILayout.Space();
    }

    private void DrawMission()
    {
        EditorGUILayout.LabelField("Mission", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("missionType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("targetValue"));

        EditorGUILayout.Space();
    }

    private void DrawTarget()
    {
        EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);

        SerializedProperty needBusiness =
            serializedObject.FindProperty("needBusinessType");

        EditorGUILayout.PropertyField(needBusiness);

        using (new EditorGUI.DisabledScope(!needBusiness.boolValue))
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("targetBusinessType"));
        }

        SerializedProperty needRarity =
            serializedObject.FindProperty("needRarity");

        EditorGUILayout.PropertyField(needRarity);

        using (new EditorGUI.DisabledScope(!needRarity.boolValue))
        {
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("targetRarity"));
        }

        EditorGUILayout.Space();
    }

    private void DrawReward()
    {
        EditorGUILayout.LabelField("Reward", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("rewardCurrency"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("rewardAmount"));
    }
}