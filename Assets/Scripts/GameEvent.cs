
public abstract class GameEvent { }

public class RequestCurrencyChangeEvent : GameEvent
{
    public int amount;
    public CurrencyType currencyType;

    public RequestCurrencyChangeEvent(int amount, CurrencyType currencyType)
    {
        this.amount = amount;
        this.currencyType = currencyType;
    }
}

public class CurrencyChangedEvent : GameEvent
{
    public CurrencyType CurrencyType;

    public CurrencyChangedEvent(
        CurrencyType currencyType)
    {
        CurrencyType = currencyType;
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

public class EnoughCurrencyEvent : GameEvent
{

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



