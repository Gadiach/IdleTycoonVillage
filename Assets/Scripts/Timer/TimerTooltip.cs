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
        float remainingTime = (float)timer.secondsLeft;
        slider.value = slider.maxValue - remainingTime;

        if (timer.secondsLeft <= 0 && incomeObject != null)
        {
            incomeObject.SetActive(true);
        }
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
}