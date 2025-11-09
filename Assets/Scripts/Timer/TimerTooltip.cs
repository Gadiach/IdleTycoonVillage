using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerTooltip : MonoBehaviour
{
    [SerializeField] private Timer timer;   
    [SerializeField] private Slider slider; 
    [SerializeField] private float timeToEnd;
    [SerializeField] private GameObject incomeObject;
    DateTime finishTime;

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
    }

    private void Update()
    {
        UpdateTimerProgress();            
    }

    public void ResetTimer()
    {
        finishTime = DateTime.Now.AddSeconds(timeToEnd);

        if (timer != null)
        {
            timer.Initialize(finishTime);
            timer.StartTimer();
        }

        if (slider != null)
        {
            slider.maxValue = timeToEnd;
            slider.value = 0f;
        }

        if (incomeObject != null)
        {
            incomeObject.SetActive(false);
        }
    }

    private void UpdateTimerProgress()
    {
        float remainingTime = (float)timer.secondsLeft;
        slider.value = slider.maxValue - remainingTime;
    }

    private void OnTimerFinished()
    {
        incomeObject.SetActive(true);

        slider.value = slider.maxValue;

        Debug.Log("[TimerTooltip] Timer finished! Income object activated.");
    }
}