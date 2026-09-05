using DG.Tweening;
using UnityEngine;

public class TutorialTapHint : MonoBehaviour
{
    [SerializeField] private RectTransform hand;

    [Header("Animation")]
    [SerializeField] private float appearDuration = 0.2f;
    [SerializeField] private float tapDuration = 0.15f;
    [SerializeField] private float pauseDuration = 0.7f;
    [SerializeField] private float tapScale = 0.8f;
    [SerializeField] private float targetTapScale = 1.08f;

    private Sequence sequence;

    private Transform tapTarget;
    private Vector3 targetInitialScale;

    private void Awake()
    {
        hand.gameObject.SetActive(false);
    }

    public void Play(Vector3 screenPosition, Transform target)
    {
        Stop();

        tapTarget = target;
        targetInitialScale = tapTarget.localScale;

        hand.position = screenPosition;
        hand.gameObject.SetActive(true);

        PlaySequence();
    }

    public void Stop()
    {
        sequence?.Kill();
        sequence = null;

        hand.DOKill();
        hand.localScale = Vector3.one;
        hand.gameObject.SetActive(false);

        if (tapTarget != null)
        {
            tapTarget.DOKill();
            tapTarget.localScale = targetInitialScale;
            tapTarget = null;
        }
    }

    private void PlaySequence()
    {
        hand.localScale = Vector3.zero;

        sequence = DOTween.Sequence();

        // Hand appears
        sequence.Append(
            hand.DOScale(Vector3.one, appearDuration)
                .SetEase(Ease.OutBack)
        );

        // Hand presses + building grows
        sequence.Append(
            hand.DOScale(tapScale, tapDuration)
                .SetEase(Ease.InOutSine)
        );

        sequence.Join(
            tapTarget.DOScale(
                targetInitialScale * targetTapScale,
                tapDuration
            ).SetEase(Ease.InOutSine)
        );

        // Hand releases + building returns
        sequence.Append(
            hand.DOScale(Vector3.one, tapDuration)
                .SetEase(Ease.InOutSine)
        );

        sequence.Join(
            tapTarget.DOScale(
                targetInitialScale,
                tapDuration
            ).SetEase(Ease.InOutSine)
        );

        sequence.AppendInterval(pauseDuration);

        // Hand disappears
        sequence.Append(
            hand.DOScale(Vector3.zero, appearDuration)
                .SetEase(Ease.InBack)
        );

        sequence.AppendInterval(pauseDuration);

        sequence.OnComplete(PlaySequence);
    }

    private void OnDisable()
    {
        Stop();
    }
}