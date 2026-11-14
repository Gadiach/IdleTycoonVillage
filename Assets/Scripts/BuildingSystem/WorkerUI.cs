using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkerUI : MonoBehaviour
{
    public static WorkerUI Instance;

    [Header("Colors")]
    [SerializeField] private ColorDatabase colorDatabase;

    [Header("Databases")]
    [SerializeField] private WorkerIconDatabase workerIconDatabase;

    [Header("Upgrade Visual States")]
    [SerializeField] private GameObject workerCanUpgradeVisuals;
    [SerializeField] private GameObject workerNotEnoughCurrencyVisuals;

    [SerializeField] private Image[] nextTierUpgradeStars;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI currentRarityText;

    [SerializeField] private Image[] currentTierStars;
    [SerializeField] private Image[] nextTierStars;
    
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

    [SerializeField] private Image activeBlueprintImage;
    [SerializeField] private Image inactiveBlueprintImage;

    [SerializeField] private TextMeshProUGUI activeUpgradePriceText;
    [SerializeField] private TextMeshProUGUI inactiveUpgradePriceText;

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

        UpdateWorkerPanelUI();

        workerPanel.SetActive(true);
        blackBackground.SetActive(true);
    }

    private void UpdateWorkerIcon()
    {
        if (currentWorker == null)
            return;

        workerIcon.sprite = workerIconDatabase.GetFullBodyIcon(
            currentWorker.BusinessType,
            currentWorker.CurrentRarity
        );
    }

    private void UpdateWorkerPanelUI()
    {
        UpdateWorkerIcon();

        UpdateCurrentRarityText();
        UpdateUpgradeVisualState();
        UpdateStarUI(currentWorker);
        UpdateNextTierUpgradeStar();
        UpdateCurrentMaxLvlTxt();
        UpdateNextMaxLvlTxt();
        UpdateCurrentMinCycleDurationText();
        UpdateNextMinCycleDurationText();
        UpdateStarUpgradeButton();
        UpdateBlueprintPriceUI();
    }

    private void UpdateUpgradeVisualState()
    {
        bool canAfford = currentWorker.HasEnoughCurrencyForTierOrRarityUpgrade;

        workerCanUpgradeVisuals.SetActive(canAfford);
        workerNotEnoughCurrencyVisuals.SetActive(!canAfford);
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
        activeBlueprintImage.gameObject.SetActive(visible);
        inactiveBlueprintImage.gameObject.SetActive(visible);

        activeUpgradePriceText.gameObject.SetActive(visible);
        inactiveUpgradePriceText.gameObject.SetActive(visible);
    }

    private void UpdateBlueprintIcon(CurrencyType currencyType)
    {
        Sprite icon = currencyIconDatabase.GetIcon(currencyType);

        activeBlueprintImage.sprite = icon;
        inactiveBlueprintImage.sprite = icon;
    }

    private void UpdateUpgradePriceText(int ownedAmount, int requiredAmount)
    {
        string priceText = $"{ownedAmount}/{requiredAmount}";

        activeUpgradePriceText.text = priceText;
        inactiveUpgradePriceText.text = priceText;
    }

    private void UpdateUpgradePriceTextColor()
    {
        activeUpgradePriceText.color = currentWorker.HasEnoughCurrencyForTierOrRarityUpgrade ? Color.white : Color.red;
    }

    public void OnAddStarButtonClicked()
    {
        if (!currentWorker.CanUpgradeTierOrRarity)
            return;

        Image sourceStar = GetActiveUpgradeStar();
        Image targetStar = GetUpgradeTargetStar();

        if (sourceStar == null || targetStar == null)
            return;

        Vector3 sourcePosition = sourceStar.transform.position;
        Sprite sourceSprite = sourceStar.sprite;

        Color flyingStarColor = colorDatabase.GetColor(
            RaritySystem.GetColorCategory(currentWorker.CurrentRarity)
        );

        Color previousStarColor = targetStar.color;

        currentWorker.UpgradeTierOrRarity();

        UpdateWorkerPanelUI();

        Color upgradedStarColor = targetStar.color;
        targetStar.color = previousStarColor;

        PlayStarUpgradeVFX(
            sourcePosition,
            sourceSprite,
            targetStar,
            upgradedStarColor,
            flyingStarColor
        );
    }

    public void OnAddBPBtnClicked()
    {
        BuildingData university = EntityRegistry.Instance.GetBuilding(BusinessType.University);

        BuildingMainMenuUI.Instance.CloseBuildingPanel();
        ClosePanel();

        if (university != null)
        {
            UniversityUI.Instance.OpenUniversityPanel();
            return;
        }

        BuildShopSystem.Instance.OpenBuildShop(BuildShopCategory.Buildings);

        BuildShopItemUI targetItem = BuildShopSystem.Instance.GetBuildingItem(BusinessType.University);

        if (targetItem != null)
        {
            TutorialHighlightSystem.Instance.Highlight(targetItem.IconAndArrow);

            BuildShopSystem.Instance.ScrollUp(1f);
        }
    }

    private Image GetUpgradeTargetStar()
    {
        int targetIndex = currentWorker.CurrentTier == Tiers.Tier5
            ? 0
            : (int)currentWorker.CurrentTier;

        return currentTierStars[targetIndex];
    }

    private Image GetActiveUpgradeStar()
    {
        foreach (Image star in nextTierUpgradeStars)
        {
            if (star.gameObject.activeSelf)
                return star;
        }

        return null;
    }

    private void PlayStarUpgradeVFX(
    Vector3 sourcePosition,
    Sprite sourceSprite,
    Image targetStar,
    Color targetColor,
    Color flyingStarColor)
    {
        starUpgradeVFX.PlayWorkerUpgrade(
            sourcePosition,
            targetStar,
            sourceSprite,
            targetColor,
            flyingStarColor
        );
    }

    private void HideAllUpgradeStars()
    {
        foreach (Image star in nextTierUpgradeStars)
        {
            star.gameObject.SetActive(false);
        }
    }

    private void UpdateNextTierUpgradeStar()
    {
        HideAllUpgradeStars();

        if (currentWorker.IsMaxProgression)
            return;

        int upgradeStarIndex = currentWorker.CurrentTier == Tiers.Tier5
            ? 0
            : (int)currentWorker.NextTier - 1;

        bool canAfford =
            currentWorker.HasEnoughCurrencyForTierOrRarityUpgrade;

        ColorCategory category = canAfford
            ? ColorCategory.ActiveUpgradeStarColor
            : ColorCategory.InactiveStarColor;

        Image upgradeStar = nextTierUpgradeStars[upgradeStarIndex];

        upgradeStar.color = colorDatabase.GetColor(category);
        upgradeStar.gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        workerPanel.SetActive(false);
        blackBackground.SetActive(false);
    }

    private void UpdateStarUI(WorkerData worker)
    {
        var info = worker.GetStarDisplayInfo();

        int currentTier = (int)info.CurrentTier;
        int nextTier = (int)info.NextTierValue;

        int maxStars = currentTierStars.Length;

        Color activeColor = colorDatabase.GetColor(
            RaritySystem.GetColorCategory(info.CurrentRarity)
        );

        Color inactiveColor = colorDatabase.GetColor(
            ColorCategory.InactiveStarColor
        );

        for (int i = 0; i < maxStars; i++)
        {
            currentTierStars[i].color =
                i < currentTier ? activeColor : inactiveColor;
        }

        if (nextTier > currentTier)
        {
            for (int i = 0; i < maxStars; i++)
            {
                nextTierStars[i].color =
                    i < nextTier ? activeColor : inactiveColor;
            }

            return;
        }

        if (info.NextRarity != info.CurrentRarity)
        {
            Color nextColor = colorDatabase.GetColor(
                RaritySystem.GetColorCategory(info.NextRarity)
            );

            for (int i = 0; i < maxStars; i++)
            {
                nextTierStars[i].color =
                    i == 0 ? nextColor : inactiveColor;
            }

            return;
        }

        for (int i = 0; i < nextTierStars.Length; i++)
        {
            nextTierStars[i].color = inactiveColor;
        }
    }
}