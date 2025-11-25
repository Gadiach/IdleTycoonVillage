using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimerTooltip : MonoBehaviour
{
    [SerializeField] private Timer timer;   
    [SerializeField] private Slider slider; 
    [SerializeField] private float timeToEnd;
    [SerializeField] private GameObject incomeObject;
    DateTime finishTime;
    bool isActiveBusiness = false;
    private BuildingData buildingData;

    [Header("Slider Colors")]
    [SerializeField] private Color manualColor = new Color(1f, 0.64f, 0f); 
    [SerializeField] private Color autoColor = Color.green;

    private void Start()
    {
        DateTime finishTime = DateTime.Now.AddSeconds(timeToEnd);

        if (timer != null)
        {
            timer.Initialize(finishTime);
            timer.TimerFinishedEvent.AddListener(OnTimerFinished);
            timer.StartTimer();
        }

        if (slider != null)
        {
            slider.maxValue = (float)(finishTime - DateTime.Now).TotalSeconds;
            slider.value = 0f; 
        }

        buildingData = GetComponentInParent<BuildingData>();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildingAutomationChangedGameEvent>(OnAutomationChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<BuildingAutomationChangedGameEvent>(OnAutomationChanged);
    }

    private void Update()
    {
        UpdateTimerProgress();            
    }

    public void ResetTimer()
    {       
        if(!isActiveBusiness)
        {
            RestartTimerLogic();
            ResetSliderUI();
        }

        SetIncomeBtn(false);
    }

    private void RestartTimerLogic()
    {
        finishTime = DateTime.Now.AddSeconds(timeToEnd);

        if (timer != null)
        {
            timer.Initialize(finishTime);
            timer.StartTimer();
        }
    }

    private void ResetSliderUI()
    {
        if (slider == null)
            return;

        slider.maxValue = timeToEnd;
        slider.value = 0f;
    }

    private void SetIncomeBtn(bool state)
    {
        incomeObject.SetActive(state);
    }

    private void UpdateTimerProgress()
    {
        float remainingTime = (float)timer.secondsLeft;
        slider.value = slider.maxValue - remainingTime;
    }

    private void OnAutomationChanged(BuildingAutomationChangedGameEvent evt)
    {
        if (buildingData == evt.Building)
        {
            isActiveBusiness = evt.IsAutomated;
            Debug.Log($"[TimerTooltip] Automation changed for {evt.Building.Name}: {isActiveBusiness}");
            UpdateSliderColor();
        }
    }

    private void UpdateSliderColor()
    {
        if (slider == null)
            return;

        Image fillImage = slider.fillRect?.GetComponent<Image>();

        if (fillImage != null)
        {
            fillImage.color = isActiveBusiness ? autoColor : manualColor;
        }
    }

    private void OnTimerFinished()
    {
        SetIncomeBtn(true);

        slider.value = slider.maxValue;

        if (isActiveBusiness)
        {
            RestartTimerLogic();
            ResetSliderUI();
        }

        buildingData.AddIncomeCircle();
    }
}