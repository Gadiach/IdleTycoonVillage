using UnityEngine;

[CreateAssetMenu(
    fileName = "New Blueprint Item",
    menuName = "University/Blueprint Item")]
public class BlueprintItem : ScriptableObject
{
    [SerializeField] private string BlueprintName;
    [SerializeField] private Rarities Rarity;
    [SerializeField] private int studyCost;
    [SerializeField] private float studyTime;
    [SerializeField] private CurrencyType type;

    public CurrencyType Type => type;
    public int StudyCost => studyCost;
}