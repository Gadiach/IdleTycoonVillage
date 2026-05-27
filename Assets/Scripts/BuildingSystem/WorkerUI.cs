using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUI : MonoBehaviour
{
    public static WorkerUI Instance;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text speedBonusText;
    [SerializeField] private Text incomeBonusText;
    [SerializeField] private TextMeshProUGUI priceForUpgradingText;
    private WorkerData currentWorker;
    [SerializeField] private int baseUpgradePrice = 10;

    [SerializeField] private GameObject workerPanel;
    [SerializeField] private Button upgradeButton;

    private void Awake()
    {
        Instance = this;
        workerPanel.SetActive(false);
    }

    public void OpenWorkerPanel(WorkerData worker)
    {
        currentWorker = worker;

        UpdateUpgradeButtonState();

        icon.sprite = worker.Icon;

        UpdateLvlTxt(currentWorker);

        priceForUpgradingText.text = GetUpgradePrice(currentWorker).ToString();

        workerPanel.SetActive(true);
    }

    private void UpdateUpgradeButtonState()
    {
        int price = GetUpgradePrice(currentWorker);

        bool canAfford =
            CurrencySystem.Instance.IsEnoughMoneyForUpgrade(
                CurrencyType.Coins,
                price);

        upgradeButton.interactable = canAfford;
    }

    

    private void UpdateLvlTxt(WorkerData worker)
    {
        levelText.text = "Level: " + worker.CurrentLevel;
    }

    private int GetUpgradePrice(WorkerData worker)
    {
        return Mathf.RoundToInt(baseUpgradePrice * Mathf.Pow(1.25f, worker.CurrentLevel));
    }
}