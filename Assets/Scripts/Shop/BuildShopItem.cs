using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "BuildShop/Build Shop Item", order = 0)]

public class BuildShopItem : ScriptableObject
{
    public string Name = "Default";
    public int PurchasePrice;
    public CurrencyType Currency;
    public BuildShopCategory Type;
    public Sprite Icon;
    public GameObject Prefab;
    public BusinessType BusinessType;
}


