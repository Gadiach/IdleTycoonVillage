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
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<CurrencyAddedEvent>(OnCurrencyAdded);
        EventManager.Instance.RemoveListener<MissionClaimedEvent>(OnMissionClaimed);
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