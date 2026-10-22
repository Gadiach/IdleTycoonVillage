using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildShopUI : MonoBehaviour
{
    public static BuildShopUI Instance;

    [SerializeField] private RectTransform buildShopPanel;
    [SerializeField] private RectTransform itemView;
    [SerializeField] private RectTransform shopRoot;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private TabButton workerTab;

    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;
    [SerializeField] private GameObject buildShopItemPrefab;
    [SerializeField] private GameObject bottomSpacerPrefab;

    private readonly List<BuildShopItemUI> shopItemUIs = new();

    private Vector2 closedPosition;
    private Vector2 openedPosition;

    [SerializeField] private TabGroup tabGroup;
    [SerializeField] private float animationTime = 0.1f;

    private bool isAnimating;
    private bool opened;

    public bool IsOpened => opened;

    #region Initialization

    private void Awake()
    {
        InitializeShopPanelPositions();

        buildShopPanel.gameObject.SetActive(false);
    }

    private void InitializeShopPanelPositions()
    {
        closedPosition = shopRoot.anchoredPosition;
        openedPosition = closedPosition + new Vector2(buildShopPanel.rect.width, 0);
    }

    #endregion

    public void CreateAndInitializeShopItems(Dictionary<BuildShopCategory, List<BuildShopItem>> shopItems)
    {
        for (int i = 0; i < shopItems.Keys.Count; i++)
        {
            Transform parent = tabGroup.objectsToSwap[i].transform;

            foreach (var item in shopItems[(BuildShopCategory)i])
            {
                BuildShopItemUI itemUI = Instantiate(buildShopItemPrefab, parent).GetComponent<BuildShopItemUI>();

                shopItemUIs.Add(itemUI);

                Sprite currencyIcon = currencyIconDatabase.GetIcon(item.Currency);

                itemUI.Initialize(item, currencyIcon, itemView);
            }
            Instantiate(bottomSpacerPrefab, parent);
        }
    }

    public void ScrollUp(float amount)
    {
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - amount);
    }

    public BuildShopItemUI GetWorkerItem(BusinessType businessType)
    {
        foreach (BuildShopItemUI itemUI in shopItemUIs)
        {
            BuildShopItem item = itemUI.ShopItem;

            if (item.Type != BuildShopCategory.Workers)
                continue;

            if (item.BusinessType != businessType)
                continue;

            return itemUI;
        }

        Debug.LogWarning($"Worker shop item not found: {businessType}");
        return null;
    }

    public BuildShopItemUI GetBuildingItem(BusinessType businessType)
    {
        foreach (BuildShopItemUI itemUI in shopItemUIs)
        {
            BuildShopItem item = itemUI.ShopItem;

            if (item.Type != BuildShopCategory.Buildings)
            {
                continue;
            }

            if (item.BusinessType != businessType)
            {
                continue;
            }

            return itemUI;
        }

        return null;
    }

    public void UpdateAvailableTabs()
    {
        if (TutorialSystem.Instance == null)
        {
            workerTab.SetInteractable(true);
            return;
        }

        workerTab.SetInteractable(
            TutorialSystem.Instance.CanShowWorkerShopTab()
        );
    }

    public void OpenBuildShop(Action onComplete = null)
    {
        if (isAnimating)
            return;

        if (opened)
        {
            onComplete?.Invoke();
            return;
        }

        buildShopPanel.gameObject.SetActive(true);

        isAnimating = true;

        shopRoot
            .DOAnchorPos(openedPosition, animationTime)
            .OnComplete(() =>
            {
                isAnimating = false;
                opened = true;

                Canvas.ForceUpdateCanvases();

                onComplete?.Invoke();
            });
    }

    public void CloseBuildShop()
    {
        if (!buildShopPanel.gameObject.activeSelf)
            return;

        shopRoot.DOKill();

        isAnimating = true;

        shopRoot
            .DOAnchorPos(closedPosition, animationTime)
            .OnComplete(() =>
            {
                isAnimating = false;
                opened = false;
                buildShopPanel.gameObject.SetActive(false);
            });
    }

    public void SelectBuildShopTab(BuildShopCategory category)
    {
        tabGroup.SelectTabByIndex((int)category);
    }

    public void UpdateShopItems()
    {
        foreach (var itemUI in shopItemUIs)
        {
            itemUI.UpdateItemState();
        }
    }
}