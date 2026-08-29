using DG.Tweening;
using UnityEngine;

public class TutorialHighlightSystem : MonoBehaviour
{
    public static TutorialHighlightSystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Highlight(RectTransform target)
    {
        if (target == null)
            return;

        target.DOKill();

        Vector3 originalScale = target.localScale;

        target
            .DOScale(originalScale * 1.15f, 0.25f)
            .SetLoops(6, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}