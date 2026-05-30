using UnityEngine;

[CreateAssetMenu(fileName = "ProgressionConfig", menuName = "Configs/ProgressionConfig")]

public class ProgressionConfig : ScriptableObject
{
    [Header("Max Level for Each Rarity")]
    public RarityLevel[] rarityLevels;

    [Header("Bonus Levels for Each Tier")]
    public TierLevel[] tierLevels;

    public int GetRarityMaxLevel(Rarities rarity)
    {
        foreach (var r in rarityLevels)
        {
            if (r.rarity == rarity)
                return r.maxLevel;
        }

        Debug.LogError($"No rarity config found for {rarity}");
        return 1;
    }

    public int GetTierLevelBonus(Tiers tier)
    {
        foreach (var t in tierLevels)
        {
            if (t.tier == tier)
                return t.bonus;
        }

        Debug.LogError($"No tier config found for {tier}");
        return 0;
    }
}

[System.Serializable]
public struct RarityLevel
{
    public Rarities rarity;
    public int maxLevel;
}

[System.Serializable]
public struct TierLevel
{
    public Tiers tier;
    public int bonus;
}