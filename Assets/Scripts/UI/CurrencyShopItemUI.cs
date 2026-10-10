using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyShopItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;

    private CurrencyShopItem item;

    public void Initialize(CurrencyShopItem item)
    {
        this.item = item;

        iconImage.sprite = item.Icon;
        amountText.text = item.Amount.ToString();
        priceText.text = item.PriceText;
    }

    public void OnBuyButtonClicked()
    {
        // Later:
        // IAPSystem.Instance.Purchase(item);
    }
}