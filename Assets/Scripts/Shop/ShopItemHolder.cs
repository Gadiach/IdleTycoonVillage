using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemHolder : MonoBehaviour
{
    private ShopItem Item;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image currencyImage;
    [SerializeField] private TextMeshProUGUI priceText;

    private ShopItemDrag dragComponent;

    public void Initialize(ShopItem item)
    {
        Item = item;

        iconImage.sprite = Item.Icon;
        titleText.text = Item.Name;
        descriptionText.text = Item.Description;
        currencyImage.sprite = ShopManager.currencySprites[Item.Currency];
        priceText.text = Item.PurchasePrice.ToString();

        if(Item.Level >= LevelSystem.Level)
        {
            UnlockItem();
        }
        UpdateItemState();
    }

    public void UnlockItem()
    {
        if (iconImage.gameObject.GetComponent<ShopItemDrag>() == null)
        {
            dragComponent = iconImage.gameObject.AddComponent<ShopItemDrag>();
            dragComponent.Initialize(Item);
        }        
    }

    public void UpdateItemState()
    {
        int currentMoney = CurrencySystem.GetCurrencyAmount(Item.Currency); 

        if (currentMoney < Item.PurchasePrice)
        {           
            priceText.color = Color.red; 
            if (dragComponent != null)
                dragComponent.enabled = false;
        }
        else
        {
            priceText.color = Color.white;
            if (dragComponent != null)
                dragComponent.enabled = true;
        }
    }
}
