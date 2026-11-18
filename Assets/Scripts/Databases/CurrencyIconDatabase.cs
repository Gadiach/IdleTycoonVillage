using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrencyIconDatabase",menuName = "Databases/Currency Icon Database")]
public class CurrencyIconDatabase : ScriptableObject
{
    [Serializable]
    public class CurrencyIconEntry
    {
        public CurrencyCategory Category;
        public Sprite Icon;
    }

    [SerializeField] private CurrencyIconEntry[] icons;

    public Sprite GetIcon(CurrencyCategory category)
    {
        foreach (var entry in icons)
        {
            if (entry.Category == category)
                return entry.Icon;
        }

        Debug.LogWarning($"Currency icon not found: {category}");
        return null;
    }
}