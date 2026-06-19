using UnityEngine;

public class UniversityManager : MonoBehaviour
{
    [System.Serializable]

    private class BlueprintBinding
    {
        public BlueprintItemUI UI;
    }

    public static UniversityManager Instance;

    [SerializeField] private BlueprintBinding[] bindings;

    [SerializeField] private BlueprintItem[] buildingBlueprints;
    [SerializeField] private BlueprintItem[] workerBlueprints;

    private BuildingProductionController activeProduction;

    public bool IsStudyInProgress => activeProduction != null && activeProduction.HasActiveStudy;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        EventManager.Instance.AddListener<CurrencyChangedEvent>(UpdateUI);
    }

    public void ShowBlueprints(BlueprintItem[] blueprints)
    {
        for (int i = 0; i < bindings.Length; i++)
        {
            if (i < blueprints.Length)
            {
                bindings[i].UI.gameObject.SetActive(true);
                bindings[i].UI.Initialize(blueprints[i]);
            }
            else
            {
                bindings[i].UI.gameObject.SetActive(false);
            }
        }

        RefreshStudyButtons();
    }

    private void RefreshStudyButtons()
    {
        foreach (var binding in bindings)
        {
            binding.UI.UpdateStudyButtonState();
        }
    }

    public void FinishStudy()
    {
        RefreshStudyButtons();
    }

    public void RegisterProduction(BuildingProductionController production)
    {
        activeProduction = production;
    }

    public void StartStudy(BlueprintItem item)
    {
        if (activeProduction == null)
        {
            Debug.LogWarning("No active university production!");
            return;
        }

        if (IsStudyInProgress)
            return;

        activeProduction.StartStudy(item);

        RefreshStudyButtons();
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

    private void UpdateUI(CurrencyChangedEvent info)
    {
        foreach (var binding in bindings)
        {
            BlueprintItem item = binding.UI.Item;

            if (item == null)
                continue;

            if (item.StudyCurrency == info.CurrencyType)
            {
                binding.UI.UpdateStudyButtonState();
            }

            if (item.Type == info.CurrencyType)
            {
                binding.UI.UpdateOwnedText();
            }
        }
    }
}