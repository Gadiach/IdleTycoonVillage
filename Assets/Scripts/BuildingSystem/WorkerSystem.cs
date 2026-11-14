using UnityEngine;

public class WorkerSystem : MonoBehaviour
{
    public static WorkerSystem Instance;

    [Header("Configs")]
    [SerializeField] private ProgressionConfig progressionConfig;
    [SerializeField] private UpgradeCostConfig upgradeCostConfig;
    [SerializeField] private EconomyProgressionConfig economyConfig;
    [SerializeField] private WorkerBaseEconomyConfig workerBaseEconomyConfig;

    [Header("Databases")]
    [SerializeField] private WorkerIconDatabase workerIconDatabase;

    private void Awake()
    {
        Instance = this;
    }

    public WorkerData CreateWorker(BusinessType type, bool register)
    {
        WorkerData worker = new WorkerData(
            type,
            progressionConfig,
            upgradeCostConfig,
            economyConfig,
            workerBaseEconomyConfig
        );

        if (register)
            EntityRegistry.Instance.AddWorker(worker);

        return worker;
    }

    public Sprite GetRoundIcon(BusinessType businessType, Rarities rarity)
    {
        return workerIconDatabase.GetRoundIcon(businessType, rarity);
    }

    public Sprite GetFullBodyIcon(BusinessType businessType, Rarities rarity)
    {
        return workerIconDatabase.GetFullBodyIcon(businessType, rarity);
    }
}