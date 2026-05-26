using UnityEngine;

[CreateAssetMenu(fileName = "BuildingBalanceConfig",menuName = "Configs/Building Balance Config")]

public class BuildingBalanceConfig : ScriptableObject
{
    [Header("Upgrade")]
    public int baseUpgradePrice = 5;

    public float upgradeMultiplier = 1.25f;

    [Header("Income")]
    public int baseIncomePerCycle = 5;

    [Header("Production")]
    public float baseProductionDuration = 10f;

    [Header("Automation")]
    public int workerLevelNeededForAutomation = 5;
}