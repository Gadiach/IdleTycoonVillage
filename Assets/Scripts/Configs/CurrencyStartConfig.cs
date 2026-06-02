using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyStartConfig", menuName = "Configs/Currency Start Config")]

public class CurrencyStartConfig : ScriptableObject
{
    [Header("Currencies")]
    public CurrencyAmount[] currencies;

    [Header("Building Blueprints")]
    public CurrencyAmount[] buildingBlueprints;

    [Header("Worker Blueprints")]
    public CurrencyAmount[] workerBlueprints;
}

[Serializable]
public struct CurrencyAmount
{
    public CurrencyType currencyType;
    public int amount;
}