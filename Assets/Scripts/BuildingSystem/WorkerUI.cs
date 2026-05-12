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
    private int priceToUpgradeWorker = 1;
    private WorkerData currentWorker;

    [SerializeField] private GameObject workerPanel;

    private void Awake()
    {
        Instance = this;
        workerPanel.SetActive(false);
    }

    public void OpenWorkerPanel(WorkerData worker)
    {
        currentWorker = worker;
        UpdatePriceToUpgradeWorker(currentWorker);
        
        icon.sprite = worker.icon;
        UpdateLvlTxt(currentWorker);
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
        UpdateLvlTxt(currentWorker);

        EventManager.Instance.QueueEvent(new WorkerUpgradedEvent(currentWorker));
    }

    private void UpdatePriceToUpgradeWorker(WorkerData worker)
    {
        priceToUpgradeWorker = priceToUpgradeWorker * worker.level;
    }

    private void UpdateLvlTxt(WorkerData worker)
    {
        levelText.text = "Level: " + worker.level;
    }
}