using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyShopItemUI : MonoBehaviour
{
    [SerializeField] private Image rewardIconImage;
    [SerializeField] private TextMeshProUGUI purchaseText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button purchaseButton;

    private CurrencyShopItem item;

    public void Initialize(CurrencyShopItem item)
    {
        this.item = item;

        rewardIconImage.sprite = item.Icon;

        bool isAd = item.Category == CurrencyShopCategory.Ads;

        rewardText.gameObject.SetActive(!isAd);

        if (isAd)
        {
            purchaseText.text = "Watch";
        }
        else
        {
            purchaseText.text = $"ˆ{item.Price}";
            rewardText.text = item.Reward.ToString();
        }
    }

    public void OnBuyButtonClicked()
    {
        // Later:
        // IAPSystem.Instance.Purchase(item);
        CurrencyShopSystem.Instance.Purchase(item);
    }
}