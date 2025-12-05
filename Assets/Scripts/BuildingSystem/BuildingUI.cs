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
        levelText.text = $"{building.LevelOfBuilding} / {building.GetMaxLevel()}";
        //incomeText.text = $"Income: {building.Income}";
        rarityText.text = building.Rarity.ToString();
        //upgradeCostText.text = $"Upgrade cost: {building.PriceToUpgrade}";
        UpdateStarUI(building);

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

    private Rarities GetNextRarity(Rarities current)
    {
        switch (current)
        {
            case Rarities.Primitive: return Rarities.Developed;
            case Rarities.Developed: return Rarities.Industrial;
            case Rarities.Industrial: return Rarities.Modern;
            case Rarities.Modern: return Rarities.Futuristic;
            case Rarities.Futuristic: return Rarities.Futuristic; 
            default: return current;
        }
    }

    private void UpdateStarUI(BuildingData building)
    {
        int tierValue = (int)building.tier;
        Debug.Log(tierValue);
        int maxStars = colorStars.Length;
        Debug.Log(maxStars);
        Color activeColor = GetColorByRarity(building.Rarity);
        Color inactiveColor = Color.grey;      

        for (int i = 0; i < maxStars; i++)
        {
            Debug.Log(i);
            colorStars[i].color = (i < tierValue) ? activeColor : inactiveColor;
        }

        if (tierValue < maxStars)
        {
            int nextTier = tierValue + 1;
            for (int i = 0; i < maxStars; i++)
            {
                nextTierColorStars[i].color = (i < nextTier) ? activeColor : inactiveColor;
            }
        }

        else
        {
            Rarities nextRarity = GetNextRarity(building.Rarity);

            if (nextRarity == building.Rarity)
            {
                for (int i = 0; i < maxStars; i++)
                    nextTierColorStars[i].color = inactiveColor;

                return;
            }

            Color nextRarityColor = GetColorByRarity(nextRarity);

            for (int i = 0; i < maxStars; i++)
            {
                nextTierColorStars[i].color = (i == 0) ? nextRarityColor : inactiveColor;
            }
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