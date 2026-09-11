using UnityEngine;

public class BuildingUpgradeIndicator : MonoBehaviour
{
    [SerializeField] private GameObject upgradeIndicator;

    private BuildingData building;

    private void Awake()
    {
        building = GetComponentInParent<BuildingData>();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
        EventManager.Instance.AddListener<WorkerAssignedToBuildingEvent>(OnWorkerAssigned);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
        EventManager.Instance.RemoveListener<WorkerAssignedToBuildingEvent>(OnWorkerAssigned);
    }

    private void Start()
    {
        UpdateIndicator();
    }

    private void OnCurrencyChanged(CurrencyChangedEvent evt)
    {
        UpdateIndicator();
    }

    private void OnWorkerAssigned(WorkerAssignedToBuildingEvent evt)
    {
        if (evt.Building != building)
            return;

        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        if (building == null || building.Placeable == null)
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