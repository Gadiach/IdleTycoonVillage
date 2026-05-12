using TMPro;
using UnityEngine;

public class BlueprintItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private TextMeshProUGUI studyPriceText;
    [SerializeField] private TextMeshProUGUI studyTimeText;
    [SerializeField] private TextMeshProUGUI blueprintName;

    private BlueprintItem item;


    public void Initialize(BlueprintItem blueprintItem)
    {
        item = blueprintItem;
        studyPriceText.text = item.StudyCost.ToString();
        studyTimeText.text = "Time: " + item.StudyTime.ToString();
        blueprintName.text = item.BlueprintName.ToString();

        UpdateUI();
    }

    public void UpdateUI()
    {
        int owned = CurrencySystem.GetCurrencyAmount(item.Type);

        ownedText.text = "Owned: " + owned.ToString();
    }
}