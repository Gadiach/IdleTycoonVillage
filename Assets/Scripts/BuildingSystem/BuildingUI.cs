using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance;

    [Header("UI Elements")]
    [SerializeField] private Image buildingIcon;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI incomeText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI[] upgradePriceTexts;

    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI businessType;
    [SerializeField] private TextMeshProUGUI currentTierMaxLvlTxt;
    [SerializeField] private TextMeshProUGUI nextTierMaxLvlTxt;

    [SerializeField] private GameObject buildingPanel;

    [SerializeField] private Image[] colorStars;
    [SerializeField] private Image[] nextTierColorStars;


    private BuildingData currentBuilding;

    private void Awake()
    {
        Instance = this;
        buildingPanel.SetActive(false);
    }

    public void OpenBuildingPanel(BuildingData building)
    {
        currentBuilding = building;
        buildingIcon.sprite = currentBuilding.Icon;
        buildingNameText.text = building.Name;
        businessType.text = currentBuilding.BusinessType.ToString();
        levelText.text = $"{building.CurrentLevel}";
        //incomeText.text = $"Income: {building.Income}";
        rarityText.text = building.CurrentRarity.ToString();
        //upgradeCostText.text = $"Upgrade cost: {building.PriceToUpgrade}";
        currentTierMaxLvlTxt.text = currentBuilding.CurrentTierMaxLevel.ToString();
        nextTierMaxLvlTxt.text = currentBuilding.NextTierMaxLevel.ToString() ;
        UpdateStarUI(building);
        UpdateStarUpgradeButton();
        UpdateStarUpgradeCostText();
        var requirements = currentBuilding.GetBlueprintRequirementsForNextUpgrade();

        buildingPanel.SetActive(true);
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

    private void UpdateStarUpgradeButton()
    {
        bool canUpgrade = currentBuilding.CanUpgradeTierOrRarity();

        upgradeButton.interactable = canUpgrade;
        upgradePriceTexts[0].color = canUpgrade ? Color.white : Color.red;       
    }

    public void OnAddStarButtonClicked()
    {
        currentBuilding.UpgradeTierOrRarity();

        UpdateStarUI(currentBuilding);
        UpdateStarUpgradeCostText();
        UpdateStarUpgradeButton();

        currentTierMaxLvlTxt.text = currentBuilding.CurrentTierMaxLevel.ToString();
        nextTierMaxLvlTxt.text = currentBuilding.NextTierMaxLevel.ToString();
    }

    private void UpdateStarUpgradeCostText()
    {
        var requirements = currentBuilding.GetBlueprintRequirementsForNextUpgrade();

        int currentRarityIndex = (int)currentBuilding.CurrentRarity;

        for (int r = 0; r < upgradePriceTexts.Length; r++)
        {
            var text = upgradePriceTexts[r];

            if (r > currentRarityIndex)
            {
                text.gameObject.SetActive(false);
                continue;
            }

            text.gameObject.SetActive(true);

            Rarities rarity = (Rarities)r;
            CurrencyType blueprint =
                CurrencyHelper.GetBlueprintCurrency(rarity);

            int owned = CurrencySystem.GetCurrencyAmount(blueprint);
            int required = requirements.TryGetValue(blueprint, out int val)
                ? val
                : 0;

            text.text = $"{owned}/{required}";
            text.color = owned >= required ? Color.white : Color.red;
        }
    }

    private void UpdateStarUI(BuildingData building)
    {
        var info = building.GetStarDisplayInfo();

        int currentTier = (int)info.CurrentTier;
        int nextTier = (int)info.NextTierValue;

        int maxStars = colorStars.Length;

        Color activeColor = GetColorByRarity(info.CurrentRarity);
        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
            colorStars[i].color = (i < currentTier) ? activeColor : inactiveColor;

        if (nextTier > currentTier)
        {
            for (int i = 0; i < maxStars; i++)
                nextTierColorStars[i].color = (i < nextTier) ? activeColor : inactiveColor;

            return;
        }

        if (info.NextRarity != info.CurrentRarity)
        {
            Color nextColor = GetColorByRarity(info.NextRarity);

            for (int i = 0; i < maxStars; i++)
                nextTierColorStars[i].color = (i == 0) ? nextColor : inactiveColor;

            return;
        }
    }

    public void UpgradeBuilding()
    {
        currentBuilding.UpgradeBuilding();
        UpdateUI();
    }

    private void UpdateUI()
    {
        levelText.text = $"Level: {currentBuilding.CurrentLevel}";
        incomeText.text = $"Income: {currentBuilding.IncomePerCycle}";
        //upgradePriceText.text = $"Upgrade cost: {currentBuilding.PriceToUpgrade}";
    }

    public void ClosePanel()
    {
        buildingPanel.SetActive(false);
    }
}