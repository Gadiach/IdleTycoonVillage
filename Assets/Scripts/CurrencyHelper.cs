using System;

public static class CurrencyHelper
{
    public static CurrencyType GetBuildingBlueprintCurrency(Rarities rarity)
    {
        return rarity switch
        {
            Rarities.Primitive => CurrencyType.BuildingBlueprint_Primitive,
            Rarities.Developed => CurrencyType.BuildingBlueprint_Developed,
            Rarities.Industrial => CurrencyType.BuildingBlueprint_Industrial,
            Rarities.Modern => CurrencyType.BuildingBlueprint_Modern,
            Rarities.Futuristic => CurrencyType.BuildingBlueprint_Futuristic,
            _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null)
        };
    }

    public static CurrencyType GetWorkerBlueprintCurrency(Rarities rarity)
    {
        return rarity switch
        {
            Rarities.Primitive => CurrencyType.WorkerBlueprint_Primitive,
            Rarities.Developed => CurrencyType.WorkerBlueprint_Developed,
            Rarities.Industrial => CurrencyType.WorkerBlueprint_Industrial,
            Rarities.Modern => CurrencyType.WorkerBlueprint_Modern,
            Rarities.Futuristic => CurrencyType.WorkerBlueprint_Futuristic,
            _ => CurrencyType.WorkerBlueprint_Primitive
        };
    }
}