using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Timer timer;
    [SerializeField] private Slider slider;

    [Header("Slider Colors")]
    [SerializeField]
    private Color manualColor =
        new Color(1f, 0.64f, 0f);

    [SerializeField]
    private Color autoColor = Color.green;

    private void Update()
    {
        UpdateProgress();
    }

    public void Initialize(float duration)
    {
        if (slider == null)
            return;

        slider.maxValue = duration;
        slider.value = 0f;
    }

    public void SetCompleted()
    {
        if (slider == null)
            return;

        slider.value = slider.maxValue;
    }

    public void SetAutomationVisual(bool automated)
    {
        if (slider == null)
            return;

        Image fillImage =
            slider.fillRect?.GetComponent<Image>();

        if (fillImage != null)
        {
            fillImage.color =
                automated ? autoColor : manualColor;
        }
    }

    private void UpdateProgress()
    {
        if (timer == null || slider == null)
            return;

        if (!timer.isRunning)
            return;

        float remainingTime =
            (float)timer.secondsLeft;

        slider.value =
            slider.maxValue - remainingTime;
    }

    public void ResetUI()
    {
        if (slider == null)
            return;

        slider.value = 0f;
    }
}