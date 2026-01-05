using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager current;
    public static Dictionary<CurrencyType, Sprite> currencySprites = new Dictionary<CurrencyType, Sprite>();

    [SerializeField] private List<Sprite> sprites;

    private RectTransform rt;
    private RectTransform prt;
    private bool opened;
    private Vector2 closedPos;
    private Vector2 openedPos;

    [SerializeField] private GameObject itemPrefab;
    private Dictionary<ObjectType, List<ShopItem>> shopItems = new Dictionary<ObjectType, List<ShopItem>>(capacity: 3);

    [SerializeField] public TabGroup shopTabs;
    [SerializeField] private float animationTime = 0.2f; 

    private void Awake()
    {
        current = this;
        rt = GetComponent<RectTransform>();
        prt = transform.parent.GetComponent<RectTransform>();

        closedPos = prt.anchoredPosition;
        openedPos = closedPos + new Vector2(rt.sizeDelta.x, 0);

        EventManager.Instance.AddListener<LevelChangedGameEvent>(OnLevelChanged);
    }

    private void Start()
    {
        currencySprites.Add(CurrencyType.Coins, sprites[0]);
        currencySprites.Add(CurrencyType.Crystals, sprites[1]);

        Load();
        Initialize();

        gameObject.SetActive(false);
    }

    private void Load()
    {
        ShopItem[] items = Resources.LoadAll<ShopItem>(path: "Shop");

        shopItems.Add(ObjectType.Buildings, new List<ShopItem>());
        shopItems.Add(ObjectType.Workers, new List<ShopItem>());
        shopItems.Add(ObjectType.Decorations, new List<ShopItem>());

        foreach (var item in items)
        {
            shopItems[item.Type].Add(item);
        }
    }

    private void Initialize()
    {
        for(int i = 0; i < shopItems.Keys.Count; i++)
        {
            foreach (var item in shopItems[(ObjectType)i])
            {
                GameObject itemObject = Instantiate(itemPrefab, shopTabs.objectsToSwap[i].transform);
                itemObject.GetComponent<ShopItemHolder>().Initialize(item);
            }
        }
    }

    public void UpdateShopItems()
    {
        foreach (var tab in shopTabs.objectsToSwap)
        {
            foreach (Transform item in tab.transform)
            {
                ShopItemHolder holder = item.GetComponent<ShopItemHolder>();
                if (holder != null)
                {
                    holder.UpdateItemState();
                }
            }
        }
    }

    private void OnLevelChanged(LevelChangedGameEvent info)
    {
        for(int i = 0; i < shopItems.Keys.Count; i++)
        {
            ObjectType key = shopItems.Keys.ToArray()[i];

            for(int j = 0; j < shopItems[key].Count; j++)
            {
                ShopItem item = shopItems[key][j];

                if(item.Level == info.newLvl)
                {
                    shopTabs.transform.GetChild(i).GetChild(j).GetComponent<ShopItemHolder>().UnlockItem();
                }
            }
        }
    }

    public void ShopButton_Click()
    {
        if (opened)
            CloseShop();
        else
            OpenShop(ObjectType.Buildings); 
    }

    public void OpenShop(ObjectType tabType)
    {
        if (!opened)
        {
            gameObject.SetActive(true);
            UpdateShopItems();
            StartCoroutine(MovePanel(openedPos));
            opened = true;
        }

        shopTabs.SelectTabByIndex((int)tabType);
    }

    public void CloseShop()
    {
        if (!opened) return;

        StartCoroutine(MovePanel(closedPos, () => gameObject.SetActive(false)));
        opened = false;
    }

    private IEnumerator MovePanel(Vector2 targetPos, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        Vector2 startPos = prt.anchoredPosition;

        while (elapsedTime < animationTime)
        {
            prt.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / animationTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        prt.anchoredPosition = targetPos;
        onComplete?.Invoke();
    }

    private bool dragging;
    public void OnBeginDrag()
    {
        dragging = true;
    }

    public void OnEndDrag()
    {
        dragging = false;
    }

    public void OnPointerClick()
    {
        if(!dragging)
        {
            ShopButton_Click();
        }
    }
}