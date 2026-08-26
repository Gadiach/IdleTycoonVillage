using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    #region Definition Data

    [Header("Definition Data")]
    public string Name;
    public string Description;
    public CurrencyType Currency;
    public ShopCategory Type;
    public BusinessType BusinessType;
    public Sprite Icon;
    public int PurchasePrice;
    [SerializeField] private int baseUpgradePrice = 5;
    [SerializeField] private int baseIncomePerCycle = 5;
    [SerializeField] private int workerLevelNeededForAutomation = 5;

    #endregion

    #region Serialized Fields

    [SerializeField] private ProgressionConfig progressionConfig;
    [SerializeField] private EconomyProgressionConfig economyConfig;
    [SerializeField] private UpgradeCostConfig upgradeCostConfig;

    [SerializeField] private bool startProductionOnPlace = true;

    #endregion

    #region Runtime State

    public int CurrentLevel { get; private set; } = 1;
    public bool IsAutomated { get; private set; }
    public int TotalIncomeCircles { get; private set; }

    public Rarities CurrentRarity { get; private set; } = Rarities.Primitive;

    public Tiers CurrentTier { get; private set; } = Tiers.Tier1;

    public BuildingPlaceable Placeable { get; private set; }

    #endregion

    #region Calculated Properties

    public bool CanUpgradeTierOrRarity => HasEnoughResourcesForTierOrRarityUpgrade(BlueprintRequirementsForNextUpgrade);

    public int PriceToUpgrade
    {
        get
        {
            return Mathf.RoundToInt(baseUpgradePrice * Mathf.Pow(upgradeCostConfig.buildingUpgradeMultiplier,CurrentLevel - 1));
        }
    }

    public int CurrentProgressionMaxIncome
    {
        get
        {
            float currentRarityMultiplier = economyConfig.GetRarityIncomeMultiplier(CurrentRarity);

            float currentTierMultiplier = economyConfig.GetTierIncomeMultiplier(CurrentTier);

            return Mathf.RoundToInt(baseIncomePerCycle * CurrentProgressionMaxLevel * currentRarityMultiplier * currentTierMultiplier);
        }
    }

    public int NextProgressionMaxIncome
    {
        get
        {
            float nextRarityMultiplier = economyConfig.GetRarityIncomeMultiplier(NextProgressionRarity);

            float nextTierMultiplier = economyConfig.GetTierIncomeMultiplier(NextProgressionTier);

            int nextRarityMaxLevel = progressionConfig.GetBuildingRarityMaxLevel(NextProgressionRarity);

            int nextTierLevelBonus = progressionConfig.GetBuildingTierLevelBonus(NextProgressionTier);

            int nextMaxLevel = nextRarityMaxLevel + nextTierLevelBonus;

            return Mathf.RoundToInt(baseIncomePerCycle * nextMaxLevel * nextRarityMultiplier * nextTierMultiplier);
        }
    }

    public int IncomePerCycle => baseIncomePerCycle * CurrentLevel;
    public int TotalIncome => IncomePerCycle * TotalIncomeCircles;
    
    public bool StartProductionOnPlace => startProductionOnPlace;
    public int LevelOfWorkerNeededForAutomation => workerLevelNeededForAutomation;

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
                CurrencyType blueprint = CurrencyHelper.GetBuildingBlueprintCurrency(CurrentRarity);

                requirements[blueprint] = currentTierIndex + 1;

                return requirements;
            }

            for (int r = 0; r <= currentRarityIndex; r++)
            {
                Rarities rarity = (Rarities)r;
                CurrencyType blueprint = CurrencyHelper.GetBuildingBlueprintCurrency(rarity);

                requirements[blueprint] = r < currentRarityIndex ? maxTier : 1;
            }

            return requirements;
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

    #endregion


    public void SetPlaceable(BuildingPlaceable placeable)
    {
        Placeable = placeable;
    }

    public void Initialize(ShopItem item)
    {
        Name = item.Name;
        PurchasePrice = item.PurchasePrice;
        Currency = item.Currency;
        Type = item.Type;
        Icon = item.Icon;
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

        EventManager.Instance.QueueEvent(new BuildingTierOrRarityChangedEvent(this));
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
            CurrencySystem.Instance.SpendCurrency(requirement.Key,requirement.Value);
        }
    }

    public int CurrentProgressionMaxLevel
    {
        get
        {
            int baseMax = progressionConfig.GetBuildingRarityMaxLevel(CurrentRarity);
            int tierBonus = progressionConfig.GetBuildingTierLevelBonus(CurrentTier);
            return baseMax + tierBonus;
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

    public int NextProgressionMaxLevel
    {
        get
        {
            int maxTier = (int)Tiers.Tier5;

            Rarities targetRarity;
            Tiers targetTier;

            if ((int)CurrentTier < maxTier)
            {
                targetRarity = CurrentRarity;
                targetTier = NextTier;
            }
            else
            {
                targetRarity = NextRarity;
                targetTier = Tiers.Tier1;
            }

            int baseMax = progressionConfig.GetBuildingRarityMaxLevel(targetRarity);
            int tierBonus = progressionConfig.GetBuildingTierLevelBonus(targetTier);

            return baseMax + tierBonus;
        }
    }

    public void CollectIncome()
    {
        CurrencySystem.Instance.AddCurrency(CurrencyType.Coins,TotalIncome);

        ResetTotalIncomeCircles();
    }

    public void UpgradeBuildingLvl()
    {
        if (CurrentLevel >= CurrentProgressionMaxLevel)
        {
            Debug.Log("Building is already at MAX level!");
            return;
        }

        if (CurrencySystem.Instance.SpendCurrency(Currency, PriceToUpgrade))
        {
            CurrentLevel++;

            EventManager.Instance.QueueEvent(new BuildingUpgradedEvent(this));
        }
    }

    public void CheckAutomationState()
    {
        bool newState = false;

        if (Placeable != null && Placeable.HasWorker())
        {
            WorkerData worker = Placeable.GetAssignedWorker();
            newState = worker.CurrentLevel >= LevelOfWorkerNeededForAutomation;
        }

        if (IsAutomated != newState)
        {
            IsAutomated = newState;
            EventManager.Instance.QueueEvent(new BuildingAutomationChangedEvent(this, IsAutomated));
        }
    }

    public void AddIncomeCircle()
    {
        TotalIncomeCircles++;
    }

    public void ResetTotalIncomeCircles()
    {
        TotalIncomeCircles = 0;
    }
}

