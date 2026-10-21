using System.Collections.Generic;
using UnityEngine;

public class CurrencyShopSystem : MonoBehaviour
{
    public static CurrencyShopSystem Instance;

    [SerializeField] private CurrencyShopUI currencyShopUI;

    private const string currencyShopItemsPath = "CurrencyShopItems";

    private Dictionary<CurrencyShopCategory, List<CurrencyShopItem>> currencyShopItems = new();

    [SerializeField] private BlueprintRewardConfig blueprintRewardConfig;

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
        // Тут пізніше викликаємо рекламну систему.

        // ТИМЧАСОВО:
        OnAdCompleted(item);
    }

    private void OnAdCompleted(CurrencyShopItem item)
    {
        GiveReward(item);
    }

    private void PurchaseGold(CurrencyShopItem item)
    {
        // Тут пізніше буде IAP.
        // Після успішної покупки:

        GiveReward(item);
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