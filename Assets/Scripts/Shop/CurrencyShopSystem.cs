using UnityEngine;

public class CurrencyShopSystem : MonoBehaviour
{
    public static CurrencyShopSystem Instance;

    [SerializeField] private CurrencyShopUI shopUI;

    private const string ShopItemsPath = "CurrencyShopItems";

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
        Load();
    }

    private void Load()
    {
        CurrencyShopItem[] items = Resources.LoadAll<CurrencyShopItem>(ShopItemsPath);

        shopUI.CreateAndInitializeShopItems(items);
    }

    public void OpenShop()
    {
        shopUI.Open();
    }

    public void CloseShop()
    {
        shopUI.Close();
    }
}