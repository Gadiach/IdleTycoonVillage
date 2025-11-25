using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField] private List<BuildingData> activeBuildings = new List<BuildingData>(); 

    private void Awake()
    {
        Instance = this;
    }

    public void AddBuildingToList(BuildingData building)
    {
        if (!activeBuildings.Contains(building))
            activeBuildings.Add(building);
    }

    public void RemoveBuildingFromList(BuildingData building)
    {
        if (activeBuildings.Contains(building))
            activeBuildings.Remove(building);
    }

    public List<BuildingData> GetAllActiveBuildings() => activeBuildings;
}