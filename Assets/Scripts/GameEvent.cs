using System.Collections.Generic;
using UnityEngine;

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

public class MissionProgressChangedEvent : GameEvent
{
    public MissionRuntime Mission { get; }

    public MissionProgressChangedEvent(MissionRuntime mission)
    {
        Mission = mission;
    }
}

public class MissionClaimedEvent : GameEvent
{
    public MissionRuntime Mission { get; }

    public MissionClaimedEvent(MissionRuntime mission)
    {
        Mission = mission;
    }
}

public class BuildingUpgradedEvent : GameEvent
{
    public BuildingData Building { get; }

    public BuildingUpgradedEvent(BuildingData building)
    {
        Building = building;
    }
}

public class MissionListChangedEvent : GameEvent
{
    public List<MissionRuntime> Missions { get; }

    public MissionListChangedEvent(List<MissionRuntime> missions)
    {
        Missions = missions;
    }
}

public class MissionRewardClaimedEvent : GameEvent
{
    public CurrencyType CurrencyType { get; private set; }
    public int Amount { get; private set; }
    public Vector3 SourcePosition { get; private set; }

    public MissionRewardClaimedEvent(CurrencyType currencyType, int amount,Vector3 sourcePosition)
    {
        CurrencyType = currencyType;
        Amount = amount;
        SourcePosition = sourcePosition;
    }
}

public class BuildingIncomeCollectedEvent : GameEvent
{
    public Vector3 Position { get; }

    public BuildingIncomeCollectedEvent(Vector3 position)
    {
        Position = position;
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

public class ShowMissionPanelEvent : GameEvent
{
}

public class CurrencyVFXCompletedEvent : GameEvent
{
    public CurrencyType CurrencyType { get; }

    public CurrencyVFXCompletedEvent(CurrencyType currencyType)
    {
        CurrencyType = currencyType;
    }
}

public class ShopItemDragStartedEvent : GameEvent
{
    public ShopItem ShopItem { get; }

    public ShopItemDragStartedEvent(ShopItem shopItem)
    {
        ShopItem = shopItem;
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



