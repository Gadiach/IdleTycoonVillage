using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyShopItemUI : MonoBehaviour
{
    [SerializeField] private Image rewardIconImage;
    [SerializeField] private TextMeshProUGUI purchasePriceText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button purchaseButton;

    public void Initialize(CurrencyShopItem item)
    {
        rewardIconImage.sprite = item.Icon;
        purchasePriceText.text = item.Price.ToString();
        rewardText.text = item.Reward.ToString();
    }

    public void OnBuyButtonClicked()
    {
        // Later:
        // IAPSystem.Instance.Purchase(item);
    }
}