using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CurrencyVFXSystem : MonoBehaviour
{
    private readonly Dictionary<RectTransform, Vector3> targetOriginalScales = new();

    [Header("References")]
    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;
    [SerializeField] private RectTransform flyingCurrencyPrefabsContainer;
    [SerializeField] private GameObject flyingCurrencyIconPrefab;
    [SerializeField] private CurrencyVFXTarget[] currencyTargets;

    [Header("Animation")]
    [SerializeField] private int flyingIconCount = 8;
    [SerializeField] private float scatterDuration = 0.15f;
    [SerializeField] private float flyDuration = 0.6f;
    [SerializeField] private float spawnInterval = 0.05f;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<MissionRewardClaimedEvent>(OnMissionRewardClaimed);
        EventManager.Instance.AddListener<CurrencyShopRewardClaimedEvent>(OnCurrencyShopRewardClaimed);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<MissionRewardClaimedEvent>(OnMissionRewardClaimed);
        EventManager.Instance.RemoveListener<CurrencyShopRewardClaimedEvent>(OnCurrencyShopRewardClaimed);
    }

    private void Awake()
    {
        foreach (CurrencyVFXTarget target in currencyTargets)
        {
            if (target.targetIcon != null)
            {
                targetOriginalScales[target.targetIcon] =
                    target.targetIcon.localScale;
            }
        }
    }

    private void OnMissionRewardClaimed(MissionRewardClaimedEvent info)
    {
        PlayCurrencyVFX(
            info.CurrencyType,
            info.Amount,
            info.SourcePosition
        );
    }

    private void OnCurrencyShopRewardClaimed(CurrencyShopRewardClaimedEvent info)
    {
        PlayCurrencyVFX(
            info.CurrencyType,
            info.Amount,
            info.SourcePosition
        );
    }

    private void PlayCurrencyVFX(
    CurrencyType currencyType,
    int amount,
    Vector3 sourcePosition)
    {
        Sprite currencyIcon =
            currencyIconDatabase.GetIcon(currencyType);

        RectTransform targetIcon =
            GetTargetIcon(currencyType);

        if (currencyIcon == null || targetIcon == null)
            return;

        int iconCount = IsBlueprint(currencyType)
            ? amount
            : flyingIconCount;

        Play(
     sourcePosition,
     currencyIcon,
     targetIcon,
     currencyType,
     amount,
     iconCount
 );
    }

    private bool IsBlueprint(CurrencyType currencyType)
    {
        return currencyType != CurrencyType.Coins &&
               currencyType != CurrencyType.Crystals;
    }

    private void Play(
    Vector3 startPosition,
    Sprite currencyIcon,
    RectTransform targetIcon,
    CurrencyType currencyType,
    int amount,
    int iconCount)
    {
        int finalAmount =
            CurrencySystem.GetCurrencyAmount(currencyType);

        int startAmount =
            finalAmount - amount;

        for (int i = 0; i < iconCount; i++)
        {
            SpawnCurrencyIcon(
                startPosition,
                currencyIcon,
                targetIcon,
                currencyType,
                startAmount,
                amount,
                i,
                iconCount
            );
        }
    }

    private void SpawnCurrencyIcon(
    Vector3 startPosition,
    Sprite currencyIcon,
    RectTransform targetIcon,
    CurrencyType currencyType,
    int startAmount,
    int rewardAmount,
    int index,
    int iconCount)
    {
        GameObject currencyObject = Instantiate(flyingCurrencyIconPrefab,flyingCurrencyPrefabsContainer);

        RectTransform currencyRect = currencyObject.GetComponent<RectTransform>();

        Image currencyImage = currencyObject.GetComponent<Image>();

        currencyImage.sprite = currencyIcon;

        currencyRect.position = startPosition;
        currencyRect.localScale = Vector3.zero;

        Vector3 scatterPosition = startPosition + new Vector3(UnityEngine.Random.Range(-50f, 50f),UnityEngine.Random.Range(-30f, 60f),0f);

        Sequence sequence = DOTween.Sequence();

        sequence.SetDelay(index * spawnInterval);

        sequence.Append(
            currencyRect
                .DOScale(1f, scatterDuration)
                .SetEase(Ease.OutBack)
        );

        sequence.Join(
            currencyRect
                .DOMove(scatterPosition, scatterDuration)
                .SetEase(Ease.OutQuad)
        );

        sequence.Append(
            currencyRect
                .DOMove(targetIcon.position, flyDuration)
                .SetEase(Ease.InQuad)
        );

        sequence.Join(
            currencyRect
                .DOScale(0.6f, flyDuration)
                .SetEase(Ease.InQuad)
        );

        sequence.OnComplete(() =>
        {
            PulseTarget(targetIcon);

            int displayedAmount =
                startAmount +
                Mathf.RoundToInt(
                    rewardAmount * ((index + 1f) / iconCount)
                );

            EventManager.Instance.QueueEvent(
                new CurrencyVFXIconArrivedEvent(
                    currencyType,
                    displayedAmount
                )
            );

            Destroy(currencyObject);

            if (index == iconCount - 1)
            {
                EventManager.Instance.QueueEvent(
                    new CurrencyVFXCompletedEvent(currencyType)
                );
            }
        });
    }

    private CurrencyVFXTargetType GetTargetType(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.Coins:
                return CurrencyVFXTargetType.Coin;

            case CurrencyType.BuildingBlueprint_Primitive:
            case CurrencyType.BuildingBlueprint_Developed:
            case CurrencyType.BuildingBlueprint_Industrial:
            case CurrencyType.BuildingBlueprint_Modern:
            case CurrencyType.BuildingBlueprint_Futuristic:
                return CurrencyVFXTargetType.BuildingBlueprints;

            case CurrencyType.WorkerBlueprint_Primitive:
            case CurrencyType.WorkerBlueprint_Developed:
            case CurrencyType.WorkerBlueprint_Industrial:
            case CurrencyType.WorkerBlueprint_Modern:
            case CurrencyType.WorkerBlueprint_Futuristic:
                return CurrencyVFXTargetType.WorkerBlueprints;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(currencyType),
                    currencyType,
                    null
                );
        }
    }

    private RectTransform GetTargetIcon(CurrencyType currencyType)
    {
        CurrencyVFXTargetType targetType =
            GetTargetType(currencyType);

        foreach (CurrencyVFXTarget target in currencyTargets)
        {
            if (target.targetType == targetType)
                return target.targetIcon;
        }

        Debug.LogWarning(
            $"No VFX target found for: {targetType}"
        );

        return null;
    }

    private void PulseTarget(RectTransform targetIcon)
    {
        if (!targetOriginalScales.TryGetValue(targetIcon, out Vector3 originalScale))
            return;

        targetIcon.DOKill();
        targetIcon.localScale = originalScale;

        targetIcon
            .DOPunchScale(originalScale * 0.3f,0.2f,4,0.5f)
            .OnComplete(() =>
            {
                targetIcon.localScale = originalScale;
            });
    }
}

[Serializable]
public class CurrencyVFXTarget
{
    public CurrencyVFXTargetType targetType;
    public RectTransform targetIcon;
}