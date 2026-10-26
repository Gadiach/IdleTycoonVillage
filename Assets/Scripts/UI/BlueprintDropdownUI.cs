using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintDropdownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform dropdownPanel;
    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;

    [Header("Blueprints")]
    [SerializeField] private BlueprintIconBinding[] blueprintBindings;

    [Header("Animation")]
    [SerializeField] private float animationTime = 0.15f;

    private Vector2 openedPosition;
    private Vector2 closedPosition;

    private bool isOpened;
    private bool isAnimating;

    private void Awake()
    {
        InitializePositions();
        InitializeBlueprintIcons();

        dropdownPanel.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildShopOpenedEvent>(
            OnBuildShopOpened
        );
        EventManager.Instance.AddListener<MapTappedEvent>(
       OnMapTapped
   );

        EventManager.Instance.AddListener<CurrencyShopOpenedEvent>(
       OnCurrencyShopOpened
   );
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<BuildShopOpenedEvent>(
            OnBuildShopOpened
        );
        EventManager.Instance.RemoveListener<MapTappedEvent>(
      OnMapTapped
  );

        EventManager.Instance.RemoveListener<CurrencyShopOpenedEvent>(
        OnCurrencyShopOpened
    );
    }

    private void InitializePositions()
    {
        openedPosition = dropdownPanel.anchoredPosition;

        closedPosition = openedPosition + new Vector2(
            0f,
            dropdownPanel.rect.height
        );

        dropdownPanel.anchoredPosition = closedPosition;
    }

    private void InitializeBlueprintIcons()
    {
        foreach (BlueprintIconBinding binding in blueprintBindings)
        {
            if (binding.Icon == null)
                continue;

            binding.Icon.sprite =
                currencyIconDatabase.GetIcon(binding.CurrencyType);
        }
    }

    private void OnBuildShopOpened(BuildShopOpenedEvent info)
    {
        CloseDropdown();
    }

    private void OnMapTapped(MapTappedEvent info)
    {
        TryCloseDropdown();
    }

    private void OnCurrencyShopOpened(CurrencyShopOpenedEvent info)
    {
        TryCloseDropdown();
    }

    private void UpdateBlueprintAmounts()
    {
        foreach (BlueprintIconBinding binding in blueprintBindings)
        {
            if (binding.AmountText == null)
                continue;

            int amount =
                CurrencySystem.GetCurrencyAmount(binding.CurrencyType);

            binding.AmountText.text = amount.ToString();
        }
    }

    public void ToggleDropdown()
    {
        if (isAnimating)
            return;

        if (isOpened)
            CloseDropdown();
        else
            OpenDropdown();
    }

    private void OpenDropdown()
    {
        UpdateBlueprintAmounts();

        EventManager.Instance.QueueEvent(
            new BlueprintDropdownOpenedEvent()
        );

        dropdownPanel.gameObject.SetActive(true);

        dropdownPanel.DOKill();
        dropdownPanel.anchoredPosition = closedPosition;

        isAnimating = true;

        dropdownPanel
            .DOAnchorPos(openedPosition, animationTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                isAnimating = false;
                isOpened = true;
            });
    }

    private void CloseDropdown()
    {
        dropdownPanel.DOKill();

        isAnimating = true;

        dropdownPanel
            .DOAnchorPos(closedPosition, animationTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                isAnimating = false;
                isOpened = false;

                dropdownPanel.gameObject.SetActive(false);
            });
    }

    public void TryCloseDropdown()
    {
        if (!isOpened || isAnimating)
            return;

        CloseDropdown();
    }
}



[Serializable]
public class BlueprintIconBinding
{
    public CurrencyType CurrencyType;
    public Image Icon;
    public TextMeshProUGUI AmountText;
}