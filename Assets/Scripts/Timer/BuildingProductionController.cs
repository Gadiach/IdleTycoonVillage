using System;
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

    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;

    private DateTime finishTime;

    private bool isBusinessAutomated;

    private BuildingData buildingData;
    private WorkerData workerData;

    public bool HasActiveStudy => currentBlueprint != null;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;
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
            UniversitySystem.Instance.RegisterProduction(this);
        }

        if (buildingData.Placeable != null && buildingData.Placeable.HasWorker())
        {
            workerData = buildingData.Placeable.GetAssignedWorker();
        }
    }

    public void SetWorker(WorkerData worker)
    {
        workerData = worker;
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

    public void StartProduction()
    {

        finishTime = DateTime.Now.AddSeconds(workerData.CycleDuration);

        timer.Initialize(finishTime);

        timer.StartTimer();

        timerUI.Initialize(workerData.CycleDuration);
    }

    public void StartProduction(BlueprintItem blueprintItem)
    {
        currentBlueprint = blueprintItem;

        rewardIconImage.sprite = currencyIconDatabase.GetIcon(blueprintItem.Type);

        StartProduction();
    }

    public void StartResearch(BlueprintItem item)
    {
        currentBlueprint = item;

        rewardIconImage.sprite = currencyIconDatabase.GetIcon(item.Type);

        finishTime = DateTime.Now.AddSeconds(item.StudyTime);

        timer.Initialize(finishTime);
        timer.StartTimer();

        timerUI.Initialize(item.StudyTime);
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
        if (!CurrencySystem.Instance.SpendCurrency(item.StudyCurrency, item.StudyCost))
            return;

        StartResearch(item);
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