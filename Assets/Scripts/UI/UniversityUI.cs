using System.Collections.Generic;
using UnityEngine;

public class UniversityUI : MonoBehaviour
{
    public static UniversityUI Instance;

    [SerializeField] private GameObject universityPanel;
    [SerializeField] private GameObject blackBackground;

    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;
    [SerializeField] private BlueprintItemUI blueprintItemPrefab;

    private readonly List<BlueprintItemUI> blueprintItemUIs = new();

    [SerializeField] private TabGroup tabGroup;

    private void Awake()
    {
        Instance = this;

        universityPanel.SetActive(false);
        blackBackground.SetActive(false);
    }


    public void CreateAndInitializeBlueprintItems(Dictionary<ShopCategory, List<BlueprintItem>> blueprintItems)
    {
        foreach (var pair in blueprintItems)
        {
            CreateCategoryItems(pair.Value, pair.Key);
        }
    }

    private void CreateCategoryItems(List<BlueprintItem> items, ShopCategory category)
    {
        Transform parent = tabGroup.objectsToSwap[(int)category].transform;

        foreach (var item in items)
        {
            BlueprintItemUI itemUI = Instantiate(blueprintItemPrefab, parent);

            blueprintItemUIs.Add(itemUI);

            itemUI.Initialize(item, currencyIconDatabase.GetIcon(item.Type));
        }
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

    public void OpenUniversityPanel()
    {
        universityPanel.SetActive(true);
        blackBackground.SetActive(true);
    }

    public void CloseUniversityPanel()
    {
        universityPanel.SetActive(false);
        blackBackground.SetActive(false);
    }
}