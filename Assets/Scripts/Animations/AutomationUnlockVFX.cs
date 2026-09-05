using DG.Tweening;
using UnityEngine;

public class AutomationUnlockVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform automationIcon;

    [Header("Animation")]
    [SerializeField] private Vector2 startOffset = new Vector2(100f, 50f);
    [SerializeField] private float startScale = 0.5f;
    [SerializeField] private float flyDuration = 0.4f;
    [SerializeField] private float rotation = 180f;
    [SerializeField] private float landingScale = 1.2f;
    [SerializeField] private float landingDuration = 0.12f;

    private Vector2 initialPosition;
    private Vector3 initialScale;

    private void Awake()
    {
        initialPosition = automationIcon.anchoredPosition;
        initialScale = automationIcon.localScale;

        automationIcon.gameObject.SetActive(false);
    }

    public void Play()
    {
        automationIcon.DOKill();

        automationIcon.gameObject.SetActive(true);

        automationIcon.anchoredPosition = initialPosition + startOffset;
        automationIcon.localScale = initialScale * startScale;
        automationIcon.localRotation = Quaternion.Euler(0f, 0f, -rotation);

        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            automationIcon
                .DOAnchorPos(initialPosition, flyDuration)
                .SetEase(Ease.OutCubic)
        );

        sequence.Join(
            automationIcon
                .DORotate(Vector3.zero, flyDuration)
                .SetEase(Ease.OutCubic)
        );

        sequence.Join(
            automationIcon
                .DOScale(initialScale, flyDuration)
                .SetEase(Ease.OutCubic)
        );

        sequence.Append(
            automationIcon
                .DOScale(initialScale * landingScale, landingDuration)
                .SetEase(Ease.OutQuad)
        );

        sequence.Append(
            automationIcon
                .DOScale(initialScale, landingDuration)
                .SetEase(Ease.InQuad)
        );
    }

    public void Show()
    {
        automationIcon.DOKill();

        automationIcon.anchoredPosition = initialPosition;
        automationIcon.localScale = initialScale;
        automationIcon.localRotation = Quaternion.identity;

        automationIcon.gameObject.SetActive(true);
    }

    public void Hide()
    {
        automationIcon.DOKill();

        automationIcon.gameObject.SetActive(false);
    }
}