using DG.Tweening;
using UnityEngine;

public class CurrencyShopUI : MonoBehaviour
{
    public static CurrencyShopUI Instance;

    [SerializeField] private RectTransform currencyShopPanel;
    [SerializeField] private RectTransform currencyShopRoot;

    [SerializeField] private float animationTime = 0.1f;

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

    public void Open()
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

    public void Close()
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