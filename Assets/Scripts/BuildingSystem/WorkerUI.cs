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
    [SerializeField] private GameObject[] nextTierUpgradeStars;
    [SerializeField] private StarUpgradeVFX starUpgradeVFX;

    [SerializeField] private Image workerIcon;

    [SerializeField] private TextMeshProUGUI currentMaxLvlTxt;
    [SerializeField] private TextMeshProUGUI nextMaxLvlTxt;

    [SerializeField] private TextMeshProUGUI currentMinCycleDurationText;
    [SerializeField] private TextMeshProUGUI nextMinCycleDurationText;

    [SerializeField] private Sprite addStarButtonActiveSprite;
    [SerializeField] private Sprite addStarButtonInactiveSprite;

    [SerializeField] private Button addStarButton;

    [SerializeField] private GameObject workerPanel;

    [SerializeField] private Image blueprintImage;
    [SerializeField] private TextMeshProUGUI upgradePriceText;
    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;

    [SerializeField] private GameObject blackBackground;

    private WorkerData currentWorker;

    private void Awake()
    {
        Instance = this;
        workerPanel.SetActive(false);
        blackBackground.SetActive(false);
        HideAllUpgradeStars();
    }

    public void OpenWorkerPanel(WorkerData worker)
    {
        currentWorker = worker;

        workerIcon.sprite = currentWorker.Icon;

        UpdateWorkerPanelUI();

        workerPanel.SetActive(true);

        blackBackground.SetActive(true);

    }

    private void UpdateWorkerPanelUI()
    {
        UpdateCurrentRarityText();

        UpdateStarUI(currentWorker);

        UpdateNextTierUpgradeStar();

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
        currentMinCycleDurationText.text = currentWorker.CurrentProgressionMinCycleDuration.ToString("F2");
    }

    private void UpdateNextMinCycleDurationText()
    {
        nextMinCycleDurationText.text = currentWorker.NextProgressionMinCycleDuration.ToString("F2");
    }

    private void UpdateStarUpgradeButton()
    {
        SetAddStarButtonState(currentWorker.CanUpgradeTierOrRarity);
    }

    private void SetAddStarButtonState(bool interactable)
    {
        addStarButton.interactable = interactable;

        UpdateAddStarButtonSprite(interactable);
    }

    private void UpdateAddStarButtonSprite(bool interactable)
    {
        addStarButton.image.sprite = interactable ? addStarButtonActiveSprite : addStarButtonInactiveSprite;
    }

    private void UpdateBlueprintPriceUI()
    {
        if (currentWorker.IsMaxProgression)
        {
            SetBlueprintRequirementVisible(false);
            return;
        }

        UpdateBlueprintRequirement(currentWorker.TierOrRarityUpgradeCurrency,currentWorker.PriceToUpgradeTierOrRarity);
    }

    private void UpdateBlueprintRequirement(CurrencyType currencyType,int requiredAmount)
    {
        int ownedAmount = CurrencySystem.GetCurrencyAmount(currencyType);

        UpdateBlueprintIcon(currencyType);
        UpdateUpgradePriceText(ownedAmount, requiredAmount);
        UpdateUpgradePriceTextColor();

        SetBlueprintRequirementVisible(true);
    }

    private void SetBlueprintRequirementVisible(bool visible)
    {
        blueprintImage.gameObject.SetActive(visible);
        upgradePriceText.gameObject.SetActive(visible);
    }

    private void UpdateBlueprintIcon(CurrencyType currencyType)
    {
        blueprintImage.sprite = currencyIconDatabase.GetIcon(currencyType);
    }

    private void UpdateUpgradePriceText(int ownedAmount, int requiredAmount)
    {
        upgradePriceText.text = $"{ownedAmount}/{requiredAmount}";
    }

    private void UpdateUpgradePriceTextColor()
    {
        upgradePriceText.color = currentWorker.HasEnoughCurrencyForTierOrRarityUpgrade ? Color.white : Color.red;
    }

    public void OnAddStarButtonClicked()
    {
        if (!currentWorker.CanUpgradeTierOrRarity)
            return;

        GameObject sourceStar = GetActiveUpgradeStar();
        Image targetStar = GetUpgradeTargetStar();

        Vector3 sourcePosition = sourceStar.transform.position;
        Sprite sourceSprite = sourceStar.GetComponent<Image>().sprite;

        Color previousStarColor = targetStar.color;

        currentWorker.UpgradeTierOrRarity();

        UpdateWorkerPanelUI();

        Color upgradedStarColor = targetStar.color;
        targetStar.color = previousStarColor;

        PlayStarUpgradeVFX(sourcePosition,sourceSprite,targetStar,upgradedStarColor);
    }

    private Image GetUpgradeTargetStar()
    {
        int targetIndex = currentWorker.CurrentTier == Tiers.Tier5 ? 0 : (int)currentWorker.CurrentTier;

        return currentTierRarityStarsColor[targetIndex];
    }

    private GameObject GetActiveUpgradeStar()
    {
        foreach (GameObject star in nextTierUpgradeStars)
        {
            if (star.activeSelf)
                return star;
        }

        return null;
    }

    private void PlayStarUpgradeVFX(Vector3 sourcePosition,Sprite sourceSprite,Image targetStar,Color targetColor)
    {
        starUpgradeVFX.PlayWorkerUpgrade(sourcePosition,targetStar,sourceSprite,targetColor);
    }

    private void HideAllUpgradeStars()
    {
        foreach (GameObject star in nextTierUpgradeStars)
        {
            star.SetActive(false);
        }
    }

    private void UpdateNextTierUpgradeStar()
    {
        HideAllUpgradeStars();

        if (currentWorker.IsMaxProgression)
            return;

        int upgradeStarIndex;

        if (currentWorker.CurrentTier == Tiers.Tier5)
        {
            upgradeStarIndex = 0;
        }
        else
        {
            upgradeStarIndex = (int)currentWorker.NextTier - 1;
        }

        nextTierUpgradeStars[upgradeStarIndex].SetActive(true);
    }

    public void ClosePanel()
    {
        workerPanel.SetActive(false);
        blackBackground.SetActive(false);
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