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
    [SerializeField] private TextMeshProUGUI upgradeCostText;
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
        levelText.text = $"{building.LevelOfBuilding}";
        //incomeText.text = $"Income: {building.Income}";
        rarityText.text = building.CurrentRarity.ToString();
        //upgradeCostText.text = $"Upgrade cost: {building.PriceToUpgrade}";
        currentTierMaxLvlTxt.text = currentBuilding.CurrentTierMaxLevel.ToString();
        nextTierMaxLvlTxt.text = currentBuilding.NextTierMaxLevel.ToString() ;
        UpdateStarUI(building);
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
        levelText.text = $"Level: {currentBuilding.LevelOfBuilding}";
        incomeText.text = $"Income: {currentBuilding.Income}";
        upgradeCostText.text = $"Upgrade cost: {currentBuilding.PriceToUpgrade}";
    }

    public void ClosePanel()
    {
        buildingPanel.SetActive(false);
    }
}