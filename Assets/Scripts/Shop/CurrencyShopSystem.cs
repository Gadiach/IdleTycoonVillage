using System.Collections.Generic;
using UnityEngine;

public class CurrencyShopSystem : MonoBehaviour
{
    public static CurrencyShopSystem Instance;

    [SerializeField] private CurrencyShopUI currencyShopUI;

    private const string currencyShopItemsPath = "CurrencyShopItems";

    private Dictionary<CurrencyShopCategory, List<CurrencyShopItem>> currencyShopItems = new();

    [SerializeField] private BlueprintRewardConfig blueprintRewardConfig;

    private Dictionary<CurrencyShopItem, int> dailyAdViews = new();

    private Dictionary<CurrencyShopItem, int> freeRewardsClaimed = new();

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        InitializeCategories();

        Load();

        currencyShopUI.CreateAndInitializeShopItems(currencyShopItems);
    }

    private void InitializeCategories()
    {
        foreach (CurrencyShopCategory category in System.Enum.GetValues(typeof(CurrencyShopCategory)))
        {
            currencyShopItems.Add(category,new List<CurrencyShopItem>());
        }
    }

    private void Load()
    {
        CurrencyShopItem[] items = Resources.LoadAll<CurrencyShopItem>(currencyShopItemsPath);

        foreach (CurrencyShopItem item in items)
        {
            currencyShopItems[item.Category].Add(item);
        }

        foreach (List<CurrencyShopItem> itemsList in currencyShopItems.Values)
        {
            itemsList.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
    }

    public int GetRemainingFreeRewards(CurrencyShopItem item)
    {
        if (!freeRewardsClaimed.TryGetValue(item, out int claimed))
            claimed = 0;

        return Mathf.Max(0, item.FreeRewardLimit - claimed);
    }

    public bool CanClaimFreeReward(CurrencyShopItem item)
    {
        return GetRemainingFreeRewards(item) > 0;
    }

    public int GetRemainingAdViews(CurrencyShopItem item)
    {
        if (!dailyAdViews.TryGetValue(item, out int views))
            views = 0;

        return Mathf.Max(0, item.DailyAdLimit - views);
    }

    public bool CanWatchAd(CurrencyShopItem item)
    {
        return GetRemainingAdViews(item) > 0;
    }

    public void Purchase(CurrencyShopItem item, Vector3 sourcePosition)
    {
        switch (item.Category)
        {
            case CurrencyShopCategory.Ads:
                WatchAd(item, sourcePosition);
                break;

            case CurrencyShopCategory.Coin:
                PurchaseCoin(item, sourcePosition);
                break;
        }
    }

    private void WatchAd(CurrencyShopItem item, Vector3 sourcePosition)
    {
        if (!CanWatchAd(item))
            return;

        OpenRewardedAd(item, sourcePosition);
    }

    private void OpenRewardedAd(CurrencyShopItem item, Vector3 sourcePosition)
    {
        Debug.Log(
            $"Rewarded ad opened for {item.name}. " +
            "Ad SDK will be implemented later."
        );

        // TEMPORARY: simulate successful ad completion
        OnAdCompleted(item, sourcePosition);
    }

    private void OnAdCompleted(CurrencyShopItem item, Vector3 sourcePosition)
    {
        if (!dailyAdViews.ContainsKey(item))
            dailyAdViews[item] = 0;

        dailyAdViews[item]++;

        GiveReward(item, sourcePosition);

        currencyShopUI.UpdateCurrencyShopItem(item);

        EventManager.Instance.QueueEvent(
        new CurrencyShopAvailabilityChangedEvent()
    );
    }

    private void PurchaseCoin(CurrencyShopItem item, Vector3 sourcePosition)
    {
        if (CanClaimFreeReward(item))
        {
            ClaimFreeReward(item, sourcePosition);
            return;
        }

        OpenPurchasePanel(item);
    }

    private void OpenPurchasePanel(CurrencyShopItem item)
    {
        Debug.Log($"Purchase panel opened for {item.name}. " + $"Price: ˆ{item.Price}. IAP will be implemented later.");
    }

    private void ClaimFreeReward(
    CurrencyShopItem item,
    Vector3 sourcePosition)
    {
        if (!freeRewardsClaimed.ContainsKey(item))
            freeRewardsClaimed[item] = 0;

        freeRewardsClaimed[item]++;

        GiveReward(item, sourcePosition);

        currencyShopUI.UpdateCurrencyShopItem(item);

        EventManager.Instance.QueueEvent(
        new CurrencyShopAvailabilityChangedEvent()
    );
    }

    private void GiveReward(
     CurrencyShopItem item,
     Vector3 sourcePosition)
    {
        switch (item.RewardType)
        {
            case CurrencyShopRewardType.Coin:
                GiveCoin(item.Reward, sourcePosition);
                break;

            case CurrencyShopRewardType.RandomBlueprint:
                GiveRandomBlueprint(
                    item.Reward,
                    sourcePosition
                );
                break;
        }
    }

    private void GiveCoin(int amount, Vector3 sourcePosition)
    {
        CurrencySystem.Instance.AddCurrency(
            CurrencyType.Coins,
            amount
        );

        EventManager.Instance.QueueEvent(
            new CurrencyShopRewardClaimedEvent(
                CurrencyType.Coins,
                amount,
                sourcePosition
            )
        );
    }

    public bool HasAvailableFreeCoinReward()
    {
        foreach (CurrencyShopItem item in currencyShopItems[CurrencyShopCategory.Coin])
        {
            if (CanClaimFreeReward(item))
                return true;
        }

        return false;
    }

    public bool HasAvailableAds()
    {
        foreach (CurrencyShopItem item in currencyShopItems[CurrencyShopCategory.Ads])
        {
            if (CanWatchAd(item))
                return true;
        }

        return false;
    }

    public void OpenAdsTab()
    {
        if (currencyShopUI.IsOpened)
        {
            currencyShopUI.CloseCurrencyShop();
            return;
        }

        currencyShopUI.OpenCurrencyShop(CurrencyShopCategory.Ads);
    }

    public void OpenGoldTab()
    {
        if (currencyShopUI.IsOpened)
        {
            currencyShopUI.CloseCurrencyShop();
            return;
        }

        currencyShopUI.OpenCurrencyShop(CurrencyShopCategory.Coin);
    }

    private void GiveRandomBlueprint(
    int amount,
    Vector3 sourcePosition)
    {
        CurrencyType blueprint = GetRandomBlueprint();

        Debug.Log($"RANDOM BLUEPRINT: {blueprint}");

        CurrencySystem.Instance.AddCurrency(
            blueprint,
            amount
        );

        EventManager.Instance.QueueEvent(
            new CurrencyShopRewardClaimedEvent(
                blueprint,
                amount,
                sourcePosition
            )
        );
    }

    private CurrencyType GetRandomBlueprint()
    {
        float totalWeight = 0f;

        foreach (BlueprintRewardChance reward in blueprintRewardConfig.rewards)
        {
            totalWeight += reward.weight;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogError("BlueprintRewardConfig has no valid reward weights.");

            return blueprintRewardConfig.rewards[0].blueprintType;
        }

        float randomValue = Random.Range(0f, totalWeight);

        foreach (BlueprintRewardChance reward in blueprintRewardConfig.rewards)
        {
            if (randomValue < reward.weight)
                return reward.blueprintType;

            randomValue -= reward.weight;
        }

        return blueprintRewardConfig.rewards[0].blueprintType;
    }
}