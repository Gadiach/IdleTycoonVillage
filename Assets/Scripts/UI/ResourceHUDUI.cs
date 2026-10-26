using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceHUDUI : MonoBehaviour
{
    [Header("Notification Elements")]
    [SerializeField] private GameObject addCoinButtonNotificationCircleImg;
    [SerializeField] private GameObject adsButton;

    [System.Serializable]
    public class CurrencyTextBinding
    {
        public CurrencyType CurrencyType;
        public TextMeshProUGUI Text;
    }

    [SerializeField] private CurrencyTextBinding[] currencyBindings;

    private Dictionary<CurrencyType, TextMeshProUGUI> currencyTexts = new();


    private void OnEnable()
    {
        EventManager.Instance.AddListener<CurrencyAddedEvent>(OnCurrencyChanged);
        EventManager.Instance.AddListener<CurrencySpentEvent>(OnCurrencyChanged);

        EventManager.Instance.AddListener<CurrencyShopAvailabilityChangedEvent>(
            OnCurrencyShopAvailabilityChanged
        );

        EventManager.Instance.AddListener<CurrencyVFXIconArrivedEvent>(
            OnCurrencyVFXIconArrived
        );

        EventManager.Instance.AddListener<CurrencyShopRewardClaimedEvent>(
            OnCurrencyShopRewardClaimed
        );
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<CurrencyAddedEvent>(OnCurrencyChanged);
        EventManager.Instance.RemoveListener<CurrencySpentEvent>(OnCurrencyChanged);
        EventManager.Instance.RemoveListener<CurrencyShopAvailabilityChangedEvent>(
        OnCurrencyShopAvailabilityChanged
    );
        EventManager.Instance.RemoveListener<CurrencyVFXIconArrivedEvent>(
    OnCurrencyVFXIconArrived
);
        EventManager.Instance.RemoveListener<CurrencyShopRewardClaimedEvent>(
    OnCurrencyShopRewardClaimed
);
    }

    private void Awake()
    {
        InitializeCurrencyTextBindings();
    }

    private void Start()
    {
        UpdateAllCurrencies();
        UpdateCoinNotificator();
        UpdateAdsButton();
    }

    private void InitializeCurrencyTextBindings()
    {
        foreach (var binding in currencyBindings)
        {
            if (binding == null || binding.Text == null)
                continue;

            currencyTexts[binding.CurrencyType] = binding.Text;
        }
    }

    private void UpdateAllCurrencies()
    {
        foreach (var pair in currencyTexts)
        {
            pair.Value.text = CurrencySystem.GetCurrencyAmount(pair.Key).ToString();
        }
    }

    private void OnCurrencyShopRewardClaimed(
    CurrencyShopRewardClaimedEvent info)
    {
        if (info.CurrencyType != CurrencyType.Coins)
            return;

        if (!currencyTexts.TryGetValue(
            info.CurrencyType,
            out var text))
            return;

        int finalAmount =
            CurrencySystem.GetCurrencyAmount(info.CurrencyType);

        int startAmount =
            finalAmount - info.Amount;

        text.text = startAmount.ToString();
    }

    private void UpdateCoinNotificator()
    {
        bool hasFreeCoinReward = CurrencyShopSystem.Instance.HasAvailableFreeCoinReward();

        addCoinButtonNotificationCircleImg.SetActive(hasFreeCoinReward);
    }

    private void UpdateAdsButton()
    {
        bool hasAvailableAds =
            CurrencyShopSystem.Instance.HasAvailableAds();

        adsButton.SetActive(hasAvailableAds);
    }

    private void OnCurrencyShopAvailabilityChanged(
    CurrencyShopAvailabilityChangedEvent info)
    {
        UpdateCoinNotificator();
        UpdateAdsButton();
    }

    private void OnCurrencyVFXIconArrived(
    CurrencyVFXIconArrivedEvent info)
    {
        if (!currencyTexts.TryGetValue(
            info.CurrencyType,
            out var text))
            return;

        text.text = info.DisplayedAmount.ToString();
    }

    private void OnCurrencyChanged(CurrencyAddedEvent info)
    {
        UpdateCurrency(info.CurrencyType);
    }

    public void OnAddBlueprintButtonClicked()
    {
        UniversityUI.Instance.TryOpenUniversity();
    }

    private void OnCurrencyChanged(CurrencySpentEvent info)
    {
        UpdateCurrency(info.CurrencyType);
    }

    private void UpdateCurrency(CurrencyType currencyType)
    {
        if (!currencyTexts.TryGetValue(currencyType, out var text))
            return;

        text.text = CurrencySystem.GetCurrencyAmount(currencyType).ToString();
    }
}