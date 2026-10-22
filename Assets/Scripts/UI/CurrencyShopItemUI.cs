using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyShopItemUI : MonoBehaviour
{
    [SerializeField] private Image rewardIconImage;
    [SerializeField] private TextMeshProUGUI purchaseText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button purchaseButton;

    [SerializeField] private float timerFontSize = 12f;

    private float defaultPurchaseFontSize;

    private CurrencyShopItem item;

    public CurrencyShopItem ShopItem => item;

    public void Initialize(CurrencyShopItem item)
    {
        this.item = item;

        defaultPurchaseFontSize = purchaseText.fontSize;

        rewardIconImage.sprite = item.Icon;

        bool isAd = item.Category == CurrencyShopCategory.Ads;

        rewardText.gameObject.SetActive(!isAd);

        if (!isAd)
            rewardText.text = item.Reward.ToString();

        UpdateItemState();
    }

    public void UpdateItemState()
    {
        if (item.Category == CurrencyShopCategory.Ads)
        {
            UpdateAdState();
            return;
        }

        if (item.Category == CurrencyShopCategory.Gold)
        {
            UpdateGoldState();
        }
    }

    private void UpdateAdState()
    {
        bool canWatchAd = CurrencyShopSystem.Instance.CanWatchAd(item);

        purchaseButton.interactable = canWatchAd;

        if (canWatchAd)
        {
            purchaseText.text = "Watch";
            purchaseText.fontSize = defaultPurchaseFontSize;
        }
        else
        {
            purchaseText.text = "11h23m02s";
            purchaseText.fontSize = timerFontSize;
        }
    }

    private void UpdateGoldState()
    {
        bool canClaimFreeReward =
            CurrencyShopSystem.Instance.CanClaimFreeReward(item);

        purchaseText.fontSize = defaultPurchaseFontSize;

        if (canClaimFreeReward)
        {
            purchaseText.text = "Free";
        }
        else
        {
            purchaseText.text = $"ˆ{item.Price}";
        }
    }

    public void OnBuyButtonClicked()
    {
        CurrencyShopSystem.Instance.Purchase(item);
    }
}