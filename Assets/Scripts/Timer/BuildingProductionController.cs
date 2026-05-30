using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildingProductionController : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private Timer timer;
    [SerializeField] private TimerUI timerUI;

    [Header("Income UI")]
    [SerializeField] private Image rewardIconImage;
    [SerializeField] private GameObject rewardObject;

    [SerializeField] private BlueprintItem currentBlueprint;
    public BlueprintItem CurrentBlueprint => currentBlueprint;

    private DateTime finishTime;

    private bool isBusinessAutomated;

    private BuildingData buildingData;
    private WorkerData workerData;

    public bool HasActiveStudy => currentBlueprint != null;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<WorkerAssignedToBuildingEvent>(OnWorkerAssigned);
        EventManager.Instance.AddListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;
        EventManager.Instance.RemoveListener<WorkerAssignedToBuildingEvent>(OnWorkerAssigned);
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

        if (buildingData.Placeable != null && buildingData.Placeable.HasWorker())
        {
            workerData = buildingData.Placeable.GetAssignedWorker();
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

    public void HideReward()
    {
        timer.StopTimer();

        SetIncomeButton(false);

        timerUI.ResetUI();
    }

    private void OnWorkerAssigned(WorkerAssignedToBuildingEvent evt)
    {
        if (evt.Building != buildingData)
            return;

        workerData = evt.Worker;
    }

    public void StartProduction()
    {
        finishTime = DateTime.Now.AddSeconds(workerData.ProductionDuration);

        timer.Initialize(finishTime);

        timer.StartTimer();

        timerUI.Initialize(workerData.ProductionDuration);
    }

    public void StartProduction(BlueprintItem blueprintItem)
    {
        currentBlueprint = blueprintItem;

        rewardIconImage.sprite = blueprintItem.Icon;

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

    public void ClearCurrentBlueprint()
    {
        currentBlueprint = null;
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