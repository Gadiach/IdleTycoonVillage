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

    [SerializeField] private Image buildingIcon;

    [SerializeField] private TextMeshProUGUI currentMaxLvlTxt;
    [SerializeField] private TextMeshProUGUI nextMaxLvlTxt;

    [SerializeField] private TextMeshProUGUI currentMaxIncomeText;
    [SerializeField] private TextMeshProUGUI nextMaxIncomeText;

    [SerializeField] private Button addStarButton;

    [SerializeField] private GameObject buildingPanel;

    [SerializeField] private GameObject[] blueprintSlots;
    [SerializeField] private TextMeshProUGUI[] upgradePriceTexts;

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

        UpdateBuildingPanelUI();

        buildingPanel.SetActive(true);
    }

    private void UpdateBuildingPanelUI()
    {
        UpdateCurrentRarityText();

        UpdateStarUI(currentBuilding);

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

        int index = 0;

        foreach (var requirement in requirements)
        {
            blueprintSlots[index].SetActive(true);

            int owned = CurrencySystem.GetCurrencyAmount(requirement.Key);

            upgradePriceTexts[index].text = $"{owned}/{requirement.Value}";

            upgradePriceTexts[index].color = owned >= requirement.Value ? Color.white : Color.red;

            index++;
        }

        for (int i = index; i < blueprintSlots.Length; i++)
        {
            blueprintSlots[i].SetActive(false);
        }
    }

    private void UpdateStarUpgradeButton()
    {
        addStarButton.interactable = currentBuilding.CanUpgradeTierOrRarity;
    }

    public void OnAddStarButtonClicked()
    {
        currentBuilding.UpgradeTierOrRarity();

        UpdateBuildingPanelUI();
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
    }
}