using UnityEngine;

public class UniversitySystem : MonoBehaviour
{
    public static UniversitySystem Instance;

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

    private void OnEnable()
    {
        EventManager.Instance.AddListener<CurrencyAddedEvent>(OnCurrencyAdded);
        EventManager.Instance.AddListener<CurrencySpentEvent>(OnCurrencySpent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<CurrencyAddedEvent>(OnCurrencyAdded);
        EventManager.Instance.RemoveListener<CurrencySpentEvent>(OnCurrencySpent);
    }

    public void FinishStudy()
    {
        UniversityUI.Instance.RefreshStudyButtons();
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

        UniversityUI.Instance.RefreshStudyButtons();
    }

    private void OnCurrencyAdded(CurrencyAddedEvent info)
    {
        UniversityUI.Instance.UpdateUI(info.CurrencyType);
    }

    private void OnCurrencySpent(CurrencySpentEvent info)
    {
        UniversityUI.Instance.UpdateUI(info.CurrencyType);
    }
}