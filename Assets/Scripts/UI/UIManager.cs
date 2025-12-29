using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public GameObject buildingPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI priceText;
    public Image iconImage;
    public Button upgradeButton;
    public TextMeshProUGUI workerLvlNeededText;
    public TextMeshProUGUI automationStatusText;

    [SerializeField] private Image[] colorStars;

    [Header("Worker UI")]
    [SerializeField] private Button workerButton;

    [Header("Building UI")]
    [SerializeField] private Button buildingButton;

    public Sprite defaultWorkerIcon;

    public BuildingPlaceable Placeable { get; private set; }

    private BuildingData currentBuilding;

    private void Awake()
    {
        Instance = this;
        buildingPanel.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<WorkerUpgradedGameEvent>(OnWorkerUpgraded);
        EventManager.Instance.AddListener<BuildingAutomationChangedGameEvent>(OnAutomationChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<WorkerUpgradedGameEvent>(OnWorkerUpgraded);
        EventManager.Instance.RemoveListener<BuildingAutomationChangedGameEvent>(OnAutomationChanged);
    }

    private void OnWorkerUpgraded(WorkerUpgradedGameEvent evt)
    {
        currentBuilding = evt.Worker.AssignedBuilding;

        if (currentBuilding == null || currentBuilding.Placeable == null) return;

        WorkerData assignedWorker = currentBuilding.Placeable.GetAssignedWorker();

        if (assignedWorker == evt.Worker)
        {
            workerButton.image.sprite = evt.Worker.icon;
            currentBuilding.CheckAutomationState();
            Debug.Log($"[UIManager] worker has been upgraded to lvl {evt.Worker.level}. UI updated.");
        }
    }

    public void OpenMainBuildingPanel(BuildingData building)
    {
        currentBuilding = building;

        nameText.text = building.Name;
        levelText.text = "Lvl: " + building.LevelOfBuilding;
        priceText.text = building.PriceToUpgrade.ToString();
        iconImage.sprite = building.Icon;

        buildingPanel.SetActive(true);

        UpdateUIForUpgrade();
        UpdateStarUI(building);

        buildingButton.onClick.RemoveAllListeners();
        buildingButton.onClick.AddListener(() =>
        {
            BuildingUI.Instance.OpenBuildingPanel(building);
        });
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

        if (!canUpgrade)
        {
            priceText.color = Color.red;
            upgradeButton.interactable = false;
        }
        else
        {
            priceText.color = Color.white;
            upgradeButton.interactable = true;
        }
    }

    public void UpgradeBuilding()
    {
        currentBuilding.UpgradeBuilding();

        currentBuilding.PriceToUpgrade += 5;

        levelText.text = "Lv: " + currentBuilding.LevelOfBuilding;
        priceText.text = currentBuilding.PriceToUpgrade.ToString();
        EventManager.Instance.QueueEvent(new XPAddedGameEvent(currentBuilding.LevelOfBuilding - 1));
        UpdateUIForUpgrade();
    }

    private void OnAutomationChanged(BuildingAutomationChangedGameEvent evt)
    {
        if (currentBuilding == evt.Building)
        {
            automationStatusText.text = evt.IsAutomated ? "Auto ON" : "Auto OFF";
            Debug.Log($"[UIManager] Automation changed: {(evt.IsAutomated ? "ON" : "OFF")} for {evt.Building.Name}");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}