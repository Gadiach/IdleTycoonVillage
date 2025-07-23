using System;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public bool isRunning { get; private set; }
    public UnityEvent TimerFinishedEvent;

    private DateTime finishTime;  // Конечное время

    public double secondsLeft { get; private set; }

    // Инициализация таймера с конечной датой
    public void Initialize(DateTime finishDate)
    {
        finishTime = finishDate;  // Устанавливаем конечное время
        secondsLeft = (finishTime - DateTime.Now).TotalSeconds;  // Рассчитываем сколько времени осталось
        TimerFinishedEvent = new UnityEvent();
    }

    private void Update()
    {
        if (isRunning)
        {
            // Просто сохраняем текущее оставшееся время
            secondsLeft = (finishTime - DateTime.Now).TotalSeconds;

            // Если время прошло, вызываем событие завершения
            if (secondsLeft <= 0)
            {
                secondsLeft = 0;
                isRunning = false;
                TimerFinishedEvent.Invoke();
            }
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