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

    [SerializeField] private GameObject buildingPanel;

    private BuildingData currentBuilding;

    private void Awake()
    {
        Instance = this;
        buildingPanel.SetActive(false);
    }

    public void OpenBuildingPanel(BuildingData building)
    {
        currentBuilding = building;

        //buildingIcon.sprite = building.Icon;
        //buildingNameText.text = building.Name;
        //levelText.text = $"Level: {building.LevelOfBuilding}";
        //incomeText.text = $"Income: {building.Income}";
        //rarityText.text = $"Tier: {building.Rarity}";
        //upgradeCostText.text = $"Upgrade cost: {building.PriceToUpgrade}";

        buildingPanel.SetActive(true);
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