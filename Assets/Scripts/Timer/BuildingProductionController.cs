using System;
using UnityEngine;

public class BuildingProductionController : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private Timer timer;
    [SerializeField] private TimerUI timerUI;

    [Header("Income UI")]
    [SerializeField] private GameObject rewardObject;

    [SerializeField] private BlueprintItem currentBlueprint;
    public BlueprintItem CurrentBlueprint => currentBlueprint;

    private DateTime finishTime;

    private bool isBusinessAutomated;

    private BuildingData buildingData;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildingPlacedEvent>(OnBuildingPlaced);
        EventManager.Instance.AddListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<BuildingPlacedEvent>(OnBuildingPlaced);
        EventManager.Instance.RemoveListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
    }

    private void Awake()
    {
        buildingData = GetComponentInParent<BuildingData>();
    }

    private void Start()
    {

        timer.TimerFinishedEvent.AddListener(OnTimerFinished);

        if (!buildingData.StartProductionOnPlace)       
        {
            UniversityManager.Instance.RegisterProduction(this);
        }
    }

    public void CollectReward()
    {
        SetIncomeButton(false);

        if (!isBusinessAutomated)
        {
            StartProduction();
        }  
    }

    private void OnBuildingPlaced(BuildingPlacedEvent evt)
    {
        if (evt.Building != buildingData)
            return;

        if (buildingData.StartProductionOnPlace)
        {
            StartProduction();
        }
    }

    public void HideReward()
    {
        timer.StopTimer();

        SetIncomeButton(false);

        timerUI.ResetUI();
    }

    public void StartProduction()
    {
        finishTime = DateTime.Now.AddSeconds(buildingData.ProductionDuration);

        timer.Initialize(finishTime);

        timer.StartTimer();

        timerUI.Initialize(buildingData.ProductionDuration);
    }

    public void StartProduction(BlueprintItem blueprintItem)
    {
        currentBlueprint = blueprintItem;

        StartProduction();
    }

    private void SetIncomeButton(bool state)
    {
        rewardObject.SetActive(state);
    }

    private void OnAutomationChanged(BuildingAutomationChangedEvent evt)
    {
        if (buildingData != evt.Building)
            return;

        isBusinessAutomated = evt.IsAutomated;

        timerUI.SetAutomationVisual(isBusinessAutomated);

        StartProduction();
    }

    public void StartStudy(BlueprintItem item)
    {
        bool success = CurrencySystem.Instance.TrySpendCurrency(item.StudyCurrency, item.StudyCost);

        if (!success)
            return;

        StartProduction(item);
    }

    private void OnTimerFinished()
    {
        SetIncomeButton(true);

        timerUI.SetCompleted();

        buildingData.AddIncomeCircle();

        if (isBusinessAutomated)
        {
            StartProduction();
        }
    }
}