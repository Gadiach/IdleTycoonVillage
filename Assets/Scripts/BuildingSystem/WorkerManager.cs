using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager current;

    public List<WorkerData> allWorkers = new List<WorkerData>();

    [SerializeField] private ProgressionConfig progressionConfig;
    [SerializeField] private UpgradeCostConfig upgradeCostConfig;
    [SerializeField] private EconomyProgressionConfig economyConfig;
    [SerializeField] private WorkerDefinition[] workerDefinitions;

    private void Awake()
    {
        current = this;
    }

    private WorkerDefinition GetDefinition(BusinessType type)
    {
        foreach (var definition in workerDefinitions)
        {
            if (definition.Type == type)
                return definition;
        }

        Debug.LogError($"WorkerDefinition for {type} not found.");
        return null;
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
        WorkerDefinition definition = GetDefinition(type);

        WorkerData worker = new WorkerData(definition,progressionConfig,upgradeCostConfig,economyConfig);

        if (register)
            AddWorker(worker);

        return worker;
    }
}