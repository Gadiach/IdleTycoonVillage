using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;

    private static Dictionary<CurrencyType, int> CurrencyAmounts = new Dictionary<CurrencyType, int>();

    [SerializeField] private List<GameObject> texts;

    private Dictionary<CurrencyType, TextMeshProUGUI> currencyTexts = new Dictionary<CurrencyType, TextMeshProUGUI> ();

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
            CurrencyAmounts[type] = 0;
        }

        for (int i = 0; i < texts.Count; i++)
        {
            CurrencyType currencyType = (CurrencyType)i;
            if (!CurrencyAmounts.ContainsKey(currencyType))
            {
                CurrencyAmounts.Add(currencyType, 0); 
            }

            currencyTexts.Add(currencyType, texts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>());
        }
    }
    private void Start()
    {
        ApplyStartConfig();
        
        UpdateUI();

        EventManager.Instance.AddListener<CurrencyChangeGameEvent>(OnCurrencyChange);
        EventManager.Instance.AddListener<NotEnoughCurrencyGameEvent>(OnNotEnough);
    }

    private void ApplyStartConfig()
    {
        if (startConfig == null)
        {
            Debug.LogWarning("CurrencyStartConfig not assigned!");
            return;
        }

        foreach (var entry in startConfig.startAmounts)
        {
            CurrencyAmounts[entry.currencyType] = entry.amount;
        }
        
    }

    public static int GetCurrencyAmount(CurrencyType currencyType)
    {
        if (CurrencyAmounts.ContainsKey(currencyType))
        {
            return CurrencyAmounts[currencyType];
        }
        return 0;
    }

    private void OnCurrencyChange(CurrencyChangeGameEvent info)
    {
        if (info.amount < 0)
        {
            if (!CurrencyAmounts.ContainsKey(info.currencyType) ||
                CurrencyAmounts[info.currencyType] < Mathf.Abs(info.amount))
            {
                EventManager.Instance.QueueEvent(
                    new NotEnoughCurrencyGameEvent(info.amount, info.currencyType)
                );
                return;
            }

            EventManager.Instance.QueueEvent(new EnoughCurrencyGameEvent());
        }

        CurrencyAmounts[info.currencyType] += info.amount;

        if (currencyTexts.TryGetValue(info.currencyType, out var text))
        {
            text.text = CurrencyAmounts[info.currencyType].ToString();
        }

        UpdateUI();
        ShopManager.current.UpdateShopItems();
    }

    public bool IsEnoughMoneyForUpgrade(CurrencyType currencyType, int amount)
    {
        if (CurrencyAmounts.ContainsKey(currencyType) && CurrencyAmounts[currencyType] >= amount)
        {
            return true;
        }
        return false;
    }

    public bool TrySpendCurrency(CurrencyType currencyType, int amount)
    {
        if (!CurrencyAmounts.ContainsKey(currencyType))
            return false;

        if (CurrencyAmounts[currencyType] < amount)
            return false;

        CurrencyAmounts[currencyType] -= amount;

        if (currencyTexts.TryGetValue(currencyType, out var text))
        {
            text.text = CurrencyAmounts[currencyType].ToString();
        }

        UpdateUI();
        return true;
    }

    private void OnNotEnough(NotEnoughCurrencyGameEvent info)
    {
        Debug.Log(message: $"You don't have enough of {info.amount} {info.currencyType}");
    }

    private void UpdateUI()
    {
        foreach (var pair in currencyTexts)
        {
            pair.Value.text = CurrencyAmounts[pair.Key].ToString();
        }
    }
}

public enum CurrencyType
{
    Coins,
    Crystals,

    Blueprint_Primitive,
    Blueprint_Developed,
    Blueprint_Industrial,
    Blueprint_Modern,
    Blueprint_Futuristic
}

