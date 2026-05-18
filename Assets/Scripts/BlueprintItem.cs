using UnityEngine;

[CreateAssetMenu(
    fileName = "New Blueprint Item",
    menuName = "University/Blueprint Item")]
public class BlueprintItem : ScriptableObject
{
    [SerializeField] private string blueprintName;
    [SerializeField] private Rarities Rarity;
    [SerializeField] private int studyCost;
    private CurrencyType studyCurrency = CurrencyType.Coins;
    [SerializeField] private float studyTime;
    [SerializeField] private CurrencyType type;
    [SerializeField] private Sprite icon;

    public Sprite Icon => icon;

    public CurrencyType Type => type;
    public int StudyCost => studyCost;
    public CurrencyType StudyCurrency => studyCurrency;
    public float StudyTime => studyTime;
    public string BlueprintName => blueprintName;
}