using System.Collections.Generic;
using UnityEngine;
using System;
public class ShopSystem : MonoBehaviour 
{
    public static ShopSystem Instance;

    [SerializeField] private ShopUI shopUI;

    private const string ShopItemsPath = "ShopItems";

    private bool canPlayerCloseShop = true;

    private Dictionary<ShopCategory, List<ShopItem>> shopItems = new Dictionary<ShopCategory, List<ShopItem>>(capacity: 3);

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
    
    private void OnEnable() 
    { 
        EventManager.Instance.AddListener<CurrencyAddedEvent>(OnCurrencyChanged); 
        EventManager.Instance.AddListener<CurrencySpentEvent>(OnCurrencyChanged); 
    } 
    
    private void OnDisable() 
    { 
        if (EventManager.Instance == null) return; 
        EventManager.Instance.RemoveListener<CurrencyAddedEvent>(OnCurrencyChanged); 
        EventManager.Instance.RemoveListener<CurrencySpentEvent>(OnCurrencyChanged); 
    } 
   
    private void Start() 
    {
        InitializeCategories();

        Load();

        shopUI.CreateAndInitializeShopItems(shopItems);
    }

    private void InitializeCategories()
    {
        foreach (ShopCategory category in System.Enum.GetValues(typeof(ShopCategory)))
        {
            shopItems.Add(category, new List<ShopItem>());
        }
    }

    private void Load()
    {
        ShopItem[] items = Resources.LoadAll<ShopItem>(ShopItemsPath);

        foreach (var item in items)
        {
            shopItems[item.Type].Add(item);
        }
    }

    private void OnCurrencyChanged(CurrencyAddedEvent info) 
    {
        shopUI.UpdateShopItems();
    } 
    private void OnCurrencyChanged(CurrencySpentEvent info) 
    {
        shopUI.UpdateShopItems();
    }

    public ShopItemUI GetWorkerItem(BusinessType businessType)
    {
        return shopUI.GetWorkerItem(businessType);
    }

    public ShopItemUI GetBuildingItem(BusinessType businessType)
    {
        return shopUI.GetBuildingItem(businessType);
    }

    public void ShopButton_Click()
    {
        if (shopUI.IsOpened)
        {
            if (!canPlayerCloseShop)
                return;

            CloseShop();
        }
        else
        {
            OpenShop(ShopCategory.Buildings);
        }
    }

    public void OpenShop(ShopCategory category, Action onComplete = null)
    {
        shopUI.UpdateShopItems();
        shopUI.SelectTab((int)category);
        shopUI.Open(onComplete);
    }

    public void SetPlayerCloseEnabled(bool enabled)
    {
        canPlayerCloseShop = enabled;
    }

    public void CloseShop()
    {
        shopUI.Close();
    }
}