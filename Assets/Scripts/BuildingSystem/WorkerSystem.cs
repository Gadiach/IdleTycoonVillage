using System.Collections.Generic;
using UnityEngine;

public class WorkerSystem : MonoBehaviour
{
    public static WorkerSystem Instance;

    [SerializeField] private ProgressionConfig progressionConfig;
    [SerializeField] private UpgradeCostConfig upgradeCostConfig;
    [SerializeField] private EconomyProgressionConfig economyConfig;
    [SerializeField] private WorkerDefinition[] workerDefinitions;

    private void Awake()
    {
        Instance = this;
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

    public WorkerData CreateWorker(BusinessType type, bool register)
    {
        WorkerDefinition definition = GetDefinition(type);

        WorkerData worker = new WorkerData(definition,progressionConfig,upgradeCostConfig,economyConfig);

        if (register)
            EntityRegistry.Instance.AddWorker(worker);

        return worker;
    }
}