using UnityEngine;

[CreateAssetMenu(
    fileName = "New Blueprint Item",
    menuName = "University/Blueprint Item")]
public class BlueprintItem : ScriptableObject
{
    public string name;
    public Sprite icon;
    public Rarities rarity;
    public int owned;
    public int studyCost;
    public float studyTime;
}