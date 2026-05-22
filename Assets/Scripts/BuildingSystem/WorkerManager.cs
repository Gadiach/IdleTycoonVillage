using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager current;

    public List<WorkerData> allWorkers = new List<WorkerData>();

    [SerializeField] private WorkerIconLibrary iconLibrary;

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

    public WorkerData CreateWorker(BusinessType type)
    {
        WorkerData newWorker = new WorkerData
        {
            type = type,
            available = true,
            Icon = iconLibrary.GetIcon(type)
        };

        AddWorker(newWorker);
        return newWorker;
    }

    public WorkerData CreateWorker(BusinessType type, bool register)
    {
        var w = new WorkerData
        {
            type = type,
            available = true,
            roundIcon = iconLibrary.GetRoundIcon(type),
            Icon = iconLibrary.GetIcon(type)
        };

        if (register) AddWorker(w);
        return w;
    }
}