using UnityEngine;

public class MissionPanelUI : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private MissionItemUI missionPrefab;

    private void Start()
    {
        SpawnTestMissions();
    }

    private void SpawnTestMissions()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(missionPrefab, content);
        }
    }
}