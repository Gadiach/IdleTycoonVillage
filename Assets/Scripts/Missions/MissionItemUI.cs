using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image progressFillImage;

    private MissionRuntime mission;

    public void Initialize(MissionRuntime missionRuntime)
    {
        mission = missionRuntime;

        missionNameText.text = mission.Data.missionName;
        iconImage.sprite = mission.Data.icon;

        UpdateProgressUI(mission);
    }

    public void UpdateProgressUI(MissionRuntime missionRuntime)
    {
        progressText.text = missionRuntime.ProgressText;

        progressFillImage.fillAmount = missionRuntime.ProgressPercentage;

        //claimButton.gameObject.SetActive(missionRuntime.CanClaim);
    }
}