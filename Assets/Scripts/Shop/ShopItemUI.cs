using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    private ShopItem shopItem;
    public ShopItem ShopItem => shopItem;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image currencyImage;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private ShopItemDrag dragComponent;

    public RectTransform ItemIcon => iconImage.rectTransform;
    public Sprite ItemIconSprite => iconImage.sprite;

    [SerializeField] private RectTransform iconAndArrow;

    public RectTransform IconAndArrow => iconAndArrow;

    #region Initialization
    public void Initialize(ShopItem item, Sprite currencyIcon, RectTransform itemView)
    {
        shopItem = item;

        dragComponent.Initialize(item, itemView);

        InitializeUI(currencyIcon);

        UpdateItemState();
    }
    private void InitializeUI(Sprite currencyIcon)
    {
        InitializeIcon();
        InitializeTitle();
        InitializeCurrencyImage(currencyIcon);
        InitializePrice();
    }

    private void InitializeIcon()
    {
        iconImage.sprite = shopItem.Icon;
    }

    private void InitializeTitle()
    {
        titleText.text = shopItem.Name;
    }

    private void InitializeCurrencyImage(Sprite currencyImg)
    {
        currencyImage.sprite = currencyImg;
    }

    private void InitializePrice()
    {
        priceText.text = shopItem.PurchasePrice.ToString();
    }
    #endregion


    private void UpdatePriceColor(bool canAfford)
    {
        priceText.color = canAfford ? Color.white : Color.red;
    }

    public void UpdateItemState()
    {
        bool canAfford = CurrencySystem.Instance.HasEnoughCurrency(shopItem.Currency,shopItem.PurchasePrice);

        UpdatePriceColor(canAfford);
    }

    public void UpdateDragState(bool enabled)
    {
        dragComponent.enabled = enabled;
    }
}
