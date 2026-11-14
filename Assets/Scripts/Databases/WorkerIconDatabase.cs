using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WorkerIconDatabase", menuName = "Databases/Worker Icon Database")]
public class WorkerIconDatabase : ScriptableObject
{
    [Serializable]
    public class WorkerIconEntry
    {
        public BusinessType BusinessType;
        public Rarities Rarity;

        [FormerlySerializedAs("Icon")]
        public Sprite FullBodyIcon;

        public Sprite RoundIcon;
    }

    [SerializeField] private WorkerIconEntry[] icons;
    [SerializeField] private Sprite defaultInactiveWorkerIcon;

    public Sprite DefaultInactiveWorkerIcon => defaultInactiveWorkerIcon;

    public Sprite GetFullBodyIcon(BusinessType businessType, Rarities rarity)
    {
        WorkerIconEntry entry = GetEntry(businessType, rarity);
        return entry?.FullBodyIcon;
    }

    public Sprite GetRoundIcon(BusinessType businessType, Rarities rarity)
    {
        WorkerIconEntry entry = GetEntry(businessType, rarity);
        return entry?.RoundIcon;
    }

    private WorkerIconEntry GetEntry(BusinessType businessType, Rarities rarity)
    {
        foreach (var entry in icons)
        {
            if (entry.BusinessType == businessType && entry.Rarity == rarity)
                return entry;
        }

        Debug.LogWarning($"Worker icon not found for {businessType}, {rarity}");
        return null;
    }
}