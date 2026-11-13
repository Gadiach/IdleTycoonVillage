using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingIconDatabase",menuName = "Databases/Building Icon Database")]
public class BuildingIconDatabase : ScriptableObject
{
    [Serializable]
    public class BuildingIconEntry
    {
        public BusinessType BusinessType;
        public Rarities Rarity;
        public Sprite Icon;
    }

    [SerializeField] private BuildingIconEntry[] icons;

    public Sprite GetIcon(BusinessType businessType, Rarities rarity)
    {
        foreach (var entry in icons)
        {
            if (entry.BusinessType == businessType &&
                entry.Rarity == rarity)
            {
                return entry.Icon;
            }
        }

        Debug.LogWarning(
            $"No building icon found for {businessType}, {rarity}"
        );

        return null;
    }
}