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

    public CurrencyType Currency => Definition.Currency;

    private float BaseProductionDuration => Definition.BaseProductionDuration;

    private float BaseUpgradePrice => Definition.BaseUpgradePrice;

    private float ProductionTimeReductionPerLevel => Definition.ProductionTimeReductionPerLevel;

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

    public float CurrentProgressionMinCycleDuration => CalculateCycleDuration(CurrentProgressionMaxLevel,CurrentRarity,CurrentTier);

    public float NextProgressionMinCycleDuration => CalculateCycleDuration(NextProgressionMaxLevel,NextProgressionRarity,NextProgressionTier);

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

    public int PriceToUpgrade
    {
        get
        {
            return Mathf.RoundToInt(BaseUpgradePrice * Mathf.Pow(upgradeCostConfig.workerUpgradeMultiplier, CurrentLevel - 1));
        }
    }

    public float CycleDuration => CalculateCycleDuration(CurrentLevel,CurrentRarity,CurrentTier);

    public Dictionary<CurrencyType, int> BlueprintRequirementsForNextUpgrade
    {
        get
        {
            Dictionary<CurrencyType, int> requirements = new();

            int currentRarityIndex = (int)CurrentRarity;
            int currentTierIndex = (int)CurrentTier;

            int maxTier = Enum.GetValues(typeof(Tiers)).Length;

            if (currentTierIndex < maxTier)
            {
                CurrencyType blueprint = CurrencyHelper.GetWorkerBlueprintCurrency(CurrentRarity);

                requirements[blueprint] = currentTierIndex + 1;
                return requirements;
            }

            for (int r = 0; r <= currentRarityIndex; r++)
            {
                Rarities rarity = (Rarities)r;
                CurrencyType blueprint = CurrencyHelper.GetWorkerBlueprintCurrency(rarity);

                if (r < currentRarityIndex)
                    requirements[blueprint] = maxTier;
                else
                    requirements[blueprint] = 1;
            }

            return requirements;
        }
    }

    public bool CanUpgradeTierOrRarity => HasEnoughResourcesForTierOrRarityUpgrade(BlueprintRequirementsForNextUpgrade);

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
        if (CurrentLevel >= CurrentProgressionMaxLevel)
        {
            Debug.Log("Worker is already at MAX level!");
            return;
        }

        if (CurrencySystem.Instance.SpendCurrency(Currency, PriceToUpgrade))
        {
            CurrentLevel++;
        }
    }

    private float CalculateCycleDuration(int level, Rarities rarity, Tiers tier)
    {
        float rarityMultiplier = economyConfig.GetRarityProductionTimeMultiplier(rarity);

        float tierMultiplier = economyConfig.GetTierProductionTimeMultiplier(tier);

        float levelMultiplier = 1f - ((level - 1) * ProductionTimeReductionPerLevel);

        return BaseProductionDuration * rarityMultiplier * tierMultiplier * levelMultiplier;
    }

    private bool HasEnoughResourcesForTierOrRarityUpgrade(Dictionary<CurrencyType, int> requirements)
    {
        foreach (var requirement in requirements)
        {
            if (!CurrencySystem.Instance.HasEnoughCurrency(requirement.Key, requirement.Value))
            {
                return false;
            }
        }

        return true;
    }

    public void UpgradeTierOrRarity()
    {
        var requirements = BlueprintRequirementsForNextUpgrade;

        if (!HasEnoughResourcesForTierOrRarityUpgrade(requirements))
            return;

        SpendUpgradeRequirements(requirements);

        ApplyTierOrRarityUpgrade();

        EventManager.Instance.QueueEvent(new WorkerTierOrRarityChangedEvent(this));
    }

    private void ApplyTierOrRarityUpgrade()
    {
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
    }

    private void SpendUpgradeRequirements(Dictionary<CurrencyType, int> requirements)
    {
        foreach (var requirement in requirements)
        {
            CurrencySystem.Instance.SpendCurrency(requirement.Key, requirement.Value);
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
