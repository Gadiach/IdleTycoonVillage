using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueprintRewardConfig", menuName = "Configs/Blueprint Reward Config")]

public class BlueprintRewardConfig : ScriptableObject
{
    public BlueprintRewardChance[] rewards;
}

[Serializable]
public struct BlueprintRewardChance
{
    public CurrencyType blueprintType;
    public int weight;
}