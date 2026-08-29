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

        target.localScale = Vector3.one;

        target
            .DOScale(1.15f, 0.3f)
            .SetEase(Ease.InOutSine)
            .SetLoops(6, LoopType.Yoyo)
            .OnComplete(() =>
            {
                target.localScale = Vector3.one;
            });
    }
}