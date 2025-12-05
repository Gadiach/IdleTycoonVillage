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
            int max = System.Enum.GetValues(typeof(Tiers)).Length - 1;

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
            int max = System.Enum.GetValues(typeof(Rarities)).Length - 1;

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

    public int GetMaxLevel()
    {
        int baseMax = CurrentRarity switch
        {
            Rarities.Primitive => 2,
            Rarities.Developed => 4,
            Rarities.Industrial => 6,
            Rarities.Modern => 8,
            Rarities.Futuristic => 10,
            _ => 2
        };

        int tierBonus = CurrentTier switch
        {
            Tiers.Tier1 => 0,
            Tiers.Tier2 => 3,
            Tiers.Tier3 => 6,
            Tiers.Tier4 => 9,
            Tiers.Tier5 => 13,
            _ => 0
        };

        return baseMax + tierBonus;
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
        int maxLevel = GetMaxLevel();

        if (LevelOfBuilding >= maxLevel)
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
            EventManager.Instance.QueueEvent(new BuildingAutomationChangedGameEvent(this, IsAutomated));
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

