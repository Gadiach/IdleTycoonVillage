using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkerData
{
    #region Definition Data
    
    public BusinessType Type;

    public Sprite RoundIcon;
    public Sprite Icon;

    public CurrencyType Currency;
    [SerializeField] private float baseProductionDuration = 10f;
    [SerializeField] private float baseUpgradePrice = 3f;

    #endregion

    #region Configs

    [SerializeField] private ProgressionConfig progressionConfig;
    [SerializeField] private UpgradeCostConfig upgradeCostConfig;
    [SerializeField] private EconomyProgressionConfig economyConfig;

    #endregion

    #region Runtime State

    public int CurrentLevel { get; private set; } = 1;

    public Rarities CurrentRarity { get; private set; } = Rarities.Primitive;

    public Tiers CurrentTier { get; private set; } = Tiers.Tier1;

    public bool IsAvailable { get; private set; } = true;

    public BuildingData AssignedBuilding { get; set; }

    #endregion

    #region Calculated Properties

    public int PriceToUpgrade
    {
        get
        {
            return Mathf.RoundToInt(baseUpgradePrice * Mathf.Pow(upgradeCostConfig.workerUpgradeMultiplier, CurrentLevel - 1));
        }
    }

    public float ProductionDuration
    {
        get
        {
            float rarityMultiplier = economyConfig.GetRarityProductionTimeMultiplier(CurrentRarity);

            float tierMultiplier = economyConfig.GetTierProductionTimeMultiplier(CurrentTier);

            return baseProductionDuration * rarityMultiplier * tierMultiplier;
        }
    }

    public int CurrentTierMaxLevel
    {
        get
        {
            int baseMax = progressionConfig.GetRarityMaxLevel(CurrentRarity);

            int tierBonus = progressionConfig.GetTierLevelBonus(CurrentTier);

            return baseMax + tierBonus;
        }
    }

    #endregion

    public WorkerData(ProgressionConfig progressionConfig, UpgradeCostConfig upgradeCostConfig, EconomyProgressionConfig economyConfig)
    {
        this.progressionConfig = progressionConfig;
        this.upgradeCostConfig = upgradeCostConfig;
        this.economyConfig = economyConfig;
    }

    public void UpgradeWorkerLvl()
    {
        if (CurrentLevel >= CurrentTierMaxLevel)
            return;

        CurrentLevel++;
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

    public void AssignToBuilding(BuildingData building)
    {
        AssignedBuilding = building;

        IsAvailable = false;
    }

    public void UnassignFromBuilding()
    {
        AssignedBuilding = null;

        IsAvailable = true;
    }
}
