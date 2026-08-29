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
        EditorGUILayout.LabelField(
            "Runtime Worker (Read-only)",
            EditorStyles.boldLabel
        );

        var worker = bp.GetAssignedWorker();

        if (worker == null)
        {
            EditorGUILayout.HelpBox(
                "No worker assigned.",
                MessageType.Info
            );

            return;
        }

        if (worker.Definition == null)
        {
            EditorGUILayout.HelpBox(
                "Worker exists, but WorkerDefinition is not initialized.",
                MessageType.Warning
            );

            return;
        }

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.LabelField(
                "Type",
                worker.Type.ToString()
            );

            EditorGUILayout.IntField(
                "Level",
                worker.CurrentLevel
            );

            EditorGUILayout.Toggle(
                "Available",
                worker.IsAvailable
            );

            EditorGUILayout.ObjectField(
                "Icon",
                worker.Icon,
                typeof(Sprite),
                false
            );
        }
    }
}
#endif