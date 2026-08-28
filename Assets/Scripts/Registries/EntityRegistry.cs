using System.Collections.Generic;
using UnityEngine;

public class EntityRegistry : MonoBehaviour
{
    public static EntityRegistry Instance;

    [SerializeField] private List<WorkerData> workers = new();
    [SerializeField] private List<BuildingData> buildings = new();

    public IReadOnlyList<WorkerData> Workers => workers;
    public IReadOnlyList<BuildingData> Buildings => buildings;

    private void Awake()
    {
        Instance = this;
    }

    public void AddWorker(WorkerData worker)
    {
        if (!workers.Contains(worker))
        {
            workers.Add(worker);
        }
    }

    public void RemoveWorker(WorkerData worker)
    {
        workers.Remove(worker);
    }

    public void AddBuilding(BuildingData building)
    {
        if (!buildings.Contains(building))
        {
            buildings.Add(building);
        }
    }

    public void RemoveBuilding(BuildingData building)
    {
        buildings.Remove(building);
    }

    public WorkerData GetWorker(BusinessType type)
    {
        foreach (WorkerData worker in workers)
        {
            if (worker.Type == type)
                return worker;
        }

        return null;
    }

    public BuildingData GetBuilding(BusinessType type)
    {
        foreach (BuildingData building in buildings)
        {
            if (building.BusinessType == type)
                return building;
        }

        return null;
    }
}