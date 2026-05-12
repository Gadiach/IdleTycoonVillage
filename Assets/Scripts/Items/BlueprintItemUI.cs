using TMPro;
using UnityEngine;

public class BlueprintItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private TextMeshProUGUI studyPriceText;

    private BlueprintItem item;


    public void Initialize(BlueprintItem blueprintItem)
    {
        item = blueprintItem;
        studyPriceText.text = item.StudyCost.ToString();

        UpdateUI();
    }

    public void UpdateUI()
    {
        int owned = CurrencySystem.GetCurrencyAmount(item.Type);

        ownedText.text = owned.ToString();
    }
}