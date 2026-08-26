using DG.Tweening;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionNameText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image progressFillImage;
    [SerializeField] private Button claimButton;
    [SerializeField] private TextMeshProUGUI claimRewardText; 
    [SerializeField] private RectTransform rewardIcon;

    private Tween progressTween;

    private MissionRuntime mission;
    public MissionRuntime Mission => mission;


    public void Initialize(MissionRuntime missionRuntime, Sprite missionIcon)
    {
        mission = missionRuntime;

        missionNameText.text = mission.Data.missionName;
        iconImage.sprite = missionIcon;

        progressText.text = mission.ProgressText;
        progressFillImage.fillAmount = mission.ProgressPercentage;

        claimButton.transform.localScale = Vector3.one;

        if (mission.CanClaim)
        {
            ShowClaimButton();
        }
        else
        {
            claimButton.gameObject.SetActive(false);
        }

        claimRewardText.text = mission.Data.rewardAmount.ToString();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<MissionProgressChangedEvent>(OnMissionProgressChanged);
        claimButton.onClick.AddListener(OnClaimButtonClicked);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;
        progressTween?.Kill();
        claimButton.transform.DOKill();
        claimButton.onClick.RemoveListener(OnClaimButtonClicked);

        EventManager.Instance.RemoveListener<MissionProgressChangedEvent>(OnMissionProgressChanged);
    }

    private void OnClaimButtonClicked()
    {
        mission.ClaimReward();

        EventManager.Instance.QueueEvent(new MissionRewardClaimedEvent(CurrencyType.Coins, mission.Data.rewardAmount, rewardIcon.position));

        claimButton.transform.DOKill();
        claimButton.gameObject.SetActive(false);
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

        claimButton.transform.DOScale(1.04f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
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