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

        EventManager.Instance.AddListener<RequestCurrencyChangeEvent>(HandleCurrencyChangeRequest);
        EventManager.Instance.AddListener<NotEnoughCurrencyEvent>(OnNotEnough);
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

    public static int GetCurrencyAmount(CurrencyType currencyType)
    {
        if (currencyAmounts.ContainsKey(currencyType))
        {
            return currencyAmounts[currencyType];
        }
        return 0;
    }

    private void HandleCurrencyChangeRequest(RequestCurrencyChangeEvent info)
    {
        if (info.amount < 0)
        {
            if (!currencyAmounts.ContainsKey(info.currencyType) ||
                currencyAmounts[info.currencyType] < Mathf.Abs(info.amount))
            {
                EventManager.Instance.QueueEvent(
                    new NotEnoughCurrencyEvent(info.amount, info.currencyType)
                );
                return;
            }

            EventManager.Instance.QueueEvent(new EnoughCurrencyEvent());
        }

        AddCurrency(info.currencyType, info.amount);

        if (currencyTexts.TryGetValue(info.currencyType, out var text))
        {
            text.text = currencyAmounts[info.currencyType].ToString();
        }

        UpdateUI();
        ShopManager.current.UpdateShopItems();

        EventManager.Instance.QueueEvent(new CurrencyChangedEvent(info.currencyType));
    }


    private void AddCurrency(CurrencyType currencyType, int amount)
    {
        currencyAmounts[currencyType] += amount;
    }

    private void SubtractCurrency(CurrencyType currencyType, int amount)
    {
        currencyAmounts[currencyType] -= amount;
    }

    public bool IsEnoughMoneyForUpgrade(CurrencyType currencyType, int amount)
    {
        if (currencyAmounts.ContainsKey(currencyType) && currencyAmounts[currencyType] >= amount)
        {
            return true;
        }
        return false;
    }

    public bool TrySpendCurrency(CurrencyType currencyType, int amount)
    {
        if (!currencyAmounts.ContainsKey(currencyType))
            return false;

        if (currencyAmounts[currencyType] < amount)
            return false;

        SubtractCurrency (currencyType, amount);

        if (currencyTexts.TryGetValue(currencyType, out var text))
        {
            text.text = currencyAmounts[currencyType].ToString();
        }
        EventManager.Instance.QueueEvent(new CurrencyChangedEvent(currencyType));
        UpdateUI();
        return true;
    }

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

