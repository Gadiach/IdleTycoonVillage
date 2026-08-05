using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission Icon Database", menuName = "Missions/Mission Icon Database")]
public class MissionIconDatabase : ScriptableObject
{
    [SerializeField] private MissionIcon[] missionIcons;

    public Sprite GetIcon(
    MissionType missionType,
    BusinessType businessType,
    Rarities rarity)
    {
        foreach (var missionIcon in missionIcons)
        {
            if (missionIcon.MissionType == missionType &&
                missionIcon.BusinessType == businessType &&
                missionIcon.Rarity == rarity)
            {
                return missionIcon.Icon;
            }
        }

        Debug.LogWarning(
            $"Mission icon not found. " +
            $"MissionType={missionType}, " +
            $"BusinessType={businessType}, " +
            $"Rarity={rarity}");

        return null;
    }
}

[Serializable]
public class MissionIcon
{
    public MissionType MissionType;
    public BusinessType BusinessType;
    public Rarities Rarity;
    public Sprite Icon;
}