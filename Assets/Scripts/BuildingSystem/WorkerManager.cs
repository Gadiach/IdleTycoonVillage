using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager current;

    public List<WorkerData> allWorkers = new List<WorkerData>();

    [SerializeField] private WorkerIconLibrary iconLibrary;

    [SerializeField] private ProgressionConfig progressionConfig;

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
        var w = new WorkerData
        {
            type = type,
            available = true,

            CurrentLevel = 1,

            CurrentTier = Tiers.Tier1,

            CurrentRarity = Rarities.Primitive,

            Currency = CurrencyType.Coins,

            PriceToUpgrade = 3,

            Icon = iconLibrary.GetIcon(type),

            roundIcon = iconLibrary.GetRoundIcon(type),

            ProgressionConfig = progressionConfig
        };

        if (register) AddWorker(w);
        return w;
    }
}