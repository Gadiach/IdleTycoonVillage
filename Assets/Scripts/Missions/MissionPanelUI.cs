using System.Collections.Generic;
using UnityEngine;

public class MissionPanelUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private MissionItemUI missionPrefab;

    public void ShowMissions(List<MissionRuntime> missions)
    {
        Clear();

        foreach (var mission in missions)
        {
            MissionItemUI item = Instantiate(missionPrefab, content);

            item.Initialize(mission);
        }
    }

    private void Clear()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void Refresh(List<MissionRuntime> missions)
    {
        MissionItemUI[] items =
            content.GetComponentsInChildren<MissionItemUI>();

        for (int i = 0; i < items.Length; i++)
        {
            items[i].Refresh(missions[i]);
        }
    }
}