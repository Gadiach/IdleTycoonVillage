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

    public void Purchase(CurrencyShopItem item)
    {
        switch (item.Category)
        {
            case CurrencyShopCategory.Ads:
                WatchAd(item);
                break;

            case CurrencyShopCategory.Gold:
                PurchaseGold(item);
                break;
        }
    }

    private void WatchAd(CurrencyShopItem item)
    {
        if (!CanWatchAd(item))
            return;

        // Пізніше тут запускаємо rewarded ad.

        // ТИМЧАСОВО:
        OnAdCompleted(item);
    }

    private void OnAdCompleted(CurrencyShopItem item)
    {
        if (!dailyAdViews.ContainsKey(item))
            dailyAdViews[item] = 0;

        dailyAdViews[item]++;

        GiveReward(item);

        currencyShopUI.UpdateCurrencyShopItem(item);
    }

    private void PurchaseGold(CurrencyShopItem item)
    {
        if (CanClaimFreeReward(item))
        {
            ClaimFreeReward(item);
            return;
        }

        // Тут пізніше буде IAP.
        // Після успішної покупки:
        GiveReward(item);
    }

    private void ClaimFreeReward(CurrencyShopItem item)
    {
        if (!freeRewardsClaimed.ContainsKey(item))
            freeRewardsClaimed[item] = 0;

        freeRewardsClaimed[item]++;

        GiveReward(item);

        currencyShopUI.UpdateCurrencyShopItem(item);
    }

    private void GiveReward(CurrencyShopItem item)
    {
        switch (item.RewardType)
        {
            case CurrencyShopRewardType.Gold : GiveGold(item.Reward);
                break;

            case CurrencyShopRewardType.RandomBlueprint : GiveRandomBlueprint();
                break;
        }
    }

    private void GiveGold(int amount)
    {
        CurrencySystem.Instance.AddCurrency(CurrencyType.Coins,amount);
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

        currencyShopUI.OpenCurrencyShop(CurrencyShopCategory.Gold);
    }

    private void GiveRandomBlueprint()
    {
        CurrencyType blueprint = GetRandomBlueprint();

        CurrencySystem.Instance.AddCurrency(blueprint,1);
    }

    private CurrencyType GetRandomBlueprint()
    {
        float totalWeight = 0f;

        foreach (BlueprintRewardChance reward in blueprintRewardConfig.rewards)
        {
            totalWeight += reward.weight;
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