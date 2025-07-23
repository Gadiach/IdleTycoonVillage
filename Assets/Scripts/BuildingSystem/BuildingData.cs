using UnityEngine;

public class BuildingData : MonoBehaviour
{
    public string Name;// { get;  set; }
    public string Description;// { get; private set; }
    public int Level; //{ get;  set; }
    public int Price; //{ get;  set; }
    public CurrencyType Currency;// { get; private set; }
    public ObjectType Type;// { get; private set; }
    public BusinessType BusinessType;
    public Sprite Icon;// { get;  set; }
    public int PriceToUpgrade = 5;
    public int LevelOfBuilding = 1;
    public int Income;



    public void Initialize(ShopItem item)
    {
        Name = item.Name;
        Description = item.Description;
        Level = item.Level;
        Price = item.Price;
        Currency = item.Currency;
        Type = item.Type;
        Icon = item.Icon;

        UpdatePriceToUpgrade();
    }

    public void UpdatePriceToUpgrade()
    {
        PriceToUpgrade = 5 * LevelOfBuilding;
    }

    public void UpdateIncome()
    {
        Income = 5 * LevelOfBuilding;
    }

    public void UpgradeBuilding()
    {
        if (CurrencySystem.Instance.TrySpendCurrency(Currency, PriceToUpgrade))
        {
            UpdatePriceToUpgrade();
        }
    }
}