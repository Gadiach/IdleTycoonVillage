using UnityEngine;

public class BuildingUpgradeIndicator : MonoBehaviour
{
    [Header("Indicators")]
    [SerializeField] private GameObject levelUpgradeIndicator;
    [SerializeField] private GameObject starUpgradeIndicator;

    private BuildingData building;

    public void Initialize(BuildingData buildingData)
    {
        building = buildingData;

        UpdateIndicators();
    }

    public void UpdateIndicators()
    {
        if (building == null)
            return;

        WorkerData worker = building.Placeable != null
            ? building.Placeable.GetAssignedWorker()
            : null;

        bool canUpgradeLevel =
            CanUpgradeBuildingLevel() ||
            CanUpgradeWorkerLevel(worker);

        bool canUpgradeStar =
            CanUpgradeBuildingStar() ||
            CanUpgradeWorkerStar(worker);

        levelUpgradeIndicator.SetActive(canUpgradeLevel);
        starUpgradeIndicator.SetActive(canUpgradeStar);
    }

    private bool CanUpgradeBuildingLevel()
    {
        return building.CurrentLevel < building.CurrentProgressionMaxLevel &&
               CurrencySystem.Instance.HasEnoughCurrency(
                   building.LevelUpgradeCurrency,
                   building.PriceToUpgradeLevel
               );
    }

    private bool CanUpgradeWorkerLevel(WorkerData worker)
    {
        if (worker == null)
            return false;

        return worker.CurrentLevel < worker.CurrentProgressionMaxLevel &&
               CurrencySystem.Instance.HasEnoughCurrency(
                   worker.Currency,
                   worker.PriceToUpgrade
               );
    }

    private bool CanUpgradeBuildingStar()
    {
        return building.CanUpgradeTierOrRarity;
    }

    private bool CanUpgradeWorkerStar(WorkerData worker)
    {
        if (worker == null)
            return false;

        return worker.CanUpgradeTierOrRarity;
    }
}