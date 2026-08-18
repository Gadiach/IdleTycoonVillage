using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyVFXSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CurrencyIconDatabase currencyIconDatabase;
    [SerializeField] private RectTransform flyingCurrencyPrefabsContainer;
    [SerializeField] private GameObject flyingCurrencyIconPrefab;

    [Header("Animation")]
    [SerializeField] private int flyingIconCount = 8;
    [SerializeField] private float scatterDuration = 0.15f;
    [SerializeField] private float flyDuration = 0.6f;
    [SerializeField] private float spawnInterval = 0.05f;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<CurrencyClaimedEvent>(OnCurrencyClaimed);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<CurrencyClaimedEvent>(OnCurrencyClaimed);
    }

    private void OnCurrencyClaimed(CurrencyClaimedEvent info)
    {
        Sprite currencyIcon = currencyIconDatabase.GetIcon(info.CurrencyType);

        if (currencyIcon == null)
            return;

        Play(info.SourcePosition, currencyIcon);
    }

    private void Play(Vector3 startPosition, Sprite currencyIcon)
    {
        for (int i = 0; i < flyingIconCount; i++)
        {
            SpawnCurrencyIcon(startPosition, currencyIcon, i);
        }
    }

    private void SpawnCurrencyIcon(
        Vector3 startPosition,
        Sprite currencyIcon,
        int index)
    {
        GameObject currencyObject = Instantiate(
            flyingCurrencyIconPrefab,
            flyingCurrencyPrefabsContainer
        );

        RectTransform currencyRect =
            currencyObject.GetComponent<RectTransform>();

        Image currencyImage =
            currencyObject.GetComponent<Image>();

        currencyImage.sprite = currencyIcon;

        currencyRect.position = startPosition;
        currencyRect.localScale = Vector3.zero;

        Vector3 scatterPosition =
            startPosition +
            new Vector3(
                Random.Range(-50f, 50f),
                Random.Range(-30f, 60f),
                0f
            );

        Sequence sequence = DOTween.Sequence();

        sequence.SetDelay(index * spawnInterval);

        sequence.Append(
            currencyRect
                .DOScale(1f, scatterDuration)
                .SetEase(Ease.OutBack)
        );

        sequence.Join(
            currencyRect
                .DOMove(scatterPosition, scatterDuration)
                .SetEase(Ease.OutQuad)
        );
    }
}