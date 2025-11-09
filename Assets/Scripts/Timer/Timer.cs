using System;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public bool isRunning { get; private set; }
    public UnityEvent TimerFinishedEvent;

    private DateTime finishTime;  

    public double secondsLeft { get; private set; }

    public void Initialize(DateTime finishDate)
    {
        finishTime = finishDate;
        secondsLeft = (finishTime - DateTime.Now).TotalSeconds;
    }

    private void Update()
    {
        if (!isRunning)
            return;

        secondsLeft = (finishTime - DateTime.Now).TotalSeconds;

        if (secondsLeft <= 0)
        {
            secondsLeft = 0;
            isRunning = false;
            TimerFinishedEvent.Invoke();
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}