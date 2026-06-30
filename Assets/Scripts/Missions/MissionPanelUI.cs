using System.Collections.Generic;
using UnityEngine;

public class MissionPanelUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private MissionItemUI missionPrefab;
    private readonly List<MissionItemUI> missionItems = new();

    private void OnEnable()
    {
        EventManager.Instance.AddListener<MissionListChangedEvent>(OnMissionInitialized);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<MissionListChangedEvent>(OnMissionInitialized);
    }

    private void OnMissionInitialized(MissionListChangedEvent info)
    {
        RebuildMissionUI(info.Missions);
    }

    public void RebuildMissionUI(List<MissionRuntime> missions)
    {
        Clear();

        foreach (var mission in missions)
        {
            MissionItemUI item = Instantiate(missionPrefab, content);

            item.Initialize(mission);

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