using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public string Name;// { get;  set; }
    public string Description;// { get; private set; }
    public int Level; //{ get;  set; }
    public int Price; //{ get;  set; }
    public CurrencyType Currency;// { get; private set; }
    public ObjectType Type;// { get; private set; }
    public BusinessType BusinessType;
    public Sprite Icon;// { get;  set; }
    public int PriceToUpgrade = 5;
    public int LevelOfBuilding = 1;
    public int Income;
    public int LevelOfWorkerNeededForAutomation = 5;
    public bool IsAutomated;
    public int TotalIncomeCircles = 0;

    public Rarities CurrentRarity = Rarities.Primitive;
    public Tiers CurrentTier = Tiers.Tier1;

    [SerializeField] private ProgressionConfig progressionConfig;

    public BuildingPlaceable Placeable { get; private set; }

    public void SetPlaceable(BuildingPlaceable placeable)
    {
        Placeable = placeable;
    }

    public void Initialize(ShopItem item)
    {
        Name = item.Name;
        Description = item.Description;
        Level = item.Level;
        Price = item.Price;
        Currency = item.Currency;
        Type = item.Type;
        Icon = item.Icon;

        UpdatePriceToUpgrade();
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

    public void UpdatePriceToUpgrade()
    {
        PriceToUpgrade = PriceToUpgrade * LevelOfBuilding;
    }

    public void UpdateIncome()
    {
        Income = 5 * LevelOfBuilding;
    }

    public void UpgradeBuilding()
    {
        if (LevelOfBuilding >= CurrentTierMaxLevel)
        {
            Debug.Log("Building is already at MAX level!");
            return;
        }

        if (CurrencySystem.Instance.TrySpendCurrency(Currency, PriceToUpgrade))
        {
            LevelOfBuilding++;
            UpdatePriceToUpgrade();
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

