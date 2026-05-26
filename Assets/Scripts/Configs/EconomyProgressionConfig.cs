using UnityEngine;

[CreateAssetMenu(
    fileName = "EconomyProgressionConfig",
    menuName = "Configs/Economy Progression Config")]
public class EconomyProgressionConfig : ScriptableObject
{
    [Header("Production Income")]
    public RarityMultiplier[] rarityIncomeMultipliers;
    public TierMultiplier[] tierIncomeMultipliers;

    [Header("Production Speed")]
    public RarityMultiplier[] rarityProductionTimeMultipliers;
    public TierMultiplier[] tierProductionTimeMultipliers;

    public float GetRarityIncomeMultiplier(Rarities rarity)
    {
        foreach (var r in rarityIncomeMultipliers)
        {
            if (r.rarity == rarity)
                return r.multiplier;
        }

        return 1f;
    }

    public float GetTierIncomeMultiplier(Tiers tier)
    {
        foreach (var t in tierIncomeMultipliers)
        {
            if (t.tier == tier)
                return t.multiplier;
        }

        return 1f;
    }

    public float GetRarityProductionTimeMultiplier(Rarities rarity)
    {
        foreach (var r in rarityProductionTimeMultipliers)
        {
            if (r.rarity == rarity)
                return r.multiplier;
        }

        return 1f;
    }

    public float GetTierProductionTimeMultiplier(Tiers tier)
    {
        foreach (var t in tierProductionTimeMultipliers)
        {
            if (t.tier == tier)
                return t.multiplier;
        }

        return 1f;
    }

    [System.Serializable]
    public struct RarityMultiplier
    {
        public Rarities rarity;
        public float multiplier;
    }

    [System.Serializable]
    public struct TierMultiplier
    {
        public Tiers tier;
        public float multiplier;
    }
}