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
        SerializedProperty freeRewardLimit = serializedObject.FindProperty("FreeRewardLimit");
        SerializedProperty dailyAdLimit = serializedObject.FindProperty("DailyAdLimit");

        EditorGUILayout.PropertyField(category);
        EditorGUILayout.PropertyField(rewardType);

        CurrencyShopCategory shopCategory =
            (CurrencyShopCategory)category.enumValueIndex;

        CurrencyShopRewardType type =
            (CurrencyShopRewardType)rewardType.enumValueIndex;

        bool isGold = shopCategory == CurrencyShopCategory.Gold;
        bool isAd = shopCategory == CurrencyShopCategory.Ads;
        bool isRandomBlueprint = type == CurrencyShopRewardType.RandomBlueprint;

        // Price
        bool priceEnabled = isGold && !isRandomBlueprint;

        if (!priceEnabled)
            price.floatValue = 0;

        using (new EditorGUI.DisabledScope(!priceEnabled))
        {
            EditorGUILayout.PropertyField(price);
        }

        // Reward
        bool rewardEnabled = !isRandomBlueprint;

        if (!rewardEnabled)
            reward.intValue = 0;

        using (new EditorGUI.DisabledScope(!rewardEnabled))
        {
            EditorGUILayout.PropertyField(reward);
        }

        EditorGUILayout.PropertyField(icon);

        // Free Reward Limit
        if (!isGold)
            freeRewardLimit.intValue = 0;

        using (new EditorGUI.DisabledScope(!isGold))
        {
            EditorGUILayout.PropertyField(freeRewardLimit);
        }

        // Daily Ad Limit
        if (!isAd)
            dailyAdLimit.intValue = 0;

        using (new EditorGUI.DisabledScope(!isAd))
        {
            EditorGUILayout.PropertyField(dailyAdLimit);
        }

        serializedObject.ApplyModifiedProperties();
    }
}