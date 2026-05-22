#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingPlaceable))]
public class BuildingPlaceableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var bp = (BuildingPlaceable)target;

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Runtime Worker (Read-only)", EditorStyles.boldLabel);

        var worker = bp.GetAssignedWorker();

        if (worker == null)
        {
            EditorGUILayout.HelpBox("No worker assigned.", MessageType.Info);
            return;
        }

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.LabelField("Type", worker.type.ToString());
            EditorGUILayout.IntField("Level", worker.level);
            EditorGUILayout.Toggle("Available", worker.available);
            EditorGUILayout.FloatField("Speed Bonus", worker.speedBonus);
            EditorGUILayout.FloatField("Income Bonus", worker.incomeBonus);
            EditorGUILayout.ObjectField("Icon", worker.Icon, typeof(Sprite), false);
        }
    }
}
#endif