using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ShopSystem : MonoBehaviour 
{
    public static ShopSystem Instance;

    [SerializeField] private ShopUI shopUI;

    private const string ShopItemsPath = "ShopItems";

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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShopButton_Click();
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

    public void ShopButton_Click()
    {
        if (shopUI.IsOpened)
            CloseShop();
        else
            OpenShop(ShopCategory.Buildings);

        Debug.Log("Button clicked");
    }

    public void OpenShop(ShopCategory category)
    {
        shopUI.UpdateShopItems();
        shopUI.Open();
        shopUI.SelectTab((int)category);
    }

    public void CloseShop()
    {
        shopUI.Close();
    }

    private bool dragging; 
    public void OnBeginDrag() { dragging = true; }
    public void OnEndDrag() { dragging = false; }
    public void OnPointerClick() 
    { 
        if (!dragging) 
        { 
            ShopButton_Click(); 
        } 
    }
}