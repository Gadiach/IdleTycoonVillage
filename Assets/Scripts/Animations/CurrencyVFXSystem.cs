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
        EventManager.Instance.AddListener<CurrencyClaimedEvent>(OnCurrencyClaimed);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<CurrencyClaimedEvent>(OnCurrencyClaimed);
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

    private void OnCurrencyClaimed(CurrencyClaimedEvent info)
    {
        Sprite currencyIcon = currencyIconDatabase.GetIcon(info.CurrencyType);

        RectTransform targetIcon = GetTargetIcon(info.CurrencyType);

        if (currencyIcon == null || targetIcon == null)
            return;

        Play(info.SourcePosition,currencyIcon,targetIcon);
    }

    private void Play(Vector3 startPosition,Sprite currencyIcon,RectTransform targetIcon)
    {
        for (int i = 0; i < flyingIconCount; i++)
        {
            SpawnCurrencyIcon(startPosition,currencyIcon,targetIcon,i);
        }
    }

    private void SpawnCurrencyIcon(Vector3 startPosition,Sprite currencyIcon,RectTransform targetIcon,int index)
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
            Destroy(currencyObject);
        });
    }

    private RectTransform GetTargetIcon(CurrencyType currencyType)
    {
        foreach (CurrencyVFXTarget target in currencyTargets)
        {
            if (target.currencyType == currencyType)
                return target.targetIcon;
        }

        Debug.LogWarning($"No VFX target found for currency: {currencyType}");

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
    public CurrencyType currencyType;
    public RectTransform targetIcon;
}