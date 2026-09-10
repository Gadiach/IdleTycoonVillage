using UnityEngine;

public class BuildingUpgradeIndicator : MonoBehaviour
{
    [SerializeField] private GameObject upgradeIndicator;

    private BuildingData building;

    public void Initialize(BuildingData buildingData)
    {
        building = buildingData;

        UpdateIndicator();
    }

    public void UpdateIndicator()
    {
        if (building == null)
            return;

        WorkerData worker = building.Placeable.GetAssignedWorker();

        bool canUpgradeBuildingLevel =
            building.CanUpgradeLevel;

        bool canUpgradeBuildingStar =
            building.CanUpgradeTierOrRarity;

        bool canUpgradeWorkerLevel =
            worker != null && worker.CanUpgradeLevel;

        bool canUpgradeWorkerStar =
            worker != null && worker.CanUpgradeTierOrRarity;

        bool hasAvailableUpgrade =
            canUpgradeBuildingLevel ||
            canUpgradeBuildingStar ||
            canUpgradeWorkerLevel ||
            canUpgradeWorkerStar;

        upgradeIndicator.SetActive(hasAvailableUpgrade);
    }
}