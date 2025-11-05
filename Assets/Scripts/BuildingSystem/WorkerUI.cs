using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUI : MonoBehaviour
{
    public static WorkerUI Instance;

    [SerializeField] private Image icon;
    [SerializeField] private Text levelText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text speedBonusText;
    [SerializeField] private Text incomeBonusText;
    [SerializeField] private TextMeshProUGUI priceForUpgradingText;
    private int priceToUpgradeWorker = 1;
    private WorkerData currentWorker;

    [SerializeField] private GameObject workerPanel;

    private void Awake()
    {
        Instance = this;
        workerPanel.SetActive(false);
    }

    public void ShowWorker(WorkerData worker)
    {
        currentWorker = worker;
        UpdatePriceToUpgradeWorker(currentWorker);

        if (worker == null) return;
        icon.sprite = worker.icon;
        //levelText.text = "Level: " + worker.level;
        //typeText.text = "Type: " + worker.type.ToString();
        //speedBonusText.text = "Speed Bonus: " + worker.speedBonus;
        //incomeBonusText.text = "Income Bonus: " + worker.incomeBonus;

        workerPanel.SetActive(true);
    }

    public void UpgradeWorker()
    {
        currentWorker.level++;
        priceForUpgradingText.text = priceToUpgradeWorker.ToString();

        UpdatePriceToUpgradeWorker(currentWorker);
    }

    private void UpdatePriceToUpgradeWorker(WorkerData worker)
    {
        priceToUpgradeWorker = priceToUpgradeWorker * worker.level;
    }
}