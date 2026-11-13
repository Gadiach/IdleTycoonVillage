using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance;

    [Header("Colors")]
    [SerializeField] private ColorDatabase colorDatabase;

    [Header("Upgrade Visual States")]
    [SerializeField] private GameObject buildingCanUpgradeVisuals;
    [SerializeField] private GameObject buildingNotEnoughCurrencyVisuals;

    [SerializeField] private Image[] nextTierUpgradeStars;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI currentRarityText;

    [SerializeField] private Image[] currentTierStars;
    [SerializeField] private Image[] nextTierStars;

    [SerializeField] private StarUpgradeVFX starUpgradeVFX;

    [SerializeField] private Image buildingIcon;

    [SerializeField] private TextMeshProUGUI currentMaxLvlTxt;
    [SerializeField] private TextMeshProUGUI nextMaxLvlTxt;

    [SerializeField] private TextMeshProUGUI currentMaxIncomeText;
    [SerializeField] private TextMeshProUGUI nextMaxIncomeText;

    [SerializeField] private Sprite addStarButtonActiveSprite;
    [SerializeField] private Sprite addStarButtonInactiveSprite;

    [SerializeField] private Button addStarButton;

    [SerializeField] private GameObject buildingPanel;

    [SerializeField] private Image activeBlueprintImage;
    [SerializeField] private Image inactiveBlueprintImage;

    [SerializeField] private TextMeshProUGUI activeUpgradePriceText;
    [SerializeField] private TextMeshProUGUI inactiveUpgradePriceText;
    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;

    [SerializeField] private GameObject blackBackground;

    private BuildingData currentBuilding;

    private void Awake()
    {
        Instance = this;
        buildingPanel.SetActive(false);
        blackBackground.SetActive(false);
        HideAllUpgradeStars();
    }

    public void OpenBuildingPanel(BuildingData building)
    {
        currentBuilding = building;

        buildingIcon.sprite = currentBuilding.Icon;

        UpdateBuildingPanelUI();

        buildingPanel.SetActive(true);

        blackBackground.SetActive(true);
    }



    private void UpdateBuildingPanelUI()
    {
        UpdateCurrentRarityText();

        UpdateUpgradeVisualState();

        UpdateStarUI(currentBuilding);

        UpdateNextTierUpgradeStar();

        UpdatecurrentMaxLvlTxt();
        UpdateNextMaxLvlTxt();

        UpdateCurrentMaxIncomeText();
        UpdateNextMaxIncomeText();

        UpdateStarUpgradeButton();

        UpdateBlueprintPriceUI();
    }

    private void UpdateUpgradeVisualState()
    {
        bool canAfford = currentBuilding.HasEnoughCurrencyForTierOrRarityUpgrade;

        buildingCanUpgradeVisuals.SetActive(canAfford);
        buildingNotEnoughCurrencyVisuals.SetActive(!canAfford);
    }

    private void UpdateCurrentRarityText()
    {
        currentRarityText.text = currentBuilding.CurrentRarity.ToString();
    }

    private void UpdatecurrentMaxLvlTxt()
    {
        currentMaxLvlTxt.text = currentBuilding.CurrentProgressionMaxLevel.ToString();
    }

    private void UpdateNextMaxLvlTxt()
    {
        nextMaxLvlTxt.text = currentBuilding.NextProgressionMaxLevel.ToString();
    }

    private void UpdateCurrentMaxIncomeText()
    {
        currentMaxIncomeText.text = currentBuilding.CurrentProgressionMaxIncome.ToString();
    }

    private void UpdateNextMaxIncomeText()
    {
        nextMaxIncomeText.text = currentBuilding.NextProgressionMaxIncome.ToString();
    }

    private void UpdateBlueprintPriceUI()
    {
        if (currentBuilding.IsMaxProgression)
        {
            SetBlueprintRequirementVisible(false);
            return;
        }

        UpdateBlueprintRequirement(currentBuilding.TierOrRarityUpgradeCurrency,currentBuilding.PriceToUpgradeTierOrRarity);
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

    public void OnAddBPBtnClicked()
    {
        BuildingMainMenuUI.Instance.CloseBuildingPanel();
        ClosePanel();

        UniversityUI.Instance.TryOpenUniversity();
    }

    private void UpdateUpgradePriceTextColor()
    {
        activeUpgradePriceText.color = currentBuilding.HasEnoughCurrencyForTierOrRarityUpgrade ? Color.white : Color.red;
    }

    private void UpdateStarUpgradeButton()
    {
        SetAddStarButtonState(currentBuilding.CanUpgradeTierOrRarity);
    }

    public void OnAddStarButtonClicked()
    {
        if (!currentBuilding.CanUpgradeTierOrRarity)
            return;

        Image sourceStar = GetActiveUpgradeStar();
        Image targetStar = GetUpgradeTargetStar();

        if (sourceStar == null || targetStar == null)
            return;

        Vector3 sourcePosition = sourceStar.transform.position;
        Sprite sourceSprite = sourceStar.sprite;

        Color flyingStarColor = colorDatabase.GetColor(
            RaritySystem.GetColorCategory(currentBuilding.CurrentRarity)
        );

        Color previousStarColor = targetStar.color;

        currentBuilding.UpgradeTierOrRarity();

        UpdateBuildingPanelUI();

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

    private Image GetUpgradeTargetStar()
    {
        int targetIndex = currentBuilding.CurrentTier == Tiers.Tier5 ? 0 : (int)currentBuilding.CurrentTier;

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
        starUpgradeVFX.PlayBuildingUpgrade(
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

        if (currentBuilding.IsMaxProgression)
            return;

        int upgradeStarIndex = currentBuilding.CurrentTier == Tiers.Tier5 ? 0 : (int)currentBuilding.NextTier - 1;

        bool canAfford = currentBuilding.HasEnoughCurrencyForTierOrRarityUpgrade;

        ColorCategory category = canAfford ? ColorCategory.ActiveUpgradeStarColor : ColorCategory.InactiveStarColor;

        Image upgradeStar = nextTierUpgradeStars[upgradeStarIndex];

        upgradeStar.color = colorDatabase.GetColor(category);
        upgradeStar.gameObject.SetActive(true);
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

    private void UpdateStarUI(BuildingData building)
    {
        var info = building.GetStarDisplayInfo();

        int currentTier = (int)info.CurrentTier;
        int nextTier = (int)info.NextTierValue;

        int maxStars = currentTierStars.Length;

        Color activeColor = colorDatabase.GetColor(RaritySystem.GetColorCategory(info.CurrentRarity));

        Color inactiveColor = colorDatabase.GetColor(ColorCategory.InactiveStarColor);

        for (int i = 0; i < maxStars; i++)
        {
            currentTierStars[i].color = i < currentTier ? activeColor : inactiveColor;
        }

        if (nextTier > currentTier)
        {
            for (int i = 0; i < maxStars; i++)
            {
                nextTierStars[i].color = i < nextTier ? activeColor : inactiveColor;
            }

            return;
        }

        if (info.NextRarity != info.CurrentRarity)
        {
            Color nextColor = colorDatabase.GetColor(RaritySystem.GetColorCategory(info.NextRarity));

            for (int i = 0; i < maxStars; i++)
            {
                nextTierStars[i].color = i == 0 ? nextColor : inactiveColor;
            }

            return;
        }
    }

    public void UpgradeBuilding()
    {
        currentBuilding.UpgradeBuildingLvl();
        
    }

    public void ClosePanel()
    {
        buildingPanel.SetActive(false);
        blackBackground.SetActive(false);
    }
}