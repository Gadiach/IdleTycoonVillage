using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniversityUI : MonoBehaviour
{
    public static UniversityUI Instance;

    [SerializeField] private GameObject universityPanel;
    [SerializeField] private GameObject blackBackground;

    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;
    [SerializeField] private BlueprintItemUI blueprintItemPrefab;

    private readonly List<BlueprintItemUI> blueprintItemUIs = new();

    [SerializeField] private TabGroup tabGroup;

    [Header("University Building Icon")]
    [SerializeField] private BuildingIconDatabase buildingIconDatabase;
    [SerializeField] private Image universityBuildingImage;

    [Header("Worker Icon")]
    [SerializeField] private WorkerIconDatabase workerIconDatabase;
    [SerializeField] private Image universityWorkerImage;

    [Header("Worker UI")]
    [SerializeField] private GameObject OpenWorkerBPPanelButton;
    [SerializeField] private GameObject AddWorkerBtn;

    private void Awake()
    {
        Instance = this;

        universityPanel.SetActive(false);
        blackBackground.SetActive(false);
    }


    public void CreateAndInitializeBlueprintItems(Dictionary<BuildShopCategory, List<BlueprintItem>> blueprintItems)
    {
        if (tabGroup == null || blueprintItemPrefab == null)
            return;

        foreach (var pair in blueprintItems)
        {
            CreateCategoryItems(pair.Value, pair.Key);
        }
    }

    private void CreateCategoryItems(List<BlueprintItem> items, BuildShopCategory category)
    {
        Transform parent = tabGroup.objectsToSwap[(int)category].transform;

        foreach (var item in items)
        {
            BlueprintItemUI itemUI = Instantiate(blueprintItemPrefab, parent);

            blueprintItemUIs.Add(itemUI);

            itemUI.Initialize(item, currencyIconDatabase.GetIcon(item.Type));
        }
    }

    private void UpdateUniversityBuildingIcon()
    {
        BuildingData university =
            EntityRegistry.Instance.GetBuilding(BusinessType.University);

        if (university == null)
            return;

        universityBuildingImage.sprite = buildingIconDatabase.GetIcon(
            university.BusinessType,
            university.CurrentRarity
        );
    }

    private void UpdateUniversityWorkerUI()
    {
        BuildingData university =
            EntityRegistry.Instance.GetBuilding(BusinessType.University);

        WorkerData worker = university?.Placeable?.GetAssignedWorker();

        bool hasWorker = worker != null;

        OpenWorkerBPPanelButton.SetActive(hasWorker);
        AddWorkerBtn.SetActive(!hasWorker);

        universityWorkerImage.sprite = hasWorker
            ? workerIconDatabase.GetFullBodyIcon(
                worker.BusinessType,
                worker.CurrentRarity
            )
            : workerIconDatabase.DefaultInactiveWorkerIcon;
    }


    public void RefreshStudyButtons()
    {
        foreach (var itemUI in blueprintItemUIs)
        {
            itemUI.UpdateStudyButtonState();
        }
    }

    public void UpdateUI(CurrencyType currencyType)
    {
        foreach (var itemUI in blueprintItemUIs)
        {
            if (itemUI.Item == null)
                continue;

            if (itemUI.Item.StudyCurrency == currencyType)
            {
                itemUI.UpdateStudyButtonState();
            }

            if (itemUI.Item.Type == currencyType)
            {
                itemUI.UpdateOwnedText();
            }
        }
    }

    public void TryOpenUniversity()
    {
        BuildingData university =
            EntityRegistry.Instance.GetBuilding(BusinessType.University);

        if (university != null)
        {
            OpenUniversityPanel();
            return;
        }

        BuildShopSystem.Instance.OpenBuildShop(BuildShopCategory.Buildings);

        BuildShopItemUI targetItem =
            BuildShopSystem.Instance.GetBuildingItem(BusinessType.University);

        TutorialHighlightSystem.Instance.Highlight(
            targetItem.IconAndArrow
        );

        BuildShopSystem.Instance.ScrollUp(1f);
    }

    public void OpenWorkerShop()
    {
        BuildingData university =
            EntityRegistry.Instance.GetBuilding(BusinessType.University);

        if (university == null || university.Placeable == null)
            return;

        BusinessType workerType = university.Placeable.AcceptedBusinessType;

        CloseUniversityPanel();

        BuildShopSystem.Instance.OpenBuildShop(BuildShopCategory.Workers);

        BuildShopItemUI targetItem =
            BuildShopSystem.Instance.GetWorkerItem(workerType);

        if (targetItem != null)
        {
            TutorialHighlightSystem.Instance.Highlight(targetItem.IconAndArrow);
        }
    }

    public void OpenUniversityPanel()
    {
        UpdateUniversityBuildingIcon();
        UpdateUniversityWorkerUI();

        universityPanel.SetActive(true);
        blackBackground.SetActive(true);
    }

    public void CloseUniversityPanel()
    {
        universityPanel.SetActive(false);
        blackBackground.SetActive(false);
    }
}