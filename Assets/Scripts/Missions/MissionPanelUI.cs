using System.Collections.Generic;
using UnityEngine;

public class MissionPanelUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private MissionItemUI missionPrefab;
    [SerializeField] private MissionIconDatabase missionIconDatabase;

    private readonly List<MissionItemUI> missionItems = new();

    private void OnEnable()
    {
        EventManager.Instance.AddListener<MissionListChangedEvent>(OnMissionListChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<MissionListChangedEvent>(OnMissionListChanged);
    }

    private void OnMissionListChanged(MissionListChangedEvent info)
    {
        UpdateMissionUI(info.Missions);
    }

    private void UpdateMissionUI(List<MissionRuntime> missions)
    {
        RemoveInactiveMissions(missions);
        AddNewMissions(missions);
    }

    private void RemoveInactiveMissions(List<MissionRuntime> missions)
    {
        for (int i = missionItems.Count - 1; i >= 0; i--)
        {
            MissionItemUI item = missionItems[i];

            if (missions.Contains(item.Mission))
                continue;

            missionItems.RemoveAt(i);
            Destroy(item.gameObject);
        }
    }

    private void AddNewMissions(List<MissionRuntime> missions)
    {
        foreach (MissionRuntime mission in missions)
        {
            if (HasMissionItem(mission))
                continue;

            MissionItemUI item = Instantiate(missionPrefab, content);

            Sprite missionIcon = missionIconDatabase.GetIcon(
                mission.Data.missionType,
                mission.Data.TargetBusinessType,
                mission.Data.TargetRarity
            );

            item.Initialize(mission, missionIcon);

            missionItems.Add(item);
        }
    }

    private bool HasMissionItem(MissionRuntime mission)
    {
        foreach (MissionItemUI item in missionItems)
        {
            if (item.Mission == mission)
                return true;
        }

        return false;
    }
}