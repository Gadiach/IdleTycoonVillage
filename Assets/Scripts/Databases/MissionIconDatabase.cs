using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission Icon Database", menuName = "Missions/Mission Icon Database")]
public class MissionIconDatabase : ScriptableObject
{
    [SerializeField] private MissionIcon[] missionIcons;

    public Sprite GetIcon(MissionType missionType)
    {
        foreach (var missionIcon in missionIcons)
        {
            if (missionIcon.MissionType == missionType)
                return missionIcon.Icon;
        }

        Debug.LogWarning($"No icon found for mission: {missionType}");
        return null;
    }
}

[Serializable]
public class MissionIcon
{
    public MissionType MissionType;
    public Sprite Icon;
}