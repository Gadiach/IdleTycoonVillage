using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Common UI Elements")]
    public GameObject buildingPanel;
    public TextMeshProUGUI AutomationStatusText;
    public TextMeshProUGUI IncomeText;
    public TextMeshProUGUI TimeText;
    [SerializeField] private GameObject workerPanel;
    [SerializeField] private GameObject noWorkerPanel;

    [Header("Building UI Elements")]

    public TextMeshProUGUI BuildingLevelText;
    public TextMeshProUGUI BuildingUpgradePriceText;
    public Image BuildingImage;
    public Button BuildingUpgradeButton;
    [SerializeField] private Image[] colorStarsBuilding;

    [Header("Worker UI Elements")]

    public TextMeshProUGUI WorkerLevelText;             
    public TextMeshProUGUI WorkerUpgradePriceText;      
    public Image WorkerImage;                           
    public Button WorkerUpgradeButton;                  
    [SerializeField] private Image[] colorStarsWorker;  

    [Header("Building UI")]
    [SerializeField] private Button buildingButton;

    private BuildingData currentBuilding;
    private WorkerData currentWorker;


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

        EvaluateBuildingUpgradeState();
        UpdateBuildingStarUI(currentBuilding);
    }

    public void OpenWorkerShop()
    {
        buildingPanel.SetActive(false);

        ShopManager.current.OpenShop(ObjectType.Workers);
    }

    public void OpenMainBuildingPanel(BuildingData building)
    {
        currentBuilding = building;
        currentWorker = building.Placeable.GetAssignedWorker();

        UpdateIncomeText();
        UpdateTimeText();

        BuildingUpgradePriceText.text = building.PriceToUpgrade.ToString();
        BuildingImage.sprite = building.Icon;
        EvaluateBuildingUpgradeState();

        if (HasWorker())
        {
            workerPanel.SetActive(true);
            noWorkerPanel.SetActive(false);

            UpdateWorkerUpgradePriceText();
            UpdateWorkerImage();
            EvaluateWorkerUpgradeState();

            UpdateWorkerStarUI(currentWorker);
        }
        else
        {
            workerPanel.SetActive(false);
            noWorkerPanel.SetActive(true);
        }
                                                                                

        buildingPanel.SetActive(true);

        UpdateBuildingStarUI(building);                                    

        UpdateAutomationUI(building);

        buildingButton.onClick.RemoveAllListeners();
        buildingButton.onClick.AddListener(() =>
        {
            BuildingUI.Instance.OpenBuildingPanel(building);
        });
    }

    private void UpdateWorkerImage()
    {
        WorkerImage.sprite = currentWorker.Icon;
    }

    private void UpdateWorkerUpgradePriceText()
    {
        WorkerUpgradePriceText.text =
            currentWorker.PriceToUpgrade.ToString();
    }

    private void UpdateIncomeText()
    {
        if (currentWorker == null)
        {
            IncomeText.text = "Income: --";
            return;
        }

        IncomeText.text = $"Income: {currentBuilding.IncomePerCycle}";
    }

    private void UpdateTimeText()
    {
        if (currentWorker == null)
        {
            TimeText.text = "Time: --";
            return;
        }

        TimeText.text = $"Time: {currentBuilding.ProductionDuration:F1}s";
    }

    private void EvaluateBuildingUpgradeState()
    {
        if (currentBuilding.CurrentLevel >= currentBuilding.CurrentTierMaxLevel)
        {
            SetBuildingUpgradeState(UpgradeUIState.NeedTierUpgrade);
        }
        else
        {
            SetBuildingUpgradeState(UpgradeUIState.CanUpgradeLevel);
        }
    }

    private void EvaluateWorkerUpgradeState()
    {
        if (currentWorker.CurrentLevel >= currentWorker.CurrentTierMaxLevel)
        {
            SetWorkerUpgradeState(UpgradeUIState.NeedTierUpgrade);
        }
        else
        {
            SetWorkerUpgradeState(UpgradeUIState.CanUpgradeLevel);
        }
    }

    private void SetWorkerUpgradeState (UpgradeUIState state)
    {
        switch (state)
        {
            case UpgradeUIState.CanUpgradeLevel:
                ApplyCanUpgradeWorkerLevelUI();
                break;

            case UpgradeUIState.NeedTierUpgrade:
                ApplyNeedWorkerTierUpgradeUI();
                break;
        }
    }

    private void ApplyNeedWorkerTierUpgradeUI()
    {
        SetWorkerLevelTextMaxed();

        WorkerUpgradeButton.interactable = false;
        WorkerUpgradePriceText.color = Color.gray;

    }

    private void ApplyCanUpgradeWorkerLevelUI()
    {
        SetWorkerLevelTextWithRedMaxLevel();

        bool canAfford = CanAffordWorkerUpgrade();

        WorkerUpgradePriceText.color =
            canAfford ? Color.white : Color.red;

        WorkerUpgradeButton.interactable =
            canAfford;
    }

    private void SetBuildingUpgradeState(UpgradeUIState state)
    {
        switch (state)
        {
            case UpgradeUIState.CanUpgradeLevel:
                ApplyCanUpgradeBuildingLevelUI();
                break;

            case UpgradeUIState.NeedTierUpgrade:
                ApplyNeedBuildingTierUpgradeUI();
                break;
        }
    }

    private void ApplyCanUpgradeBuildingLevelUI()
    {
        SetBuildingLevelTextWithRedMaxLevel();

        bool canAfford = CanAffordBuildingUpgrade();

        BuildingUpgradePriceText.color =
            canAfford ? Color.white : Color.red;

        BuildingUpgradeButton.interactable =
            canAfford;
    }

    private void SetBuildingLevelTextWithRedMaxLevel()
    {
        BuildingLevelText.text =
            $"Lv: {currentBuilding.CurrentLevel} / " +
            $"<color=red>{currentBuilding.CurrentTierMaxLevel}</color>";
    }

    private void SetWorkerLevelTextWithRedMaxLevel()
    {
        WorkerLevelText.text =
            $"Lv: {currentWorker.CurrentLevel} / " +
            $"<color=red>{currentWorker.CurrentTierMaxLevel}</color>";
    }

    private void ApplyNeedBuildingTierUpgradeUI()
    {
        SetBuildingLevelTextMaxed();

        BuildingUpgradeButton.interactable = false;
        BuildingUpgradePriceText.color = Color.gray;

    }

    private void SetBuildingLevelTextMaxed()
    {
        BuildingLevelText.text =
            $"Lv: <color=red>{currentBuilding.CurrentLevel} / " +
            $"{currentBuilding.CurrentTierMaxLevel}</color>";
    }

    private void SetWorkerLevelTextMaxed()
    {
        WorkerLevelText.text =
            $"Lv: <color=red>{currentWorker.CurrentLevel} / " +
            $"{currentWorker.CurrentTierMaxLevel}</color>";
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

    private void UpdateWorkerStarUI(WorkerData worker)
    {
        int tierValue = (int)worker.CurrentTier;
        int maxStars = colorStarsWorker.Length;
        Color activeColor = GetColorByRarity(worker.CurrentRarity);
        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
        {
            colorStarsWorker[i].color = (i < tierValue) ? activeColor : inactiveColor;
        }
    }

    private void UpdateBuildingStarUI(BuildingData building)
    {
        int tierValue = (int)building.CurrentTier;
        int maxStars = colorStarsBuilding.Length;
        Color activeColor = GetColorByRarity(building.CurrentRarity);
        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
        {
            colorStarsBuilding[i].color = (i < tierValue) ? activeColor : inactiveColor;
        }
    }

    public void CloseBuildingPanel()
    {
        buildingPanel.SetActive(false);
    }

    public void OnUpgradeBuildingClicked()
    {
        currentBuilding.UpgradeBuilding();

        UpdateIncomeText();

        EvaluateBuildingUpgradeState();

        BuildingUpgradePriceText.text = currentBuilding.PriceToUpgrade.ToString();
        EventManager.Instance.QueueEvent(new XPAddedEvent(currentBuilding.CurrentLevel - 1));
    }

    private void UpdateAutomationUI(BuildingData building)
    {
        if (building.IsAutomated)
        {
            AutomationStatusText.text = "ON";
        }
        else
        {
            AutomationStatusText.text = "OFF";
        }
    }

    private void OnAutomationChanged(BuildingAutomationChangedEvent evt)
    {
        if (currentBuilding == evt.Building)
        {
            UpdateAutomationUI(currentBuilding);
        }
    }

    private bool CanAffordBuildingUpgrade()
    {
        return CurrencySystem.Instance.IsEnoughMoneyForUpgrade(
            currentBuilding.Currency,
            currentBuilding.PriceToUpgrade);
    }

    private bool HasWorker()
    {
        return currentWorker != null;
    }

    private bool CanAffordWorkerUpgrade()
    {
        return CurrencySystem.Instance.IsEnoughMoneyForUpgrade(
            currentWorker.Currency,
            currentWorker.PriceToUpgrade);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}