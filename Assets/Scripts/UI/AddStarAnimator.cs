using UnityEngine;
using DG.Tweening;

public class AddStarAnimator : MonoBehaviour
{
    private Tween pulseTween;
    private Tween ringTween;

    void OnEnable()
    {
        StartPulse();
    }

    void OnDisable()
    {
        StopAll();
    }

    public void StartPulse()
    {
        StopAll();

        pulseTween = transform
            .DOScale(1.08f, 0.8f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        ringTween = transform
            .DORotate(new Vector3(0, 0, 3f), 0.15f)
            .SetLoops(2, LoopType.Yoyo)
            .SetDelay(5f)
            .SetAutoKill(false)
            .Pause();
    }

    void Update()
    {
        if (ringTween != null && !ringTween.IsPlaying())
        {
            ringTween.Restart();
        }
    }

    public void StopAll()
    {
        pulseTween?.Kill();
        ringTween?.Kill();
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }
}