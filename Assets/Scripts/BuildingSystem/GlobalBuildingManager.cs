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
            existing.Rarity = building.CurrentRarity;
        }
        else
        {
            globalBuildings.Add(new BuildingGlobalData
            {
                Type = building.Type,
                Rarity = building.CurrentRarity
            });
        }
    }

    public BuildingGlobalData GetGlobalBuilding(ShopCategory type)
    {
        return globalBuildings.Find(b => b.Type == type);
    }

    public bool IsBuildingUnlocked(ShopCategory type)
    {
        return globalBuildings.Exists(b => b.Type == type);
    }
}

[System.Serializable]
public class BuildingGlobalData
{
    public ShopCategory Type;
    public Rarities Rarity;
}