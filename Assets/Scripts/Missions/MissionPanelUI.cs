using System.Collections.Generic;
using UnityEngine;

public class MissionPanelUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private MissionItemUI missionPrefab;
    private readonly List<MissionItemUI> missionItems = new();
    [SerializeField] private MissionIconDatabase missionIconDatabase;

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
        RebuildMissionUI(info.Missions);
    }

    public void RebuildMissionUI(List<MissionRuntime> missions)
    {
        Clear();

        foreach (var mission in missions)
        {
            MissionItemUI item = Instantiate(missionPrefab, content);

            Sprite missionIcon = missionIconDatabase.GetIcon(mission.Data.missionType,mission.Data.TargetBusinessType, mission.Data.TargetRarity);

            item.Initialize(mission, missionIcon);

            missionItems.Add(item);
        }
    }

    private void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        missionItems.Clear();
    }
}