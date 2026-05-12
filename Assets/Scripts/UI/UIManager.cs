using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private BuildingUpgradeUIState currentUpgradeState;

    [Header("UI Elements")]
    public GameObject buildingPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI priceText;
    public Image iconImage;
    public Button upgradeButton;
    public TextMeshProUGUI workerLvlNeededText;
    public TextMeshProUGUI automationStatusText;
    public TextMeshProUGUI IsWorkerAssignedText;
    public TextMeshProUGUI WorkerBonusStatusText;

    [SerializeField] private Image[] colorStars;
    [SerializeField] private GameObject needAddStarPanel;

    [Header("Building UI")]
    [SerializeField] private Button buildingButton;

    public BuildingPlaceable Placeable { get; private set; }

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
            Debug.Log($"[UIManager] worker has been upgraded to lvl {evt.Worker.level}. UI updated.");
        }
    }
    private void OnBuildingTierOrRarityChanged(BuildingTierOrRarityChangedEvent evt)
    {
        if (currentBuilding != evt.Building)
            return;

        EvaluateUpgradeState();
        UpdateStarUI(currentBuilding);
        UpdateUIForUpgrade();

        levelText.text =
            $"Lv: {currentBuilding.LevelOfBuilding} / " +
            $"{currentBuilding.CurrentTierMaxLevel}";
    }

    public void OpenMainBuildingPanel(BuildingData building)
    {
        currentBuilding = building;

        nameText.text = building.Name;

        EvaluateUpgradeState();

        priceText.text = building.PriceToUpgrade.ToString();
        iconImage.sprite = building.Icon;

        buildingPanel.SetActive(true);

        UpdateUIForUpgrade();
        UpdateStarUI(building);
        UpdateWorkerAndAutomationUI(building);

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
        currentUpgradeState = state;

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
        needAddStarPanel.SetActive(false);

        levelText.text =
            $"Lv: {currentBuilding.LevelOfBuilding} / " +
            $"<color=red>{currentBuilding.CurrentTierMaxLevel}</color>";

        UpdateUIForUpgrade(); 
    }

    private void ApplyNeedTierUpgradeUI()
    {
        needAddStarPanel.SetActive(true);

        levelText.text =
            $"Lv: <color=red>{currentBuilding.LevelOfBuilding} / " +
            $"{currentBuilding.CurrentTierMaxLevel}</color>";

        upgradeButton.interactable = false;
        priceText.color = Color.gray;
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
        Debug.Log(tierValue);
        int maxStars = colorStars.Length;
        Debug.Log(maxStars);
        Color activeColor = GetColorByRarity(building.CurrentRarity);
        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
        {
            Debug.Log(i);
            colorStars[i].color = (i < tierValue) ? activeColor : inactiveColor;
        }
    }

    public void CloseBuildingPanel()
    {
        buildingPanel.SetActive(false);
    }

    public void UpdateUIForUpgrade()
    {
        bool canUpgrade = CurrencySystem.Instance.IsEnoughMoneyForUpgrade(currentBuilding.Currency, currentBuilding.PriceToUpgrade);

        if (canUpgrade && currentBuilding.LevelOfBuilding < currentBuilding.CurrentTierMaxLevel)
        {
            priceText.color = Color.white;
            upgradeButton.interactable = true;
        }
        else
        {           
            priceText.color = Color.red;
            upgradeButton.interactable = false;
        }
    }

    public void UpgradeBuilding()
    {
        currentBuilding.UpgradeBuilding();

        currentBuilding.PriceToUpgrade += 5;

        EvaluateUpgradeState();

        priceText.text = currentBuilding.PriceToUpgrade.ToString();
        EventManager.Instance.QueueEvent(new XPAddedEvent(currentBuilding.LevelOfBuilding - 1));
        UpdateUIForUpgrade();
    }

    private void UpdateWorkerAndAutomationUI(BuildingData building)
    {
        var placeable = building.Placeable;

        if (placeable != null && placeable.HasWorker())
        {
            IsWorkerAssignedText.text = "Worker assigned";
            WorkerBonusStatusText.text = "ON";
        }
        else
        {
            IsWorkerAssignedText.text = "No worker assigned";
            WorkerBonusStatusText.text = "OFF";
            automationStatusText.text = "OFF";
            return; 
        }

        if (building.IsAutomated)
        {
            automationStatusText.text = "ON";
        }
    }

    private void OnAutomationChanged(BuildingAutomationChangedEvent evt)
    {
        if (currentBuilding == evt.Building)
        {
            UpdateWorkerAndAutomationUI(currentBuilding);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}