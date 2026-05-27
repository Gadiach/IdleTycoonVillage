using UnityEngine;

[CreateAssetMenu(fileName = "BuildingBalanceConfig",menuName = "Configs/Building Balance Config")]

public class BuildingBalanceConfig : ScriptableObject
{
    [Header("Upgrade")]
    public int BaseUpgradePrice = 5;

    [Header("Income")]
    public int BaseIncomePerCycle = 5;

    [Header("Production")]
    public float BaseProductionDuration = 10f;

    [Header("Automation")]
    public int WorkerLevelNeededForAutomation = 5;
}