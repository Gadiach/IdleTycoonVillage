using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSpotlightSystem : MonoBehaviour
{
    public static TutorialSpotlightSystem Instance;

    [Header("References")]
    [SerializeField] private Image overlayImage;
    [SerializeField] private CanvasGroup overlayCanvasGroup;
    [SerializeField] private GameObject blackOverlay;

    private RectTransform currentTarget;

    private Tween hideDelayTween;

    [SerializeField] private float displayDuration = 1f;

    [Header("Settings")]
    [SerializeField] private float horizontalPadding = 20f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Material spotlightMaterial;

    private void Awake()
    {
        Instance = this;

        spotlightMaterial = Instantiate(overlayImage.material);
        overlayImage.material = spotlightMaterial;
    }

    private void LateUpdate()
    {
        if (currentTarget == null)
            return;

        SetTarget(currentTarget);
    }

    public void Show(RectTransform target)
    {
        if (target == null)
            return;

        blackOverlay.SetActive(true);

        currentTarget = target;

        overlayCanvasGroup.DOKill();
        overlayCanvasGroup.alpha = 1f;

        SetTarget(currentTarget);

        hideDelayTween?.Kill();

        hideDelayTween = DOVirtual.DelayedCall(
            displayDuration,
            Hide
        );
    }

    public void Hide()
    {
        hideDelayTween?.Kill();
        hideDelayTween = null;

        overlayCanvasGroup.DOKill();

        overlayCanvasGroup
            .DOFade(0f, fadeDuration)
            .OnComplete(() =>
            {
                currentTarget = null;
                blackOverlay.SetActive(false);
            });
    }

    private void SetTarget(RectTransform target)
    {
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);

        Vector2 screenMin = RectTransformUtility.WorldToScreenPoint(
            null,
            corners[0]
        );

        Vector2 screenMax = RectTransformUtility.WorldToScreenPoint(
            null,
            corners[2]
        );

        Vector2 center = (screenMin + screenMax) * 0.5f;
        Vector2 size = screenMax - screenMin;

        center = new Vector2(
            center.x / Screen.width,
            center.y / Screen.height
        );

        size = new Vector2((size.x + horizontalPadding * 2f) / Screen.width,size.y / Screen.height);

        spotlightMaterial.SetVector("_HoleCenter", center);
        spotlightMaterial.SetVector("_HoleSize", size);
    }
}