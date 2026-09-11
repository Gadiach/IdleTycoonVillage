using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkerData
{
    #region Definition Properties
    public BusinessType Type => Definition.Type;

    public Sprite Icon => Definition.Icon;

    public Sprite RoundIcon => Definition.RoundIcon;

    public CurrencyType LevelUpgradeCurrency => Definition.Currency;

    private float BaseProductionDuration => Definition.BaseProductionDuration;

    private float BaseUpgradePrice => Definition.BaseUpgradePrice;

    private float ProductionTimeReductionPerLevel => Definition.ProductionTimeReductionPerLevel;

    public bool IsMaxProgression => CurrentRarity == Rarities.Futuristic && CurrentTier == Tiers.Tier5;
    public bool IsMaxLevel => CurrentLevel >= CurrentProgressionMaxLevel;

    #endregion

    #region Configs

    [SerializeField] private ProgressionConfig progressionConfig;
    [SerializeField] private UpgradeCostConfig upgradeCostConfig;
    [SerializeField] private EconomyProgressionConfig economyConfig;

    #endregion

    #region Runtime State
    public WorkerDefinition Definition { get; private set; }
    public int CurrentLevel { get; private set; } = 1;

    public Rarities CurrentRarity { get; private set; } = Rarities.Primitive;

    public Tiers CurrentTier { get; private set; } = Tiers.Tier1;

    public bool IsAvailable { get; private set; } = true;

    public BuildingData AssignedBuilding { get; set; }

    #endregion

    #region Calculated Properties

    public float LastCycleDurationDecrease { get; private set; }

    public float CurrentProgressionMinCycleDuration => Mathf.Round(CalculateCycleDuration(CurrentProgressionMaxLevel,CurrentRarity,CurrentTier) * 100f) / 100f;

    public float NextProgressionMinCycleDuration => Mathf.Round(CalculateCycleDuration(NextProgressionMaxLevel,NextProgressionRarity,NextProgressionTier) * 100f) / 100f;

    public int PriceToUpgradeTierOrRarity => CurrentTier == Tiers.Tier5 ? 1 : (int)NextTier;
    public int PriceToUpgradeLevel => Mathf.RoundToInt(BaseUpgradePrice * Mathf.Pow(upgradeCostConfig.workerUpgradeMultiplier, CurrentLevel - 1));

    public int CurrentProgressionMaxLevel
    {
        get
        {
            int baseMax = progressionConfig.GetWorkerRarityMaxLevel(CurrentRarity);
            int tierBonus = progressionConfig.GetWorkerTierLevelBonus(CurrentTier);
            return baseMax + tierBonus;
        }
    }

    public int NextProgressionMaxLevel
    {
        get
        {
            int nextRarityMaxLevel = progressionConfig.GetWorkerRarityMaxLevel(NextProgressionRarity);

            int nextTierLevelBonus = progressionConfig.GetWorkerTierLevelBonus(NextProgressionTier);

            return nextRarityMaxLevel + nextTierLevelBonus;
        }
    }

    public Rarities NextProgressionRarity
    {
        get
        {
            return CurrentTier == Tiers.Tier5 ? NextRarity : CurrentRarity;
        }
    }

    public Tiers NextProgressionTier
    {
        get
        {
            return CurrentTier == Tiers.Tier5 ? Tiers.Tier1 : NextTier;
        }
    }

    public Tiers NextTier
    {
        get
        {
            if (CurrentTier < Tiers.Tier5)
                return (Tiers)((int)CurrentTier + 1);

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

    public bool HasEnoughCurrencyForTierOrRarityUpgrade => CurrencySystem.Instance.HasEnoughCurrency(TierOrRarityUpgradeCurrency,PriceToUpgradeTierOrRarity);

    public bool HasEnoughCurrencyForLevelUpgrade => CurrencySystem.Instance.HasEnoughCurrency(LevelUpgradeCurrency,PriceToUpgradeLevel);

    public bool CanUpgradeTierOrRarity => !IsMaxProgression && HasEnoughCurrencyForTierOrRarityUpgrade;

    public bool CanUpgradeLevel => !IsMaxLevel && HasEnoughCurrencyForLevelUpgrade;

    public float CycleDuration => Mathf.Round(CalculateCycleDuration(CurrentLevel, CurrentRarity, CurrentTier) * 100f) / 100f;

    #endregion

    public WorkerData(WorkerDefinition definition, ProgressionConfig progressionConfig, UpgradeCostConfig upgradeCostConfig, EconomyProgressionConfig economyConfig)
    {
        Definition = definition;

        this.progressionConfig = progressionConfig;
        this.upgradeCostConfig = upgradeCostConfig;
        this.economyConfig = economyConfig;
    }

    public void UpgradeWorkerLvl()
    {
        if (!CanUpgradeLevel)
            return;

        float previousDuration = CycleDuration;

        CurrencySystem.Instance.SpendCurrency(LevelUpgradeCurrency,PriceToUpgradeLevel);

        CurrentLevel++;

        LastCycleDurationDecrease = Mathf.Round((previousDuration - CycleDuration) * 100f) / 100f;

        EventManager.Instance.QueueEvent(new WorkerUpgradedEvent(this));
    }

    private float CalculateCycleDuration(int level, Rarities rarity, Tiers tier)
    {
        float rarityMultiplier = economyConfig.GetRarityProductionTimeMultiplier(rarity);

        float tierMultiplier = economyConfig.GetTierProductionTimeMultiplier(tier);

        float levelMultiplier = 1f - ((level - 1) * ProductionTimeReductionPerLevel);

        return BaseProductionDuration * rarityMultiplier * tierMultiplier * levelMultiplier;
    }

    public void UpgradeTierOrRarity()
    {
        if (!CanUpgradeTierOrRarity)
            return;

        CurrencySystem.Instance.SpendCurrency(TierOrRarityUpgradeCurrency,PriceToUpgradeTierOrRarity);

        ApplyTierOrRarityUpgrade();

        EventManager.Instance.QueueEvent(new WorkerTierOrRarityChangedEvent(this));
    }

    private void ApplyTierOrRarityUpgrade()
    {
        if (CurrentTier < Tiers.Tier5)
        {
            CurrentTier = NextTier;
            return;
        }

        CurrentTier = Tiers.Tier1;
        CurrentRarity = NextRarity;
    }

    public CurrencyType TierOrRarityUpgradeCurrency
    {
        get
        {
            if (CurrentTier == Tiers.Tier5)
            {
                return CurrencyHelper.GetWorkerBlueprintCurrency(NextRarity);
            }

            return CurrencyHelper.GetWorkerBlueprintCurrency(CurrentRarity);
        }
    }

   

    public StarDisplayInfo GetStarDisplayInfo()
    {
        return new StarDisplayInfo
        {
            CurrentTier = CurrentTier,
            NextTierValue = NextTier,
            CurrentRarity = CurrentRarity,
            NextRarity = NextRarity
        };
    }

    public void AssignToBuilding(BuildingData building)
    {
        AssignedBuilding = building;

        IsAvailable = false;
    }
}
