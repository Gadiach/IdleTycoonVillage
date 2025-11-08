using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent { }

public class CurrencyChangeGameEvent : GameEvent
{
    public int amount;
    public CurrencyType currencyType;

    public CurrencyChangeGameEvent(int amount, CurrencyType currencyType)
    {
        this.amount = amount;
        this.currencyType = currencyType;
    }
}

public class NotEnoughCurrencyGameEvent : GameEvent
{
    public int amount;
    public CurrencyType currencyType;

    public NotEnoughCurrencyGameEvent(int amount, CurrencyType currencyType)
    {
        this.amount = amount;
        this.currencyType = currencyType;
    }
}

public class EnoughCurrencyGameEvent : GameEvent
{

}

public class XPAddedGameEvent : GameEvent
{
    public int amount;

    public XPAddedGameEvent(int amount)
    {
        this.amount = amount;
    }
}

public class LevelChangedGameEvent : GameEvent
{
    public int newLvl;

    public LevelChangedGameEvent(int currLvl)
    {
        newLvl = currLvl;
    }
}

public class WorkerUpgradedGameEvent : GameEvent
{
    public WorkerData Worker { get; private set; }

    public WorkerUpgradedGameEvent(WorkerData worker)
    {
        Worker = worker;
    }
}

public class BuildingAutomationChangedGameEvent : GameEvent
{
    public BuildingData Building { get; private set; }
    public bool IsAutomated { get; private set; }

    public BuildingAutomationChangedGameEvent(BuildingData building, bool isAutomated)
    {
        Building = building;
        IsAutomated = isAutomated;
    }
}



