using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private RectTransform itemView;
    [SerializeField] private RectTransform shopRoot;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private TabButton workerTab;

    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject bottomSpacerPrefab;

    private readonly List<ShopItemUI> shopItemUIs = new();

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

        shopPanel.gameObject.SetActive(false);
    }

    private void InitializeShopPanelPositions()
    {
        closedPosition = shopRoot.anchoredPosition;
        openedPosition = closedPosition + new Vector2(shopPanel.rect.width, 0);
    }

    #endregion

    public void CreateAndInitializeShopItems(Dictionary<ShopCategory, List<ShopItem>> shopItems)
    {
        for (int i = 0; i < shopItems.Keys.Count; i++)
        {
            Transform parent = tabGroup.objectsToSwap[i].transform;

            foreach (var item in shopItems[(ShopCategory)i])
            {
                ShopItemUI itemUI = Instantiate(itemPrefab, parent).GetComponent<ShopItemUI>();

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

    public ShopItemUI GetWorkerItem(BusinessType businessType)
    {
        foreach (ShopItemUI itemUI in shopItemUIs)
        {
            ShopItem item = itemUI.ShopItem;

            if (item.Type != ShopCategory.Workers)
                continue;

            if (item.BusinessType != businessType)
                continue;

            return itemUI;
        }

        Debug.LogWarning($"Worker shop item not found: {businessType}");
        return null;
    }

    public ShopItemUI GetBuildingItem(BusinessType businessType)
    {
        foreach (ShopItemUI itemUI in shopItemUIs)
        {
            ShopItem item = itemUI.ShopItem;

            if (item.Type != ShopCategory.Buildings)
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

    public void Open(Action onComplete = null)
    {
        if (isAnimating)
            return;

        if (opened)
        {
            onComplete?.Invoke();
            return;
        }

        shopPanel.gameObject.SetActive(true);

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

    public void Close()
    {
        if (!shopPanel.gameObject.activeSelf)
            return;

        shopRoot.DOKill();

        isAnimating = true;

        shopRoot
            .DOAnchorPos(closedPosition, animationTime)
            .OnComplete(() =>
            {
                isAnimating = false;
                opened = false;
                shopPanel.gameObject.SetActive(false);
            });
    }

    public void SelectTab(int index)
    {
        tabGroup.SelectTabByIndex(index);
    }

    public void UpdateShopItems()
    {
        foreach (var itemUI in shopItemUIs)
        {
            itemUI.UpdateItemState();
        }
    }
}