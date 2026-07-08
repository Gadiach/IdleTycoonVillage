using UnityEngine;

public class UniversityUI : MonoBehaviour
{
    public static UniversityUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private BlueprintItemUI[] blueprintItems;

    [SerializeField] private BlueprintItem[] buildingBlueprints;
    [SerializeField] private BlueprintItem[] workerBlueprints;

    [SerializeField] private TabGroup tabGroup;

    private void OnEnable()
    {
        tabGroup.TabSelected += ShowTab;
    }

    private void OnDisable()
    {
        tabGroup.TabSelected -= ShowTab;
    }

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void ShowBlueprints(BlueprintItem[] blueprints)
    {
        for (int i = 0; i < blueprintItems.Length; i++)
        {
            if (i < blueprints.Length)
            {
                blueprintItems[i].gameObject.SetActive(true);
                blueprintItems[i].Initialize(blueprints[i]);
            }
            else
            {
                blueprintItems[i].gameObject.SetActive(false);
            }
        }

        RefreshStudyButtons();
    }

    public void RefreshStudyButtons()
    {
        foreach (var item in blueprintItems)
        {
            item.UpdateStudyButtonState();
        }
    }

    public void UpdateUI(CurrencyType currencyType)
    {
        foreach (var itemUI in blueprintItems)
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

    public void ShowTab(int index)
    {
        switch (index)
        {
            case 0:
                ShowBlueprints(buildingBlueprints);
                break;

            case 1:
                ShowBlueprints(workerBlueprints);
                break;
        }
    }
    public void OpenUniversityPanel()
    {
        panel.SetActive(true);
    }

    public void CloseUniversityPanel()
    {
        panel.SetActive(false);
    }
}