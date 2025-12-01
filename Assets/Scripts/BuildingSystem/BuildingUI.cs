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

    [SerializeField] private GameObject buildingPanel;

    [SerializeField] private Image[] colorStars;

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
        levelText.text = $"{building.LevelOfBuilding} / {building.GetMaxLevel()}";
        //incomeText.text = $"Income: {building.Income}";
        rarityText.text = building.Rarity.ToString();
        //upgradeCostText.text = $"Upgrade cost: {building.PriceToUpgrade}";
        UpdateStarUI(building);

        buildingPanel.SetActive(true);
    }

    public Color GetColorByRarity(Rarities rarity)
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

    public void UpdateStarUI(BuildingData building)
    {
        int tierValue = (int)building.tier;    
        int maxStars = colorStars.Length;      

        Color activeColor = GetColorByRarity(building.Rarity);
        Color inactiveColor = Color.gray;      

        for (int i = 0; i < maxStars; i++)
        {
            colorStars[i].color = (i < tierValue) ? activeColor : inactiveColor;
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