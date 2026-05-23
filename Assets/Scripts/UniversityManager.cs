using UnityEngine;

public class UniversityManager : MonoBehaviour
{
    [System.Serializable]
    private class BlueprintBinding
    {
        public BlueprintItem Item;
        public BlueprintItemUI UI;
    }
    public static UniversityManager Instance;

    [SerializeField] private BlueprintBinding[] bindings;

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

        foreach (var binding in bindings)
        {
            binding.UI.Initialize(binding.Item);
        }
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

    private void UpdateUI(CurrencyChangedEvent info)
    {
        foreach (var binding in bindings)
        {
            if (binding.Item.StudyCurrency == info.CurrencyType)
            {
                binding.UI.UpdateStudyButtonState();
            }

            if (binding.Item.Type == info.CurrencyType)
            {
                binding.UI.UpdateOwnedText();
            }
        }
    }
}