using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager current;

    public List<WorkerData> allWorkers = new List<WorkerData>();

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
}