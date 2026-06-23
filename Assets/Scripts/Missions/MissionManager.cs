using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [SerializeField] private MissionDatabase missionDatabase;
    [SerializeField] private MissionPanelUI missionPanelUI;

    private readonly List<MissionRuntime> activeMissions = new();

    private const int ActiveMissionCount = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeMissions();
    }

    private void InitializeMissions()
    {
        activeMissions.Clear();

        int count = Mathf.Min(ActiveMissionCount, missionDatabase.missions.Count);

        for (int i = 0; i < count; i++)
        {
            activeMissions.Add(new MissionRuntime(missionDatabase.missions[i]));
        }

        missionPanelUI.ShowMissions(activeMissions);
    }

    public void AddProgress(MissionType type, int amount)
    {
        foreach (var mission in activeMissions)
        {
            if (mission.Completed)
                continue;

            if (mission.Data.missionType != type)
                continue;

            mission.Progress += amount;

            if (mission.Progress >= mission.Data.targetValue)
            {
                mission.Progress = mission.Data.targetValue;
                mission.Completed = true;

                Debug.Log($"Mission completed: {mission.Data.missionName}");
            }
        }

        missionPanelUI.Refresh(activeMissions);
    }
}