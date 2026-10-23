using UnityEngine;

[CreateAssetMenu(fileName = "New Currency Shop Item",menuName = "Currency Shop/Currency Shop Item")]
public class CurrencyShopItem : ScriptableObject
{
    public CurrencyShopCategory Category;
    public CurrencyShopRewardType RewardType;
    public float Price;
    public int Reward;
    public Sprite Icon;
    public int Order;

    [Header("Ads")]
    public int DailyAdLimit;

    [Header("Coin")]
    public int FreeRewardLimit;
}