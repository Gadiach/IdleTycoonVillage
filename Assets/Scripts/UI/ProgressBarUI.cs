using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform fillMask;
    [SerializeField] private RectTransform fill;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.6f;

    private Tween progressTween;
    private float currentProgress;
    private float lastBackgroundWidth = -1f;

    [SerializeField] private ColorDatabase colorDatabase;
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        RefreshSize();
    }

    private void LateUpdate()
    {
        if (!Mathf.Approximately(
                background.rect.width,
                lastBackgroundWidth))
        {
            RefreshSize();
        }
    }

    public void SetProgress(float progress, bool animate = true,
        System.Action onComplete = null)
    {
        progress = Mathf.Clamp01(progress);

        progressTween?.Kill();

        RefreshSize();

        if (!animate)
        {
            currentProgress = progress;
            ApplyProgress(currentProgress);
            onComplete?.Invoke();
            return;
        }

        progressTween = DOTween.To(
                () => currentProgress,
                value =>
                {
                    currentProgress = value;
                    ApplyProgress(value);
                },
                progress,
                animationDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                currentProgress = progress;
                ApplyProgress(currentProgress);
                onComplete?.Invoke();
            });
    }

    public void SetColor(ColorCategory category)
    {
        fillImage.color = colorDatabase.GetColor(category);
    }

    private void RefreshSize()
    {
        float width = background.rect.width;

        lastBackgroundWidth = width;

        fill.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            width
        );

        ApplyProgress(currentProgress);
    }

    private void ApplyProgress(float progress)
    {
        float width = background.rect.width * progress;

        fillMask.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            width
        );
    }

    private void OnDisable()
    {
        progressTween?.Kill();
    }
}