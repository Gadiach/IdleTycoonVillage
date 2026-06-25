
public abstract class GameEvent { }

public class CurrencyAddedEvent : GameEvent
{
    public CurrencyType CurrencyType { get; private set; }
    public int Amount { get; private set; }

    public CurrencyAddedEvent(CurrencyType currencyType, int amount)
    {
        CurrencyType = currencyType;
        Amount = amount;
    }
}

public class CurrencySpentEvent : GameEvent
{
    public CurrencyType CurrencyType { get; private set; }
    public int Amount { get; private set; }

    public CurrencySpentEvent(CurrencyType currencyType, int amount)
    {
        CurrencyType = currencyType;
        Amount = amount;
    }
}

public class NotEnoughCurrencyEvent : GameEvent
{
    public int amount;
    public CurrencyType currencyType;

    public NotEnoughCurrencyEvent(int amount, CurrencyType currencyType)
    {
        this.amount = amount;
        this.currencyType = currencyType;
    }
}

public class BuildingTierOrRarityChangedEvent : GameEvent
{
    public BuildingData Building { get; private set; }

    public BuildingTierOrRarityChangedEvent(BuildingData building)
    {
        Building = building;
    }
}

public class WorkerTierOrRarityChangedEvent : GameEvent
{
    public WorkerData Worker { get; private set; }

    public WorkerTierOrRarityChangedEvent(WorkerData worker)
    {
        Worker = worker;
    }
}

public class XPAddedEvent : GameEvent
{
    public int amount;

    public XPAddedEvent(int amount)
    {
        this.amount = amount;
    }
}

public class LevelChangedEvent : GameEvent
{
    public int newLvl;

    public LevelChangedEvent(int currLvl)
    {
        newLvl = currLvl;
    }
}

public class WorkerUpgradedEvent : GameEvent
{
    public WorkerData Worker { get; private set; }

    public WorkerUpgradedEvent(WorkerData worker)
    {
        Worker = worker;
    }
}

public class BuildingAutomationChangedEvent : GameEvent
{
    public BuildingData Building { get; private set; }
    public bool IsAutomated { get; private set; }

    public BuildingAutomationChangedEvent(BuildingData building, bool isAutomated)
    {
        Building = building;
        IsAutomated = isAutomated;
    }
}

public class BuildingPlacedEvent : GameEvent
{
    public BuildingData Building { get; private set; }

    public BuildingPlacedEvent(BuildingData building)
    {
        Building = building;
    }
}

public class WorkerAssignedToBuildingEvent : GameEvent
{
    public BuildingData Building { get; private set; }

    public WorkerData Worker { get; private set; }

    public WorkerAssignedToBuildingEvent(BuildingData building, WorkerData worker)
    {
        Building = building;
        Worker = worker;
    }
}



