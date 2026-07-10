using UnityEngine;

public class MissionData : ScriptableObject
{
    [Header("Identification")]
    public string id;

    [Header("UI")]
    public string missionName;
    public Sprite icon;

    [Header("Mission")]
    public MissionType missionType;
    public int targetValue;

    [Header("Reward")]
    public CurrencyType rewardCurrency;
    public int rewardAmount;
}