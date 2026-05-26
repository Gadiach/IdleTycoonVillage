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
    public ObjectType Type;
    public BusinessType BusinessType;
    public Sprite Icon;
    public int PurchasePrice;

    #endregion

    #region Serialized Fields

    [SerializeField] private BuildingBalanceConfig balanceConfig;
    [SerializeField] private ProgressionConfig progressionConfig;
    [SerializeField] private EconomyProgressionConfig economyConfig;

    [SerializeField] private bool startProductionOnPlace = true;

    #endregion

    #region Runtime State

    public int CurrentLevel { get; private set; } = 1;
    public bool IsAutomated { get; private set; }
    public int TotalIncomeCircles { get; private set; }

    public Rarities CurrentRarity = Rarities.Primitive;
    public Tiers CurrentTier = Tiers.Tier1;

    public BuildingPlaceable Placeable { get; private set; }

    #endregion

    #region Calculated Properties

    public int PriceToUpgrade
    {
        get
        {
            float rarityMultiplier = economyConfig.GetRarityIncomeMultiplier(CurrentRarity);

            float tierMultiplier = economyConfig.GetTierIncomeMultiplier(CurrentTier);

            float progressionPrice = Mathf.Pow(balanceConfig.upgradeMultiplier, CurrentLevel - 1);

            float basePrice = balanceConfig.baseUpgradePrice * rarityMultiplier * tierMultiplier;

            return Mathf.RoundToInt(basePrice * progressionPrice);
        }
    }

    public float ProductionDuration
    {
        get
        {
            float rarityMultiplier = economyConfig.GetRarityProductionTimeMultiplier(CurrentRarity);

            float tierMultiplier = economyConfig.GetTierProductionTimeMultiplier(CurrentTier);

            return balanceConfig.baseProductionDuration *
                   rarityMultiplier *
                   tierMultiplier;
        }
    }

    public int IncomePerCycle => balanceConfig.baseIncomePerCycle * CurrentLevel;
    public int TotalIncome => IncomePerCycle * TotalIncomeCircles;
    
    public bool StartProductionOnPlace => startProductionOnPlace;
    public int LevelOfWorkerNeededForAutomation => balanceConfig.workerLevelNeededForAutomation;

    #endregion
    

    public void SetPlaceable(BuildingPlaceable placeable)
    {
        Placeable = placeable;
    }

    public void Initialize(ShopItem item)
    {
        Name = item.Name;
        Description = item.Description;
        PurchasePrice = item.PurchasePrice;
        Currency = item.Currency;
        Type = item.Type;
        Icon = item.Icon;
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

        EventManager.Instance.QueueEvent(new BuildingTierOrRarityChangedEvent(this));
    }

    public int CurrentTierMaxLevel
    {
        get
        {
            int baseMax = progressionConfig.GetBaseMaxLevel(CurrentRarity);
            int tierBonus = progressionConfig.GetTierBonus(CurrentTier);
            return baseMax + tierBonus;
        }
    }

    public int NextTierMaxLevel
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

            int baseMax = progressionConfig.GetBaseMaxLevel(targetRarity);
            int tierBonus = progressionConfig.GetTierBonus(targetTier);

            return baseMax + tierBonus;
        }
    }

    public void CollectIncome()
    {
        EventManager.Instance.QueueEvent(new RequestCurrencyChangeEvent(TotalIncome, CurrencyType.Coins));

        ResetTotalIncomeCircles();
    }

    public void UpgradeBuilding()
    {
        if (CurrentLevel >= CurrentTierMaxLevel)
        {
            Debug.Log("Building is already at MAX level!");
            return;
        }

        if (CurrencySystem.Instance.TrySpendCurrency(Currency, PriceToUpgrade))
        {
            CurrentLevel++;
        }
    }

    public void CheckAutomationState()
    {
        bool newState = false;

        if (Placeable != null && Placeable.HasWorker())
        {
            WorkerData worker = Placeable.GetAssignedWorker();
            newState = worker.level >= LevelOfWorkerNeededForAutomation;
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

