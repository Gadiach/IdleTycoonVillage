using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item", order = 0)]

public class ShopItem : ScriptableObject
{
    public string Name = "Default";
    public int PurchasePrice;
    public CurrencyType Currency;
    public ShopCategory Type;
    public Sprite Icon;
    public GameObject Prefab;
}


