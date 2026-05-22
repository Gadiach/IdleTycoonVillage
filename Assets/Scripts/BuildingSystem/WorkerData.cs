using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkerData
{
    public BusinessType type;
    public bool available = true;
    public int level = 1;
    public float speedBonus;
    public float incomeBonus;
    public Sprite roundIcon;
    public int PriceToUpgrade = 3;
    public Sprite Icon;
    public Rarities CurrentRarity = Rarities.Primitive;
    public Tiers CurrentTier = Tiers.Tier1;
    public int CurrentLevel = 1;
    public CurrencyType Currency;
    [SerializeField]
    private ProgressionConfig progressionConfig;
    public BuildingData AssignedBuilding { get; set; }

    public int CurrentTierMaxLevel
    {
        get
        {
            int baseMax =
                progressionConfig.GetBaseMaxLevel(CurrentRarity);

            int tierBonus =
                progressionConfig.GetTierBonus(CurrentTier);

            return baseMax + tierBonus;
        }
    }

    public void UpgradeWorker()
    {
        if (level >= CurrentTierMaxLevel)
            return;

        level++;
    }

    public Tiers NextTier
    {
        get
        {
            int current = (int)CurrentTier;
            int max = Enum.GetValues(typeof(Tiers)).Length - 1;

            if (current < max)
                return (Tiers)(current + 1);

            return Tiers.Tier1;
        }
    }

    public Rarities NextRarity
    {
        get
        {
            int current = (int)CurrentRarity;
            int max = Enum.GetValues(typeof(Rarities)).Length - 1;

            if (current < max)
                return (Rarities)(current + 1);

            return CurrentRarity;
        }
    }

    public Dictionary<CurrencyType, int> GetBlueprintRequirementsForNextUpgrade()
    {
        Dictionary<CurrencyType, int> requirements = new();

        int currentRarityIndex = (int)CurrentRarity;
        int currentTierIndex = (int)CurrentTier;

        int maxTier = Enum.GetValues(typeof(Tiers)).Length;

        if (currentTierIndex < maxTier)
        {
            CurrencyType blueprint =
                CurrencyHelper.GetBlueprintCurrency(CurrentRarity);

            requirements[blueprint] = currentTierIndex + 1;
            return requirements;
        }

        for (int r = 0; r <= currentRarityIndex; r++)
        {
            Rarities rarity = (Rarities)r;
            CurrencyType blueprint =
                CurrencyHelper.GetBlueprintCurrency(rarity);

            if (r < currentRarityIndex)
                requirements[blueprint] = maxTier;
            else
                requirements[blueprint] = 1;
        }

        return requirements;
    }
    public bool CanUpgradeTierOrRarity()
    {
        var requirements = GetBlueprintRequirementsForNextUpgrade();

        foreach (var req in requirements)
        {
            int owned = CurrencySystem.GetCurrencyAmount(req.Key);
            if (owned < req.Value)
                return false;
        }

        return true;
    }
    public void UpgradeTierOrRarity()
    {
        if (!CanUpgradeTierOrRarity())
            return;

        var requirements = GetBlueprintRequirementsForNextUpgrade();

        foreach (var req in requirements)
        {
            CurrencySystem.Instance.TrySpendCurrency(req.Key, req.Value);
        }

        int maxTier = Enum.GetValues(typeof(Tiers)).Length;

        if ((int)CurrentTier < maxTier)
        {
            CurrentTier = NextTier;
        }
        else
        {
            CurrentTier = Tiers.Tier1;
            CurrentRarity = NextRarity;
        }

        EventManager.Instance.QueueEvent(new WorkerTierOrRarityChangedEvent(this));
    }
}
