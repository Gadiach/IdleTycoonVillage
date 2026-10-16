using System.Collections.Generic;
using UnityEngine;
using System;
public class BuildShopSystem : MonoBehaviour 
{
    public static BuildShopSystem Instance;

    [SerializeField] private BuildShopUI buildShopUI;

    private const string buildShopItemsPath = "BuildShopItems";

    private bool canPlayerCloseShop = true;

    private Dictionary<BuildShopCategory, List<BuildShopItem>> buildShopItems = new Dictionary<BuildShopCategory, List<BuildShopItem>>(capacity: 3);

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

        buildShopUI.CreateAndInitializeShopItems(buildShopItems);
    }

    private void InitializeCategories()
    {
        foreach (BuildShopCategory category in System.Enum.GetValues(typeof(BuildShopCategory)))
        {
            buildShopItems.Add(category, new List<BuildShopItem>());
        }
    }

    private void Load()
    {
        BuildShopItem[] items = Resources.LoadAll<BuildShopItem>(buildShopItemsPath);

        foreach (var item in items)
        {
            buildShopItems[item.Type].Add(item);
        }
    }

    private void OnCurrencyChanged(CurrencyAddedEvent info) 
    {
        buildShopUI.UpdateShopItems();
    } 
    private void OnCurrencyChanged(CurrencySpentEvent info) 
    {
        buildShopUI.UpdateShopItems();
    }

    public BuildShopItemUI GetWorkerItem(BusinessType businessType)
    {
        return buildShopUI.GetWorkerItem(businessType);
    }

    public BuildShopItemUI GetBuildingItem(BusinessType businessType)
    {
        return buildShopUI.GetBuildingItem(businessType);
    }

    public void ScrollUp(float amount)
    {
        buildShopUI.ScrollUp(amount);
    }

    public void ShopButton_Click()
    {
        if (buildShopUI.IsOpened)
        {
            TryCloseShop();
        }
        else
        {
            OpenShop(BuildShopCategory.Buildings);
        }
    }

    public void TryCloseShop()
    {
        if (!canPlayerCloseShop)
            return;

        CloseShop();
    }

    public void OpenShop(BuildShopCategory category, Action onComplete = null)
    {
        buildShopUI.UpdateAvailableTabs();
        buildShopUI.UpdateShopItems();
        buildShopUI.SelectTab((int)category);
        buildShopUI.Open(onComplete);
    }

    public void SetPlayerCloseEnabled(bool enabled)
    {
        canPlayerCloseShop = enabled;
    }

    public void CloseShop()
    {
        buildShopUI.Close();
    }
}