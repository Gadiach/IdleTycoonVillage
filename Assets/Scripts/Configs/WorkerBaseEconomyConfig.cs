using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "WorkerBaseEconomyConfig",menuName = "Configs/Worker Base Economy Config")]
public class WorkerBaseEconomyConfig : ScriptableObject
{
    [Serializable]
    public class WorkerEconomyEntry
    {
        public BusinessType BusinessType;
        public CurrencyType LevelUpgradeCurrency;

        public float BaseProductionDuration = 10f;
        public float BaseUpgradePrice = 3f;
        public float ProductionTimeReductionPerLevel = 0.0015f;
    }

    [SerializeField] private WorkerEconomyEntry[] workers;

    public WorkerEconomyEntry GetWorkerEconomy(BusinessType type)
    {
        foreach (var worker in workers)
        {
            if (worker.BusinessType == type)
                return worker;
        }

        Debug.LogError($"Worker base economy not found: {type}");
        return null;
    }
}