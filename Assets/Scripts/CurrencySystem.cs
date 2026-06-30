using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
    public static CurrencySystem Instance;

    private static Dictionary<CurrencyType, int> currencyAmounts = new Dictionary<CurrencyType, int>();

    [SerializeField] private CurrencyStartConfig startConfig;

    private void Awake()
    {
        InitializeSingleton();

        InitializeCurrencyAmountsToZero();

        ApplyStartConfig();
    }


    #region Initialization

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void InitializeCurrencyAmountsToZero()
    {
        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            currencyAmounts[type] = 0;
        }
    }

    #endregion
   
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

        EventManager.Instance.QueueEvent(new CurrencySpentEvent(currencyType, amount));

        return true;
    }

    #endregion

    #region Add Currency Logic

    public void AddCurrency(CurrencyType currencyType, int amount)
    {
        currencyAmounts[currencyType] += amount;

        EventManager.Instance.QueueEvent(new CurrencyAddedEvent(currencyType, amount));
    }

    #endregion
}



