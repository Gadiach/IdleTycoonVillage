using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyStartConfig", menuName = "Configs/Currency Start Config")]

public class CurrencyStartConfig : ScriptableObject
{
    public CurrencyAmount[] startAmounts;
}

[Serializable]
public struct CurrencyAmount
{
    public CurrencyType currencyType;
    public int amount;
}