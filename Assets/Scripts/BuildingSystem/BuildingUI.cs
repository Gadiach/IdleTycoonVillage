using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI currentRarityText;

    [SerializeField] private Image[] currentTierRarityStarsColor;
    [SerializeField] private Image[] nextTierRarityStarsColor;
    [SerializeField] private GameObject[] nextTierUpgradeStars;
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

    [SerializeField] private Image blueprintImage;
    [SerializeField] private TextMeshProUGUI upgradePriceText;
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

        UpdateStarUI(currentBuilding);

        UpdateNextTierUpgradeStar();

        UpdatecurrentMaxLvlTxt();

        UpdateNextMaxLvlTxt();

        UpdateCurrentMaxIncomeText();

        UpdateNextMaxIncomeText();

        UpdateStarUpgradeButton();

        UpdateBlueprintPriceUI();
    }

    private Color GetColorByRarity(Rarities rarity)
    {
        return rarity switch
        {
            Rarities.Primitive => Color.blue,
            Rarities.Developed => Color.green,
            Rarities.Industrial => Color.yellow,
            Rarities.Modern => new Color(0.5f, 0, 1),   
            Rarities.Futuristic => Color.red, 
            _ => Color.grey
        };
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
        var requirements = currentBuilding.BlueprintRequirementsForNextUpgrade;

        if (requirements.Count == 0)
        {
            SetBlueprintRequirementVisible(false);
            return;
        }

        foreach (var requirement in requirements)
        {
            UpdateBlueprintRequirement(requirement.Key, requirement.Value);
            break;
        }
    }

    private void UpdateBlueprintRequirement(
        CurrencyType currencyType,
        int requiredAmount)
    {
        int ownedAmount = CurrencySystem.GetCurrencyAmount(currencyType);

        UpdateBlueprintIcon(currencyType);
        UpdateUpgradePriceText(ownedAmount, requiredAmount);
        UpdateUpgradePriceTextColor(ownedAmount, requiredAmount);

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

    private void UpdateUpgradePriceTextColor(int ownedAmount,int requiredAmount)
    {
        upgradePriceText.color = ownedAmount >= requiredAmount ? Color.white : Color.red;
    }

    private void UpdateStarUpgradeButton()
    {
        SetAddStarButtonState(currentBuilding.CanUpgradeTierOrRarity);
    }

    public void OnAddStarButtonClicked()
    {
        if (!currentBuilding.CanUpgradeTierOrRarity)
            return;

        GameObject sourceStar = GetActiveUpgradeStar();
        Image targetStar = GetUpgradeTargetStar();

        Vector3 sourcePosition = sourceStar.transform.position;
        Sprite sourceSprite = sourceStar.GetComponent<Image>().sprite;

        Color previousStarColor = targetStar.color;

        currentBuilding.UpgradeTierOrRarity();

        UpdateBuildingPanelUI();

        Color upgradedStarColor = targetStar.color;
        targetStar.color = previousStarColor;

        PlayStarUpgradeVFX(
            sourcePosition,
            sourceSprite,
            targetStar,
            upgradedStarColor
        );
    }

    private Image GetUpgradeTargetStar()
    {
        int targetIndex = currentBuilding.CurrentTier == Tiers.Tier5
            ? 0
            : (int)currentBuilding.CurrentTier;

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

    private void PlayStarUpgradeVFX(
        Vector3 sourcePosition,
        Sprite sourceSprite,
        Image targetStar,
        Color targetColor)
    {
        starUpgradeVFX.PlayBuildingUpgrade(
            sourcePosition,
            targetStar,
            sourceSprite,
            targetColor
        );
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

        if (currentBuilding.IsMaxProgression)
            return;

        int upgradeStarIndex;

        if (currentBuilding.CurrentTier == Tiers.Tier5)
        {
            upgradeStarIndex = 0;
        }
        else
        {
            upgradeStarIndex = (int)currentBuilding.NextTier - 1;
        }

        nextTierUpgradeStars[upgradeStarIndex].SetActive(true);
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

        int maxStars = currentTierRarityStarsColor.Length;

        Color activeColor = GetColorByRarity(info.CurrentRarity);
        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
            currentTierRarityStarsColor[i].color = (i < currentTier) ? activeColor : inactiveColor;

        if (nextTier > currentTier)
        {
            for (int i = 0; i < maxStars; i++)
                nextTierRarityStarsColor[i].color = (i < nextTier) ? activeColor : inactiveColor;

            return;
        }

        if (info.NextRarity != info.CurrentRarity)
        {
            Color nextColor = GetColorByRarity(info.NextRarity);

            for (int i = 0; i < maxStars; i++)
                nextTierRarityStarsColor[i].color = (i == 0) ? nextColor : inactiveColor;

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