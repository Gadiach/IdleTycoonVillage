using UnityEngine;

[CreateAssetMenu(fileName = "Mission", menuName = "Missions/Mission")]
public class MissionData : ScriptableObject
{
    [SerializeField] private bool needBusinessType;
    [SerializeField] private BusinessType targetBusinessType;

    [SerializeField] private bool needRarity;
    [SerializeField] private Rarities targetRarity;

    public bool NeedBusinessType => needBusinessType;
    public bool NeedRarity => needRarity;

    public BusinessType TargetBusinessType => targetBusinessType;
    public Rarities TargetRarity => targetRarity;

    [Header("Identification")]
    public string id;

    [Header("UI")]
    public string missionName;

    [Header("Mission")]
    public MissionType missionType;
    public int targetValue;

    [Header("Reward")]
    public CurrencyType rewardCurrency;
    public int rewardAmount;
}