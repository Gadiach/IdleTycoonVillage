using System.Collections.Generic;
using UnityEngine;

public class GlobalBuildingManager : MonoBehaviour
{
    public static GlobalBuildingManager Instance;

    [Header("Global progress of buildings")]
    public List<BuildingGlobalData> globalBuildings = new List<BuildingGlobalData>();

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateGlobalBuilding(BuildingData building)
    {
        var existing = globalBuildings.Find(b => b.Type == building.Type);

        if (existing != null)
        {
            existing.Rarity = building.Rarity;
        }
        else
        {
            globalBuildings.Add(new BuildingGlobalData
            {
                Type = building.Type,
                Rarity = building.Rarity
            });
        }
    }

    public BuildingGlobalData GetGlobalBuilding(ObjectType type)
    {
        return globalBuildings.Find(b => b.Type == type);
    }

    public bool IsBuildingUnlocked(ObjectType type)
    {
        return globalBuildings.Exists(b => b.Type == type);
    }
}

[System.Serializable]
public class BuildingGlobalData
{
    public ObjectType Type;
    public Rarities Rarity;
}