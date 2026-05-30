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

    [SerializeField] private TextMeshProUGUI CurrentMaxIncomeText;
    [SerializeField] private TextMeshProUGUI NextMaxIncomeText;

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

        UpdateBuildingPanelUI();

        buildingIcon.sprite = currentBuilding.Icon; // later put into  UpdateUI()

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
        CurrentMaxIncomeText.text = currentBuilding.CurrentProgressionMaxIncome.ToString();
    }

    private void UpdateNextMaxIncomeText()
    {
        NextMaxIncomeText.text = currentBuilding.NextProgressionMaxIncome.ToString();
    }

    private void UpdateBlueprintPriceUI()
    {
        var requirements = currentBuilding.GetBlueprintRequirementsForNextUpgrade();

        int index = 0;

        foreach (var req in requirements)
        {
            blueprintSlots[index].SetActive(true);

            CurrencyType blueprint = req.Key;

            int owned = CurrencySystem.GetCurrencyAmount(blueprint);

            int required = req.Value;

            upgradePriceTexts[index].text = $"{owned}/{required}";

            upgradePriceTexts[index].color = owned >= required ? Color.white : Color.red;

            index++;
        }

        for (; index < blueprintSlots.Length; index++)
        {
            blueprintSlots[index].SetActive(false);
        }
    }

    private void UpdateStarUpgradeButton()
    {
        bool canUpgrade = currentBuilding.CanUpgradeTierOrRarity();

        addStarButton.interactable = canUpgrade;      
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