using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private ShopItem shopItem;

    private RectTransform itemView;
    private bool hasSpawnedPreview;

    private RectTransform rt;
    private CanvasGroup cg;
    private Image img;

    private Vector3 originPos;

    public void Initialize(ShopItem item, RectTransform itemView)
    {
        shopItem = item;
        this.itemView = itemView;
    }

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        img = GetComponent<Image>();
        originPos = rt.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        hasSpawnedPreview = false;
        cg.blocksRaycasts = false;
        img.maskable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rt.position = eventData.position;

        if (!hasSpawnedPreview &&
            !RectTransformUtility.RectangleContainsScreenPoint(
                itemView,
                eventData.position,
                eventData.pressEventCamera))
        {
            SpawnPreview(eventData.position);
            hasSpawnedPreview = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = true;
        img.maskable = true; 
        rt.anchoredPosition = originPos;
    }

    private void SpawnPreview(Vector2 screenPosition)
    {
        ShopSystem.Instance.ShopButton_Click();

        Color c = img.color;
        c.a = 0f;
        img.color = c;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        worldPosition.z = 0;

        GridPlacementSystem.current.InitializeWithObject(shopItem.Prefab,worldPosition,shopItem);
    }

    private void OnEnable()
    {
        cg.blocksRaycasts = true;
        img.maskable = true;
        rt.anchoredPosition = originPos;

        Color c = img.color;
        c.a = 1f;
        img.color = c;
    }

}
