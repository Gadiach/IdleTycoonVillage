using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Database", menuName = "Colors/Color Database")]
public class ColorDatabase : ScriptableObject
{
    [SerializeField] private ColorEntry[] colors;

    public Color GetColor(ColorCategory category)
    {
        foreach (var colorEntry in colors)
        {
            if (colorEntry.Category == category)
                return colorEntry.Color;
        }

        Debug.LogWarning($"No color found for category: {category}");
        return Color.white;
    }
}

[Serializable]
public class ColorEntry
{
    public ColorCategory Category;
    public Color Color;
}