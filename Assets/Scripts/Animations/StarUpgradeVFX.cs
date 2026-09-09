using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StarUpgradeVFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject flyingStarPrefab;

    [Header("Worker")]
    [SerializeField] private RectTransform workerFlyingStarsContainer;
    [SerializeField] private Image[] workerStars;

    [Header("Building")]
    [SerializeField] private RectTransform buildingFlyingStarsContainer;
    [SerializeField] private Image[] buildingStars;

    [Header("Fly Animation")]
    [SerializeField] private float flyDuration = 0.5f;
    [SerializeField] private float flyingStarStartScale = 0.8f;
    [SerializeField] private float flyingStarPopScale = 1.15f;
    [SerializeField] private float flyingStarPopDuration = 0.12f;

    [Header("Stars Bump")]
    [SerializeField] private float smallBumpScale = 1.05f;
    [SerializeField] private float smallBumpDuration = 0.1f;

    [SerializeField] private float targetBumpScale = 1.3f;
    [SerializeField] private float targetBumpDuration = 0.15f;

    public void PlayWorkerUpgrade(
        Vector3 startPosition,
        Image targetStar,
        Sprite flyingStarSprite,
        Color targetColor)
    {
        Play(
            startPosition,
            targetStar,
            workerStars,
            workerFlyingStarsContainer,
            flyingStarSprite,
            targetColor
        );
    }

    public void PlayBuildingUpgrade(
        Vector3 startPosition,
        Image targetStar,
        Sprite flyingStarSprite,
        Color targetColor)
    {
        Play(
            startPosition,
            targetStar,
            buildingStars,
            buildingFlyingStarsContainer,
            flyingStarSprite,
            targetColor
        );
    }

    private void Play(
        Vector3 startPosition,
        Image targetStar,
        Image[] stars,
        RectTransform flyingStarsContainer,
        Sprite flyingStarSprite,
        Color targetColor)
    {
        GameObject starObject =
            Instantiate(flyingStarPrefab, flyingStarsContainer);

        RectTransform starRect =
            starObject.GetComponent<RectTransform>();

        Image starImage =
            starObject.GetComponent<Image>();

        starImage.sprite = flyingStarSprite;

        starRect.position = startPosition;
        starRect.localScale = Vector3.one * flyingStarStartScale;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            starRect
                .DOScale(flyingStarPopScale, flyingStarPopDuration)
                .SetEase(Ease.OutBack)
        );

        sequence.Append(
            starRect
                .DOMove(targetStar.rectTransform.position, flyDuration)
                .SetEase(Ease.InOutQuad)
        );

        sequence.OnComplete(() =>
        {
            Destroy(starObject);

            targetStar.color = targetColor;

            PlayStarsBump(stars, targetStar);
        });
    }

    private void PlayStarsBump(Image[] stars, Image targetStar)
    {
        foreach (Image star in stars)
        {
            RectTransform starRect = star.rectTransform;

            starRect.DOKill();
            starRect.localScale = Vector3.one;

            if (star == targetStar)
            {
                PlayTargetBump(starRect);
            }
            else
            {
                PlaySmallBump(starRect);
            }
        }
    }

    private void PlaySmallBump(RectTransform star)
    {
        star
            .DOScale(smallBumpScale, smallBumpDuration)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo);
    }

    private void PlayTargetBump(RectTransform star)
    {
        star
            .DOScale(targetBumpScale, targetBumpDuration)
            .SetEase(Ease.OutBack)
            .SetLoops(2, LoopType.Yoyo);
    }
}