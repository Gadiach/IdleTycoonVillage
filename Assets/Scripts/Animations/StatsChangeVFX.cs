using DG.Tweening;
using TMPro;
using UnityEngine;

public class StatsChangeVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI incomeChangeText;
    [SerializeField] private TextMeshProUGUI timeChangeText;

    [Header("Animation")]
    [SerializeField] private float popScale = 1.2f;
    [SerializeField] private float popDuration = 0.15f;
    [SerializeField] private float floatDistance = 30f;
    [SerializeField] private float floatDuration = 0.5f;

    [Header("Colors")]
    [SerializeField] private Color improvementColor = Color.green;

    private TextMeshProUGUI incomeText;
    private TextMeshProUGUI timeText;

    private Vector3 incomeInitialScale;
    private Vector3 timeInitialScale;

    private Vector2 incomeChangeInitialPosition;
    private Vector2 timeChangeInitialPosition;

    private void Start()
    {
        incomeText = UIManager.Instance.IncomeText;
        timeText = UIManager.Instance.TimeText;

        incomeInitialScale = incomeText.rectTransform.localScale;
        timeInitialScale = timeText.rectTransform.localScale;

        incomeChangeInitialPosition = incomeChangeText.rectTransform.anchoredPosition;
        timeChangeInitialPosition = timeChangeText.rectTransform.anchoredPosition;

        incomeChangeText.gameObject.SetActive(false);
        timeChangeText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildingUpgradedEvent>(OnBuildingUpgraded);
        EventManager.Instance.AddListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<BuildingUpgradedEvent>(OnBuildingUpgraded);
        EventManager.Instance.RemoveListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
    }

    private void OnBuildingUpgraded(BuildingUpgradedEvent info)
    {
        Play(
            incomeText,
            incomeChangeText,
            incomeInitialScale,
            incomeChangeInitialPosition,
            $"+{info.Building.LastIncomeIncrease}",
            improvementColor
        );
    }

    private void OnWorkerUpgraded(WorkerUpgradedEvent info)
    {
        Play(
            timeText,
            timeChangeText,
            timeInitialScale,
            timeChangeInitialPosition,
            $"-{info.Worker.LastCycleDurationDecrease:F2}s",
            improvementColor
        );
    }

    private void Play(
        TextMeshProUGUI valueText,
        TextMeshProUGUI changeText,
        Vector3 initialScale,
        Vector2 initialChangePosition,
        string change,
        Color changeColor)
    {
        valueText.rectTransform.DOKill();
        changeText.DOKill();
        changeText.rectTransform.DOKill();

        valueText.rectTransform.localScale = initialScale;

        valueText.rectTransform
            .DOScale(initialScale * popScale, popDuration)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo);

        changeText.text = change;
        changeText.color = changeColor;
        changeText.gameObject.SetActive(true);
        changeText.rectTransform.anchoredPosition = initialChangePosition;

        Color color = changeText.color;
        color.a = 1f;
        changeText.color = color;

        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            changeText.rectTransform
                .DOAnchorPosY(initialChangePosition.y + floatDistance, floatDuration)
                .SetEase(Ease.OutQuad)
        );

        sequence.Join(
            changeText
                .DOFade(0f, floatDuration)
                .SetEase(Ease.InQuad)
        );

        sequence.OnComplete(() =>
        {
            changeText.gameObject.SetActive(false);
            changeText.rectTransform.anchoredPosition = initialChangePosition;
        });
    }
}