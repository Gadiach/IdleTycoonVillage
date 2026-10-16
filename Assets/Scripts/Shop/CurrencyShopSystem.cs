using System.Collections.Generic;
using UnityEngine;

public class CurrencyShopSystem : MonoBehaviour
{
    public static CurrencyShopSystem Instance;

    [SerializeField] private CurrencyShopUI currencyShopUI;

    private const string currencyShopItemsPath = "CurrencyShopItems";

    private Dictionary<CurrencyShopCategory, List<CurrencyShopItem>> currencyShopItems = new();

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

    public void OpenShop()
    {
        currencyShopUI.OpenCurrencyShop();
    }

    public void CloseShop()
    {
        currencyShopUI.CloseCurrencyShop();
    }
}