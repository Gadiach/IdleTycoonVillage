using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyShopUI : MonoBehaviour
{
    public static CurrencyShopUI Instance;

    [SerializeField] private RectTransform currencyShopPanel;
    [SerializeField] private RectTransform currencyShopRoot;

    [SerializeField] private CurrencyShopItemUI currencyItemHolderPrefab;
    [SerializeField] private TabGroup tabGroup;

    [SerializeField] private float animationTime = 0.1f;

    private readonly List<CurrencyShopItemUI> shopItemUIs = new();

    private Vector2 closedPosition;
    private Vector2 openedPosition;

    private bool isAnimating;
    private bool opened;

    public bool IsOpened => opened;

    private void Awake()
    {
        Instance = this;

        InitializePanelPositions();

        currencyShopPanel.gameObject.SetActive(false);
    }

    private void InitializePanelPositions()
    {
        closedPosition = currencyShopRoot.anchoredPosition;

        openedPosition =
            closedPosition -
            new Vector2(currencyShopPanel.rect.width, 0);
    }

    public void CreateAndInitializeShopItems(
        Dictionary<CurrencyShopCategory, List<CurrencyShopItem>> shopItems)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            Transform parent =
                tabGroup.objectsToSwap[i].transform;

            foreach (CurrencyShopItem item
                     in shopItems[(CurrencyShopCategory)i])
            {
                CurrencyShopItemUI itemUI =
                    Instantiate(currencyItemHolderPrefab, parent);

                shopItemUIs.Add(itemUI);

                itemUI.Initialize(item);
            }
        }
    }

    public void OpenCurrencyShop()
    {
        if (isAnimating || opened)
            return;

        currencyShopPanel.gameObject.SetActive(true);

        currencyShopRoot.DOKill();

        isAnimating = true;

        currencyShopRoot
            .DOAnchorPos(openedPosition, animationTime)
            .OnComplete(() =>
            {
                isAnimating = false;
                opened = true;
            });
    }

    public void CloseCurrencyShop()
    {
        if (!currencyShopPanel.gameObject.activeSelf)
            return;

        currencyShopRoot.DOKill();

        isAnimating = true;

        currencyShopRoot
            .DOAnchorPos(closedPosition, animationTime)
            .OnComplete(() =>
            {
                isAnimating = false;
                opened = false;

                currencyShopPanel.gameObject.SetActive(false);
            });
    }
}