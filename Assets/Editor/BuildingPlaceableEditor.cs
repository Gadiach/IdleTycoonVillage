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

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.LabelField(
                "Type",
                worker.BusinessType.ToString()
            );

            EditorGUILayout.IntField(
                "Level",
                worker.CurrentLevel
            );

            EditorGUILayout.LabelField(
                "Rarity",
                worker.CurrentRarity.ToString()
            );

            EditorGUILayout.LabelField(
                "Tier",
                worker.CurrentTier.ToString()
            );

            EditorGUILayout.Toggle(
                "Available",
                worker.IsAvailable
            );

            Sprite icon = WorkerSystem.Instance != null
                ? WorkerSystem.Instance.GetRoundIcon(
                    worker.BusinessType,
                    worker.CurrentRarity
                )
                : null;

            EditorGUILayout.ObjectField(
                "Round Icon",
                icon,
                typeof(Sprite),
                false
            );
        }
    }
}
#endif