using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private TextMeshProUGUI studyPriceText;
    [SerializeField] private TextMeshProUGUI studyTimeText;
    [SerializeField] private TextMeshProUGUI blueprintName;
    [SerializeField] private Button studyButton;
    [SerializeField] private BuildingProductionController productionController;

    private BlueprintItem item;


    public void Initialize(BlueprintItem blueprintItem)
    {
        item = blueprintItem;
        studyPriceText.text = item.StudyCost.ToString();
        studyTimeText.text = "Time: " + item.StudyTime.ToString();
        blueprintName.text = item.BlueprintName.ToString();

        Refresh();
    }

    public void Refresh()
    {
        UpdateOwnedText();
        UpdateStudyButtonState();
    }

    public void OnStudyClicked()
    {
        UniversityManager.Instance.StartStudy(item);
    }

    private void UpdateOwnedText()
    {
        int owned = CurrencySystem.GetCurrencyAmount(item.Type);

        ownedText.text = "Owned: " + owned;
    }

    private void UpdateStudyButtonState()
    {
        bool canAfford = CurrencySystem.Instance.IsEnoughMoneyForUpgrade(item.StudyCurrency,item.StudyCost);

        studyButton.interactable = canAfford;
    }
}