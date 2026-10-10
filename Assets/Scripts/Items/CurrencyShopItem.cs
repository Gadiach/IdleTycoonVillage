using UnityEngine;

[CreateAssetMenu(fileName = "New Currency Shop Item", menuName = "Shop/Currency Shop Item")]
public class CurrencyShopItem : ScriptableObject
{
    public string Name;

    public CurrencyType CurrencyType;
    public int Amount;

    public string PriceText;

    public Sprite Icon;
}