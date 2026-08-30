using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDragHint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform hand;
    [SerializeField] private Image fakeItem;

    [Header("Animation")]
    [SerializeField] private float appearDuration = 0.2f;
    [SerializeField] private float dragDuration = 1f;
    [SerializeField] private float endPause = 0.4f;
    [SerializeField] private float restartPause = 0.5f;

    [Header("Hand")]
    [SerializeField] private Vector2 handOffset = new Vector2(30f, -30f);

    private RectTransform fakeItemRect;

    private Sequence sequence;

    private RectTransform source;
    private RectTransform target;

    private void Awake()
    {
        fakeItemRect = fakeItem.rectTransform;

        hand.gameObject.SetActive(false);
        fakeItem.gameObject.SetActive(false);
    }

    public void Play(
        RectTransform source,
        RectTransform target,
        Sprite itemSprite)
    {
        Stop();

        this.source = source;
        this.target = target;

        fakeItem.sprite = itemSprite;

        PlaySequence();
    }

    public void Stop()
    {
        sequence?.Kill();
        sequence = null;

        hand.DOKill();
        fakeItemRect.DOKill();

        hand.gameObject.SetActive(false);
        fakeItem.gameObject.SetActive(false);
    }

    private void PlaySequence()
    {
        hand.gameObject.SetActive(true);
        fakeItem.gameObject.SetActive(true);

        Vector3 startPosition = source.position;
        Vector3 targetPosition = target.position;

        fakeItemRect.position = startPosition;
        hand.position = startPosition + (Vector3)handOffset;

        fakeItemRect.localScale = Vector3.zero;
        hand.localScale = Vector3.zero;

        sequence = DOTween.Sequence();

        // Hand appears over the shop item.
        sequence.Append(
            hand.DOScale(Vector3.one, appearDuration)
                .SetEase(Ease.OutBack)
        );

        // Fake building appears as if it was picked up.
        sequence.Append(
            fakeItemRect.DOScale(Vector3.one, appearDuration)
                .SetEase(Ease.OutBack)
        );

        // Move building and hand together.
        sequence.Append(
            fakeItemRect.DOMove(targetPosition, dragDuration)
                .SetEase(Ease.InOutSine)
        );

        sequence.Join(
            hand.DOMove(
                    targetPosition + (Vector3)handOffset,
                    dragDuration
                )
                .SetEase(Ease.InOutSine)
        );

        // Fake placement.
        sequence.Append(
            fakeItemRect
                .DOScale(Vector3.zero, appearDuration)
                .SetEase(Ease.InBack)
        );

        sequence.Join(
            hand
                .DOScale(Vector3.zero, appearDuration)
                .SetEase(Ease.InBack)
        );

        sequence.AppendInterval(endPause + restartPause);

        sequence.OnComplete(PlaySequence);
    }

    private void OnDisable()
    {
        Stop();
    }
}