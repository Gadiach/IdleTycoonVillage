using UnityEngine;

[CreateAssetMenu(fileName = "ProgressionConfig", menuName = "Configs/ProgressionConfig")]

public class ProgressionConfig : ScriptableObject
{
    [Header("Building level progression")]

    public RarityLevel[] BuildingRarityLevels;

    public TierLevel[] BuildingTierLevels;

    [Header("Worker level progression")]

    public RarityLevel[] WorkerRarityLevels;

    public TierLevel[] WorkerTierLevels;

    public int GetBuildingRarityMaxLevel(Rarities rarity)
    {
        foreach (var r in BuildingRarityLevels)
        {
            if (r.rarity == rarity)
                return r.maxLevel;
        }

        Debug.LogError($"No rarity config found for {rarity}");
        return 1;
    }

    public int GetWorkerRarityMaxLevel(Rarities rarity)
    {
        foreach (var r in WorkerRarityLevels)
        {
            if (r.rarity == rarity)
                return r.maxLevel;
        }

        Debug.LogError($"No rarity config found for {rarity}");
        return 1;
    }

    public int GetBuildingTierLevelBonus(Tiers tier)
    {
        foreach (var t in BuildingTierLevels)
        {
            if (t.tier == tier)
                return t.bonus;
        }

        Debug.LogError($"No tier config found for {tier}");
        return 0;
    }

    public int GetWorkerTierLevelBonus(Tiers tier)
    {
        foreach (var t in WorkerTierLevels)
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