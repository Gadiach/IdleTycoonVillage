using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MissionItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image progressFillImage;
    [SerializeField] private Button claimButton;
    private Tween progressTween;
    

    private MissionRuntime mission;

    public void Initialize(MissionRuntime missionRuntime)
    {
        mission = missionRuntime;

        missionNameText.text = mission.Data.missionName;
        iconImage.sprite = mission.Data.icon;

        progressText.text = mission.ProgressText;
        progressFillImage.fillAmount = mission.ProgressPercentage;

        claimButton.gameObject.SetActive(false);
        claimButton.transform.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<MissionProgressChangedEvent>(OnMissionProgressChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;
        progressTween?.Kill();
        claimButton.transform.DOKill();

        EventManager.Instance.RemoveListener<MissionProgressChangedEvent>(OnMissionProgressChanged);
    }

    private void OnMissionProgressChanged(MissionProgressChangedEvent info)
    {
        if (info.Mission != mission)
            return;

        UpdateProgressUI(info.Mission);
    }

    private void ShowClaimButton()
    {
        claimButton.gameObject.SetActive(true);

        claimButton.transform.DOScale(1.1f, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void UpdateProgressUI(MissionRuntime missionRuntime)
    {
        progressText.text = missionRuntime.ProgressText;

        progressTween?.Kill();

        progressTween = progressFillImage
            .DOFillAmount(missionRuntime.ProgressPercentage, 0.6f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                if (missionRuntime.CanClaim)
                {
                    ShowClaimButton();
                }
            });
    }
}