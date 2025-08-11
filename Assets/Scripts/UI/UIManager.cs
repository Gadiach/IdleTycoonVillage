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

    [Header("Worker UI")]
    [SerializeField] private Button workerButton;
    public Sprite defaultWorkerIcon;

    public BuildingPlaceable Placeable { get; private set; }

    private BuildingData currentBuilding;

    private void Awake()
    {
        Instance = this;
        buildingPanel.SetActive(false);
    }

    public void OpenBuildingPanel(BuildingData building)
    {
        currentBuilding = building;

        nameText.text = building.Name;
        levelText.text = "Lvl: " + building.LevelOfBuilding;
        priceText.text = building.PriceToUpgrade.ToString();
        iconImage.sprite = building.Icon;

        buildingPanel.SetActive(true);

        UpdateUIForUpgrade();

        workerButton.onClick.RemoveAllListeners();

        if (building.Placeable != null && building.Placeable.HasWorker())
        {
            WorkerData worker = building.Placeable.GetAssignedWorker();
            workerButton.image.sprite = worker.icon;

            workerButton.onClick.AddListener(() =>
            {
                WorkerUI.Instance.ShowWorker(worker);
            });
        }
        else
        {
            workerButton.image.sprite = defaultWorkerIcon;

            workerButton.onClick.AddListener(() =>
            {
                Debug.Log("There is no worker. Please assign one.");
            });
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

        currentBuilding.LevelOfBuilding++;
        currentBuilding.PriceToUpgrade += 5;

        levelText.text = "Lv: " + currentBuilding.LevelOfBuilding;
        priceText.text = currentBuilding.PriceToUpgrade.ToString();
        EventManager.Instance.QueueEvent(new XPAddedGameEvent(currentBuilding.LevelOfBuilding - 1));
        UpdateUIForUpgrade();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}