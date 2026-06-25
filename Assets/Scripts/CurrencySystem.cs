using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;

    private static Dictionary<CurrencyType, int> currencyAmounts = new Dictionary<CurrencyType, int>();

    private Dictionary<CurrencyType, TextMeshProUGUI> currencyTexts = new();

    [System.Serializable]
    public class CurrencyTextBinding
    {
        public CurrencyType CurrencyType;
        public TextMeshProUGUI Text;
    }

    [SerializeField] private CurrencyTextBinding[] currencyBindings;

    [SerializeField] private CurrencyStartConfig startConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            currencyAmounts[type] = 0;
        }

        foreach (var binding in currencyBindings)
        {
            if (binding == null || binding.Text == null)
                continue;

            currencyTexts[binding.CurrencyType] = binding.Text;
        }

        ApplyStartConfig();
    }
    private void Start()
    {   
        UpdateUI();
    }

    private void ApplyStartConfig()
    {
        if (startConfig == null)
        {
            Debug.LogWarning("CurrencyStartConfig not assigned!");
            return;
        }

        ApplyEntries(startConfig.currencies);
        ApplyEntries(startConfig.buildingBlueprints);
        ApplyEntries(startConfig.workerBlueprints);
    }

    private void ApplyEntries(CurrencyAmount[] entries)
    {
        foreach (var entry in entries)
        {
            currencyAmounts[entry.currencyType] = entry.amount;

        }
    }

    #region Get Data

    public static int GetCurrencyAmount(CurrencyType currencyType)
    {
        if (currencyAmounts.ContainsKey(currencyType))
        {
            return currencyAmounts[currencyType];
        }
        return 0;
    }

    public bool HasEnoughCurrency(CurrencyType currencyType, int amount)
    {
        return currencyAmounts[currencyType] >= amount;
    }

    #endregion

    #region Spend Currency Logic

    public bool SpendCurrency(CurrencyType currencyType, int amount)
    {
        if (!HasEnoughCurrency(currencyType, amount))
            return false;
        

        currencyAmounts[currencyType] -= amount;

        UpdateUI();

        ShopManager.current.UpdateShopItems();

        EventManager.Instance.QueueEvent(new CurrencySpentEvent(currencyType, amount));

        return true;
    }

    #endregion



    #region Add Currency Logic

    public void AddCurrency(CurrencyType currencyType, int amount)
    {
        currencyAmounts[currencyType] += amount;

        UpdateUI();

        ShopManager.current.UpdateShopItems();

        EventManager.Instance.QueueEvent(new CurrencyAddedEvent(currencyType, amount));
    }

    #endregion

    private void OnNotEnough(NotEnoughCurrencyEvent info)
    {
        Debug.Log(message: $"You don't have enough of {info.amount} {info.currencyType}");
    }

    private void UpdateUI()
    {
        foreach (var pair in currencyTexts)
        {
            pair.Value.text = currencyAmounts[pair.Key].ToString();
        }
    }
}

public enum CurrencyType
{
    Coins,
    Crystals,

    BuildingBlueprint_Primitive,
    BuildingBlueprint_Developed,
    BuildingBlueprint_Industrial,
    BuildingBlueprint_Modern,
    BuildingBlueprint_Futuristic,

    WorkerBlueprint_Primitive,
    WorkerBlueprint_Developed,
    WorkerBlueprint_Industrial,
    WorkerBlueprint_Modern,
    WorkerBlueprint_Futuristic
}

