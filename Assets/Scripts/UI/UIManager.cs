using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Common UI Elements")]
    public GameObject buildingPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI automationStatusText;
    [SerializeField] private Image[] colorStars;

    [Header("Building UI Elements")]

    public TextMeshProUGUI BuildinglevelText;
    public TextMeshProUGUI BuildingUpgradePriceText;
    public Image BuildingImage;
    public Button BuildingUpgradeButton;

    [Header("Worker UI Elements")]

    public TextMeshProUGUI WorkerCurrentlevelText;
    public TextMeshProUGUI WorkerUpgradePriceText;

    [Header("Building UI")]
    [SerializeField] private Button buildingButton;

    private BuildingData currentBuilding;
    

    private void Awake()
    {
        Instance = this;
        buildingPanel.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
        EventManager.Instance.AddListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
        EventManager.Instance.AddListener<BuildingTierOrRarityChangedEvent>(OnBuildingTierOrRarityChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
        EventManager.Instance.RemoveListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
        EventManager.Instance.RemoveListener<BuildingTierOrRarityChangedEvent>(OnBuildingTierOrRarityChanged);
    }

    private void OnWorkerUpgraded(WorkerUpgradedEvent evt)
    {
        currentBuilding = evt.Worker.AssignedBuilding;

        if (currentBuilding == null || currentBuilding.Placeable == null) return;

        WorkerData assignedWorker = currentBuilding.Placeable.GetAssignedWorker();

        if (assignedWorker == evt.Worker)
        {
            currentBuilding.CheckAutomationState();
        }
    }
    private void OnBuildingTierOrRarityChanged(BuildingTierOrRarityChangedEvent evt)
    {
        if (currentBuilding != evt.Building)
            return;

        EvaluateUpgradeState();
        UpdateStarUI(currentBuilding);
    }

    public void OpenMainBuildingPanel(BuildingData building)
    {
        currentBuilding = building;

        nameText.text = building.Name;

        BuildingUpgradePriceText.text = building.PriceToUpgrade.ToString();
        BuildingImage.sprite = building.Icon;

        EvaluateUpgradeState();

        buildingPanel.SetActive(true);

        UpdateStarUI(building);
        UpdateAutomationUI(building);

        buildingButton.onClick.RemoveAllListeners();
        buildingButton.onClick.AddListener(() =>
        {
            BuildingUI.Instance.OpenBuildingPanel(building);
        });
    }

    private void EvaluateUpgradeState()
    {
        if (currentBuilding.LevelOfBuilding >= currentBuilding.CurrentTierMaxLevel)
        {
            SetUpgradeState(BuildingUpgradeUIState.NeedTierUpgrade);
        }
        else
        {
            SetUpgradeState(BuildingUpgradeUIState.CanUpgradeLevel);
        }
    }

    private void SetUpgradeState(BuildingUpgradeUIState state)
    {
        switch (state)
        {
            case BuildingUpgradeUIState.CanUpgradeLevel:
                ApplyCanUpgradeLevelUI();
                break;

            case BuildingUpgradeUIState.NeedTierUpgrade:
                ApplyNeedTierUpgradeUI();
                break;
        }
    }

    private void ApplyCanUpgradeLevelUI()
    {
        SetBuildingLevelTextWithRedMaxLevel();

        bool canAfford = CanAffordUpgrade();

        BuildingUpgradePriceText.color =
            canAfford ? Color.white : Color.red;

        BuildingUpgradeButton.interactable =
            canAfford;
    }

    private void SetBuildingLevelTextWithRedMaxLevel()
    {
        BuildinglevelText.text =
            $"Lv: {currentBuilding.LevelOfBuilding} / " +
            $"<color=red>{currentBuilding.CurrentTierMaxLevel}</color>";
    }

    private void ApplyNeedTierUpgradeUI()
    {
        SetBuildingLevelTextMaxed();

        BuildingUpgradeButton.interactable = false;
        BuildingUpgradePriceText.color = Color.gray;

    }

    private void SetBuildingLevelTextMaxed()
    {
        BuildinglevelText.text =
            $"Lv: <color=red>{currentBuilding.LevelOfBuilding} / " +
            $"{currentBuilding.CurrentTierMaxLevel}</color>";
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
        int tierValue = (int)building.CurrentTier;
        int maxStars = colorStars.Length;
        Color activeColor = GetColorByRarity(building.CurrentRarity);
        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
        {
            colorStars[i].color = (i < tierValue) ? activeColor : inactiveColor;
        }
    }

    public void CloseBuildingPanel()
    {
        buildingPanel.SetActive(false);
    }

    public void UpgradeBuilding()
    {
        currentBuilding.UpgradeBuilding();

        currentBuilding.PriceToUpgrade += 5;

        EvaluateUpgradeState();

        BuildingUpgradePriceText.text = currentBuilding.PriceToUpgrade.ToString();
        EventManager.Instance.QueueEvent(new XPAddedEvent(currentBuilding.LevelOfBuilding - 1));
    }

    private void UpdateAutomationUI(BuildingData building)
    {
        if (building.IsAutomated)
        {
            automationStatusText.text = "ON";
        }
        else
        {
            automationStatusText.text = "OFF";
        }
    }

    private void OnAutomationChanged(BuildingAutomationChangedEvent evt)
    {
        if (currentBuilding == evt.Building)
        {
            UpdateAutomationUI(currentBuilding);
        }
    }

    private bool CanAffordUpgrade()
    {
        return CurrencySystem.Instance.IsEnoughMoneyForUpgrade(
            currentBuilding.Currency,
            currentBuilding.PriceToUpgrade);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}