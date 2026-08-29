using DG.Tweening;
using UnityEngine;

public class WorkerIconAnimation : MonoBehaviour
{
    private RectTransform target;

    [Header("Shake")]
    [SerializeField] private float angle = 10f;
    [SerializeField] private float shakeDuration = 0.35f;
    [SerializeField] private int vibrato = 8;

    [Header("Interval")]
    [SerializeField] private float interval = 5f;

    private float timer;
    private Tween shakeTween;
    private bool isAnimating;

    private void Awake()
    {
        target = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!isAnimating)
            return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            PlayShake();
        }
    }

    public void StartAnimation()
    {
        isAnimating = true;
        timer = 0f;
    }

    public void StopAnimation()
    {
        isAnimating = false;
        timer = 0f;

        shakeTween?.Kill();

        target.localRotation = Quaternion.identity;
    }

    private void PlayShake()
    {
        shakeTween?.Kill();

        target.localRotation = Quaternion.identity;

        Sequence sequence = DOTween.Sequence();

        float swingDuration = shakeDuration / (vibrato * 2 + 2);

        sequence.Append(
            target.DOLocalRotate(
                new Vector3(0f, 0f, -angle),
                swingDuration
            ).SetEase(Ease.InOutSine)
        );

        for (int i = 0; i < vibrato; i++)
        {
            sequence.Append(
                target.DOLocalRotate(
                    new Vector3(0f, 0f, angle),
                    swingDuration
                ).SetEase(Ease.InOutSine)
            );

            sequence.Append(
                target.DOLocalRotate(
                    new Vector3(0f, 0f, -angle),
                    swingDuration
                ).SetEase(Ease.InOutSine)
            );
        }

        sequence.Append(
            target.DOLocalRotate(
                Vector3.zero,
                swingDuration
            ).SetEase(Ease.InOutSine)
        );

        shakeTween = sequence;
    }

    private void OnDisable()
    {
        shakeTween?.Kill();

        if (target != null)
            target.localRotation = Quaternion.identity;
    }
}