using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image iconImage;

    private MissionRuntime mission;

    public void Initialize(MissionRuntime missionRuntime)
    {
        mission = missionRuntime;

        missionNameText.text = mission.Data.missionName;
        iconImage.sprite = mission.Data.icon;

        Refresh(mission);
    }

    public void Refresh(MissionRuntime missionRuntime)
    {
        progressText.text = $"{missionRuntime.Progress}/{missionRuntime.Data.targetValue}";
    }
}