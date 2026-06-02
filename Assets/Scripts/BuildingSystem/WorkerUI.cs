using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUI : MonoBehaviour
{
    public static WorkerUI Instance;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI currentRarityText;

    [SerializeField] private Image[] currentTierRarityStarsColor;
    [SerializeField] private Image[] nextTierRarityStarsColor;

    [SerializeField] private Image workerIcon;

    [SerializeField] private TextMeshProUGUI currentMaxLvlTxt;
    [SerializeField] private TextMeshProUGUI nextMaxLvlTxt;

    [SerializeField] private TextMeshProUGUI currentMinCycleDurationText;
    [SerializeField] private TextMeshProUGUI nextMinCycleDurationText;

    [SerializeField] private Button addStarButton;

    [SerializeField] private GameObject workerPanel;

    [SerializeField] private GameObject[] blueprintSlots;
    [SerializeField] private TextMeshProUGUI[] upgradePriceTexts;

    private WorkerData currentWorker;

    private void Awake()
    {
        Instance = this;
        workerPanel.SetActive(false);
    }

    public void OpenWorkerPanel(WorkerData worker)
    {
        currentWorker = worker;

        workerIcon.sprite = currentWorker.Icon;

        UpdateWorkerPanelUI();

        workerPanel.SetActive(true);
    }

    private void UpdateWorkerPanelUI()
    {
        UpdateCurrentRarityText();

        UpdateStarUI(currentWorker);

        UpdateCurrentMaxLvlTxt();

        UpdateNextMaxLvlTxt();

        UpdateCurrentMinCycleDurationText();

        UpdateNextMinCycleDurationText();

        UpdateStarUpgradeButton();

        UpdateBlueprintPriceUI();
    }

    private void UpdateCurrentRarityText()
    {
        currentRarityText.text = currentWorker.CurrentRarity.ToString();
    }

    private void UpdateCurrentMaxLvlTxt()
    {
        currentMaxLvlTxt.text = currentWorker.CurrentProgressionMaxLevel.ToString();
    }

    private void UpdateNextMaxLvlTxt()
    {
        nextMaxLvlTxt.text = currentWorker.NextProgressionMaxLevel.ToString();
    }

    private void UpdateCurrentMinCycleDurationText()
    {
        currentMinCycleDurationText.text = currentWorker.CurrentProgressionMinCycleDuration.ToString("F1");
    }

    private void UpdateNextMinCycleDurationText()
    {
        nextMinCycleDurationText.text = currentWorker.NextProgressionMinCycleDuration.ToString("F1");
    }

    private void UpdateStarUpgradeButton()
    {
        addStarButton.interactable = currentWorker.CanUpgradeTierOrRarity();
    }

    private void UpdateBlueprintPriceUI()
    {
        var requirements = currentWorker.GetBlueprintRequirementsForNextUpgrade();

        int index = 0;

        foreach (var req in requirements)
        {
            blueprintSlots[index].SetActive(true);

            int owned = CurrencySystem.GetCurrencyAmount(req.Key);

            int required = req.Value;

            upgradePriceTexts[index].text = $"{owned}/{required}";

            upgradePriceTexts[index].color = owned >= required ? Color.white : Color.red;

            index++;
        }

        for (int i = index; i < blueprintSlots.Length; i++)
        {
            blueprintSlots[i].SetActive(false);
        }
    }

    public void OnAddStarButtonClicked()
    {
        currentWorker.UpgradeTierOrRarity();

        UpdateWorkerPanelUI();
    }

    public void ClosePanel()
    {
        workerPanel.SetActive(false);
    }

    private Color GetColorByRarity(Rarities rarity)
    {
        return rarity switch
        {
            Rarities.Primitive => Color.blue,
            Rarities.Developed => Color.green,
            Rarities.Industrial => Color.yellow,
            Rarities.Modern => new Color(0.5f, 0f, 1f),
            Rarities.Futuristic => Color.red,
            _ => Color.grey
        };
    }

    private void UpdateStarUI(WorkerData worker)
    {
        var info = worker.GetStarDisplayInfo();

        int currentTier = (int)info.CurrentTier;
        int nextTier = (int)info.NextTierValue;

        int maxStars = currentTierRarityStarsColor.Length;

        Color activeColor = GetColorByRarity(info.CurrentRarity);

        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
        {
            currentTierRarityStarsColor[i].color = (i < currentTier) ? activeColor : inactiveColor;
        }

        if (nextTier > currentTier)
        {
            for (int i = 0; i < maxStars; i++)
            {
                nextTierRarityStarsColor[i].color = (i < nextTier) ? activeColor : inactiveColor;
            }

            return;
        }

        if (info.NextRarity != info.CurrentRarity)
        {
            Color nextColor = GetColorByRarity(info.NextRarity);

            for (int i = 0; i < maxStars; i++)
            {
                nextTierRarityStarsColor[i].color = (i == 0) ? nextColor : inactiveColor;
            }
        }
    }
}