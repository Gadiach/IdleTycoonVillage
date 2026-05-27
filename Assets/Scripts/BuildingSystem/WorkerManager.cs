using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager current;

    public List<WorkerData> allWorkers = new List<WorkerData>();

    [SerializeField] private WorkerIconLibrary iconLibrary;

    [SerializeField] private ProgressionConfig progressionConfig;
    [SerializeField] private UpgradeCostConfig upgradeCostConfig;
    private void Awake()
    {
        current = this;
    }

    public void AddWorker(WorkerData worker)
    {
        if (!allWorkers.Contains(worker))
        {
            allWorkers.Add(worker);
        }
    }

    public WorkerData CreateWorker(BusinessType type, bool register)
    {
        var w = new WorkerData(progressionConfig,upgradeCostConfig)
        {
            Type = type,

            Currency = CurrencyType.Coins,

            Icon = iconLibrary.GetIcon(type),

            RoundIcon = iconLibrary.GetRoundIcon(type),
        };

        if (register) AddWorker(w);

        return w;
    }
}