using UnityEngine;

[CreateAssetMenu(fileName = "New Currency Shop Item",menuName = "Currency Shop/Currency Shop Item")]
public class CurrencyShopItem : ScriptableObject
{
    public string Name;
    public CurrencyShopCategory Category;
    public CurrencyType CurrencyType;
    public int Price;
    public int Reward;
    public Sprite Icon;
}