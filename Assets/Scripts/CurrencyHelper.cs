using System;

public static class CurrencyHelper
{
    public static CurrencyType GetBlueprintCurrency(Rarities rarity)
    {
        return rarity switch
        {
            Rarities.Primitive => CurrencyType.Blueprint_Primitive,
            Rarities.Developed => CurrencyType.Blueprint_Developed,
            Rarities.Industrial => CurrencyType.Blueprint_Industrial,
            Rarities.Modern => CurrencyType.Blueprint_Modern,
            Rarities.Futuristic => CurrencyType.Blueprint_Futuristic,
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
        };
    }
}