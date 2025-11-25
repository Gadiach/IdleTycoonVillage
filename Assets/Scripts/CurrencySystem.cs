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
        CurrencyAmounts[CurrencyType.Coins] = 50;
        CurrencyAmounts[CurrencyType.Crystals] = 10;
        UpdateUI();

        EventManager.Instance.AddListener<CurrencyChangeGameEvent>(OnCurrencyChange);
        EventManager.Instance.AddListener<NotEnoughCurrencyGameEvent>(OnNotEnough);
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
        if(info.amount < 0)
        {
            if (CurrencyAmounts[info.currencyType] < Math.Abs(info.amount))
                {
                EventManager.Instance.QueueEvent(new NotEnoughCurrencyGameEvent(info.amount, info.currencyType));
                return;
            }
            EventManager.Instance.QueueEvent(new EnoughCurrencyGameEvent());
        }

        if (CurrencyAmounts.ContainsKey(info.currencyType))
        {
            CurrencyAmounts[info.currencyType] += info.amount;

            currencyTexts[info.currencyType].text = CurrencyAmounts[info.currencyType].ToString();
        }
        else
        {
            Debug.LogError($"Currency type {info.currencyType} not found in dictionary!");
        }

        UpdateUI();
        ShopManager.current.UpdateShopItems();
        UIManager.Instance.UpdateUIForUpgrade();
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
        if (CurrencyAmounts.ContainsKey(currencyType) && CurrencyAmounts[currencyType] >= amount)
        {
            CurrencyAmounts[currencyType] -= amount;

            currencyTexts[currencyType].text = CurrencyAmounts[currencyType].ToString();

            UpdateUI();

            return true;
        }
        else
        {
            return false; 
        }
    }

    private void OnNotEnough(NotEnoughCurrencyGameEvent info)
    {
        Debug.Log(message: $"You don't have enough of {info.amount} {info.currencyType}");
    }

    private void UpdateUI()
    {
        for(int i = 0; i < texts.Count; i++)
        {
            currencyTexts[(CurrencyType)i].text = CurrencyAmounts[(CurrencyType)i].ToString();
        }
    }
}

public enum CurrencyType
{
    Coins,
    Crystals
}

