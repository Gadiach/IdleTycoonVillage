using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    [SerializeField] private MissionDatabase missionDatabase;
    [SerializeField] private MissionPanelUI missionPanelUI;

    private readonly List<MissionRuntime> allMissions = new();

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
        allMissions.Clear();

        CreateMissionRuntimes();

        RefreshUI();
    }

    private void CreateMissionRuntimes()
    {
        foreach (var missionData in missionDatabase.missions)
        {
            allMissions.Add(new MissionRuntime(missionData));
        }
    }

    private List<MissionRuntime> GetVisibleMissions()
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

    

    private void RefreshUI()
    {
        missionPanelUI.ShowMissions(GetVisibleMissions());
    }

    public void AddProgress(MissionType type, int amount)
    {
        foreach (var mission in allMissions)
        {
            if (mission.Data.missionType != type)
                continue;

            mission.AddProgress(amount);
        }
    }
}