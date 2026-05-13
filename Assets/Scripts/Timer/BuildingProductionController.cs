using System;
using UnityEngine;

public class BuildingProductionController : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private Timer timer;
    [SerializeField] private TimerUI timerUI;

    [Header("Income UI")]
    [SerializeField] private GameObject rewardObject;

    private DateTime finishTime;

    private bool isBusinessAutomated;

    private BuildingData buildingData;

    private void Start()
    {
        buildingData = GetComponentInParent<BuildingData>();

        timer.TimerFinishedEvent.AddListener(OnTimerFinished);

        UniversityManager.Instance.RegisterProduction(this);

        if (buildingData.StartProductionOnPlace)
        {
            StartProduction();
        }
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener <BuildingAutomationChangedEvent>(OnAutomationChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener <BuildingAutomationChangedEvent>(OnAutomationChanged);
    }

    public void ResetProduction()
    {
        if (!isBusinessAutomated)
        {
            StartProduction();
        }

        SetIncomeButton(false);
    }

    public void StartProduction()
    {
        finishTime = DateTime.Now.AddSeconds(buildingData.ProductionDuration);

        timer.Initialize(finishTime);

        timer.StartTimer();

        timerUI.Initialize(buildingData.ProductionDuration);
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