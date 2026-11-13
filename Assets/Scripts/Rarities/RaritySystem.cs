public static class RaritySystem
{
    public static ColorCategory GetColorCategory(Rarities rarity)
    {
        return rarity switch
        {
            Rarities.Primitive => ColorCategory.PrimitiveColor,
            Rarities.Developed => ColorCategory.DevelopedColor,
            Rarities.Industrial => ColorCategory.IndustrialColor,
            Rarities.Modern => ColorCategory.ModernColor,
            Rarities.Futuristic => ColorCategory.FuturisticColor,
            _ => ColorCategory.PrimitiveColor
        };
    }
}