using UnityEngine;

[CreateAssetMenu(fileName = "New Blueprint Item", menuName = "University/Blueprint Item")]
public class BlueprintItem : ScriptableObject
{
    [SerializeField] private string blueprintName;
    [SerializeField] private Rarities Rarity;
    [SerializeField] private int studyCost;
    private CurrencyType studyCurrency = CurrencyType.Coins;
    [SerializeField] private float studyTime;
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private CurrencyCategory currencyCategory;
    [SerializeField] private BuildShopCategory buildShopCategory;

    public BuildShopCategory BuildShopCategory => buildShopCategory;

    public CurrencyType CurrencyType => currencyType;
    public CurrencyCategory CurrencyCategory => currencyCategory;
    public int StudyCost => studyCost;
    public CurrencyType StudyCurrency => studyCurrency;
    public float StudyTime => studyTime;
    public string BlueprintName => blueprintName;
    public Rarities CurrentRarity => Rarity;
}