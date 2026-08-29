using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonPressAnimation : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler
{
    [SerializeField] private float pressedScale = 0.9f;
    [SerializeField] private float duration = 0.1f;

    private RectTransform rectTransform;
    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.DOKill();

        rectTransform
            .DOScale(originalScale * pressedScale, duration)
            .SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rectTransform.DOKill();

        rectTransform
            .DOScale(originalScale, duration)
            .SetEase(Ease.OutBack);
    }

    private void OnDisable()
    {
        rectTransform?.DOKill();

        if (rectTransform != null)
            rectTransform.localScale = originalScale;
    }
}