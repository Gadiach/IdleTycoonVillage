using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Currency Icon Database", menuName = "Currency/Currency Icon Database")]
public class CurrencyIconDatabase : ScriptableObject
{
    [SerializeField] private CurrencyIcon[] currencyIcons;

    

    public Sprite GetIcon(CurrencyType currencyType)
    {
        foreach (var currencyIcon in currencyIcons)
        {
            if (currencyIcon.CurrencyType == currencyType)
                return currencyIcon.Icon;
        }

        Debug.LogWarning($"No icon found for currency: {currencyType}");
        return null;
    }
}

[Serializable]
public class CurrencyIcon
{
    public CurrencyType CurrencyType;
    public Sprite Icon;
}