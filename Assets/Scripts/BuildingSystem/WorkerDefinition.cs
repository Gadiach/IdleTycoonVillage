using UnityEngine;

[CreateAssetMenu(fileName = "WorkerDefinition", menuName = "Game/Worker Definition")]
public class WorkerDefinition : ScriptableObject
{
    [Header("Identity")]
    public BusinessType Type;

    [Header("Visual")]
    public Sprite Icon;
    public Sprite RoundIcon;

    [Header("Economy")]
    public CurrencyType Currency;
    public float BaseProductionDuration = 10f;
    public float BaseUpgradePrice = 3f;

    [Header("Progression")]
    public float ProductionTimeReductionPerLevel = 0.0015f;
}