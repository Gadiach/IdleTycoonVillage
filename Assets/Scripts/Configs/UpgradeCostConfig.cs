using UnityEngine;
using static EconomyProgressionConfig;

[CreateAssetMenu(fileName = "UpgradeCostConfig", menuName = "Configs/Upgrade Cost Config")]

public class UpgradeCostConfig : ScriptableObject
{
    [Header("Building Upgrade")]

    public float buildingUpgradeMultiplier = 1.25f;

    [Header("Worker Upgrade")]

    public float workerUpgradeMultiplier = 1.2f;

    [Header("Building Upgrade Cost")]

    public RarityMultiplier[] rarityBuildingUpgradeCostMultipliers;

    public TierMultiplier[] tierBuildingUpgradeCostMultipliers;


    [Header("Worker Upgrade Cost")]

    public RarityMultiplier[] rarityWorkerUpgradeCostMultipliers;

    public TierMultiplier[] tierWorkerUpgradeCostMultipliers;


    #region Building

    public float GetRarityBuildingUpgradeMultiplier(Rarities rarity)
    {
        foreach (var r in rarityBuildingUpgradeCostMultipliers)
        {
            if (r.rarity == rarity)
                return r.multiplier;
        }

        return 1f;
    }

    public float GetTierBuildingUpgradeMultiplier(Tiers tier)
    {
        foreach (var t in tierBuildingUpgradeCostMultipliers)
        {
            if (t.tier == tier)
                return t.multiplier;
        }

        return 1f;
    }

    #endregion


    #region Worker

    public float GetRarityWorkerUpgradeMultiplier(Rarities rarity)
    {
        foreach (var r in rarityWorkerUpgradeCostMultipliers)
        {
            if (r.rarity == rarity)
                return r.multiplier;
        }

        return 1f;
    }

    public float GetTierWorkerUpgradeMultiplier(Tiers tier)
    {
        foreach (var t in tierWorkerUpgradeCostMultipliers)
        {
            if (t.tier == tier)
                return t.multiplier;
        }

        return 1f;
    }

    #endregion
}