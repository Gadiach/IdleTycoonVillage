using UnityEditor;

[CustomEditor(typeof(CurrencyShopItem))]
public class CurrencyShopItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty category = serializedObject.FindProperty("Category");

        SerializedProperty rewardType = serializedObject.FindProperty("RewardType");

        SerializedProperty price = serializedObject.FindProperty("Price");

        SerializedProperty reward = serializedObject.FindProperty("Reward");

        SerializedProperty icon = serializedObject.FindProperty("Icon");

        EditorGUILayout.PropertyField(category);
        EditorGUILayout.PropertyField(rewardType);

        CurrencyShopRewardType type = (CurrencyShopRewardType)rewardType.enumValueIndex;

        if (type != CurrencyShopRewardType.RandomBlueprint)
        {
            EditorGUILayout.PropertyField(price);
            EditorGUILayout.PropertyField(reward);
        }

        EditorGUILayout.PropertyField(icon);

        serializedObject.ApplyModifiedProperties();
    }
}