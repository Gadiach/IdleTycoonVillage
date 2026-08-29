using System.Collections.Generic;
using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    public static MissionSystem Instance;

    [SerializeField] private MissionDatabase missionDatabase;

    private readonly List<MissionRuntime> allMissions = new();

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        InitializeMissions();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<CurrencyAddedEvent>(OnCurrencyAdded);
        EventManager.Instance.AddListener<MissionClaimedEvent>(OnMissionClaimed);
        EventManager.Instance.AddListener<BuildingPlacedEvent>(OnBuildingPlaced);
        EventManager.Instance.AddListener<BuildingUpgradedEvent>(OnBuildingUpgraded);
        EventManager.Instance.AddListener<WorkerAssignedToBuildingEvent>(OnWorkerAssigned);
        EventManager.Instance.AddListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<CurrencyAddedEvent>(OnCurrencyAdded);
        EventManager.Instance.RemoveListener<MissionClaimedEvent>(OnMissionClaimed);
        EventManager.Instance.RemoveListener<BuildingPlacedEvent>(OnBuildingPlaced);
        EventManager.Instance.RemoveListener<BuildingUpgradedEvent>(OnBuildingUpgraded);
        EventManager.Instance.RemoveListener<WorkerAssignedToBuildingEvent>(OnWorkerAssigned);
        EventManager.Instance.RemoveListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
    }

    #region Initialization

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void InitializeMissions()
    {
        allMissions.Clear();

        InitializeMissionRuntimes();
        InitializeMissionUI();
    }

    private void InitializeMissionRuntimes()
    {
        foreach (var missionData in missionDatabase.missions)
        {
            allMissions.Add(new MissionRuntime(missionData));
        }
    }

    private void InitializeMissionProgress(MissionRuntime mission)
{
    switch (mission.Data.missionType)
    {
        case MissionType.UpgradeBuilding:
            InitializeBuildingUpgradeProgress(mission);
            break;

        case MissionType.UpgradeWorker:
            InitializeWorkerUpgradeProgress(mission);
            break;
    }
}

    private void InitializeBuildingUpgradeProgress(MissionRuntime mission)
    {
        BuildingData building = EntityRegistry.Instance.GetBuilding(
            mission.Data.TargetBusinessType
        );

        if (building == null)
            return;

        if (mission.Data.NeedRarity &&
            mission.Data.TargetRarity != building.CurrentRarity)
            return;

        mission.SetTargetValue(building.CurrentProgressionMaxLevel);
        mission.SetProgress(building.CurrentLevel);
    }

    private void InitializeWorkerUpgradeProgress(MissionRuntime mission)
    {
        WorkerData worker = EntityRegistry.Instance.GetWorker(
            mission.Data.TargetBusinessType
        );

        if (worker == null)
            return;

        if (mission.Data.NeedRarity &&
            mission.Data.TargetRarity != worker.CurrentRarity)
            return;

        mission.SetTargetValue(worker.CurrentProgressionMaxLevel);
        mission.SetProgress(worker.CurrentLevel);
    }

    private void InitializeMissionUI()
    {
        EventManager.Instance.QueueEvent(new MissionListChangedEvent(GetActiveMissions()));
    }

    #endregion

    private List<MissionRuntime> GetActiveMissions()
    {
        List<MissionRuntime> result = new();

        foreach (var mission in allMissions)
        {
            if (mission.Claimed)
                continue;

            InitializeMissionProgress(mission);

            result.Add(mission);

            if (result.Count >= 3)
                break;
        }

        return result;
    }

    private void OnCurrencyAdded(CurrencyAddedEvent info)
    {
        if (info.CurrencyType == CurrencyType.Coins)
        {
            UpdateMissionProgress(MissionType.CollectCoins, info.Amount);
        }
    }

    private void OnMissionClaimed(MissionClaimedEvent info)
    {
        EventManager.Instance.QueueEvent(new MissionListChangedEvent(GetActiveMissions()));
    }

    private void OnBuildingPlaced(BuildingPlacedEvent info)
    {
        UpdateMissionProgress(MissionType.BuildBuilding, 1);

        UpdateBuildingUpgradeProgress(info.Building);
    }

    private void UpdateWorkerUpgradeProgress(WorkerData worker)
    {
        foreach (MissionRuntime mission in allMissions)
        {
            if (mission.Data.missionType != MissionType.UpgradeWorker)
                continue;

            if (mission.Completed)
                continue;

            if (mission.Data.NeedBusinessType &&
                mission.Data.TargetBusinessType != worker.Type)
                continue;

            if (mission.Data.NeedRarity &&
                mission.Data.TargetRarity != worker.CurrentRarity)
                continue;

            mission.SetTargetValue(worker.CurrentProgressionMaxLevel);
            mission.SetProgress(worker.CurrentLevel);

            EventManager.Instance.QueueEvent(
                new MissionProgressChangedEvent(mission)
            );
        }
    }

    private void UpdateBuildingUpgradeProgress(BuildingData building)
    {
        foreach (MissionRuntime mission in allMissions)
        {
            if (mission.Data.missionType != MissionType.UpgradeBuilding)
                continue;

            if (mission.Completed)
                continue;

            if (mission.Data.NeedBusinessType &&
                mission.Data.TargetBusinessType != building.BusinessType)
                continue;

            if (mission.Data.NeedRarity &&
                mission.Data.TargetRarity != building.CurrentRarity)
                continue;

            mission.SetTargetValue(building.CurrentProgressionMaxLevel);
            mission.SetProgress(building.CurrentLevel);

            EventManager.Instance.QueueEvent(
                new MissionProgressChangedEvent(mission)
            );
        }
    }

    private void OnWorkerAssigned(WorkerAssignedToBuildingEvent info)
    {
        foreach (var mission in allMissions)
        {
            if (mission.Data.missionType != MissionType.HireWorker)
                continue;

            if (mission.Completed)
                continue;

            if (mission.Data.NeedBusinessType && mission.Data.TargetBusinessType != info.Worker.Type)
                continue;

            mission.AddProgress(1);

            EventManager.Instance.QueueEvent(new MissionProgressChangedEvent(mission));

            break;
        }
    }

    private void OnBuildingUpgraded(BuildingUpgradedEvent info)
    {
        UpdateBuildingUpgradeProgress(info.Building);
    }

    private void OnWorkerUpgraded(WorkerUpgradedEvent info)
    {
        UpdateWorkerUpgradeProgress(info.Worker);
    }

    private void UpdateMissionProgress(MissionType missionType, int amount)
    {
        int remainingProgress = amount;

        foreach (var mission in allMissions)
        {
            if (remainingProgress <= 0)
                break;

            if (mission.Data.missionType != missionType)
                continue;

            if (mission.Completed)
                continue;

            int progressToAdd = Mathf.Min(remainingProgress, mission.RemainingProgress);

            mission.AddProgress(progressToAdd);

            remainingProgress -= progressToAdd;

            EventManager.Instance.QueueEvent(new MissionProgressChangedEvent(mission));
        }
    }
}