using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private RectTransform shopPanel;
    [SerializeField] private RectTransform itemView;
    [SerializeField] private RectTransform shopRoot;

    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;
    [SerializeField] private GameObject itemPrefab;

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
        }
    }

    public RectTransform GetWorkerItemTransform(BusinessType businessType)
    {
        foreach (ShopItemUI itemUI in shopItemUIs)
        {
            ShopItem item = itemUI.ShopItem;

            if (item.Type != ShopCategory.Workers)
                continue;

            if (item.BusinessType != businessType)
                continue;

            return itemUI.transform as RectTransform;
        }

        Debug.LogWarning($"Worker shop item not found: {businessType}");

        return null;
    }

    public void Open()
    {
        if (opened || isAnimating)
            return;

        shopPanel.gameObject.SetActive(true);

        isAnimating = true;

        shopRoot
            .DOAnchorPos(openedPosition, animationTime)
            .OnComplete(() =>
            {
                isAnimating = false;
                opened = true;
            });
    }

    public void Close()
    {
        if (!opened || isAnimating)
            return;

        isAnimating = true;

        shopRoot.DOAnchorPos(closedPosition, animationTime)
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