using System.Collections.Generic;
using UnityEngine;

public class MissionPanelUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private MissionItemUI missionPrefab;
    private readonly List<MissionItemUI> missionItems = new();

    public void ShowMissions(List<MissionRuntime> missions)
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