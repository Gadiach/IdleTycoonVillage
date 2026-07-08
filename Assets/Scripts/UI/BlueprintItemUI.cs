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
    [SerializeField] private Image pazzleMainImg;
    [SerializeField] private Image pazzleSmallImg;

    private BlueprintItem item;

    public BlueprintItem Item => item;


    public void Initialize(BlueprintItem blueprintItem)
    {
        item = blueprintItem;
        studyPriceText.text = item.StudyCost.ToString();
        studyTimeText.text =  item.StudyTime.ToString() + "m";
        blueprintName.text = item.BlueprintName.ToString();

        pazzleMainImg.sprite = item.MainIcon;
        pazzleSmallImg.sprite = item.SmallIcon;


        UpdateOwnedText();
        UpdateStudyButtonState();
    }

    public void OnStudyClicked()
    {
        UniversitySystem.Instance.StartStudy(item);
    }

    public void UpdateOwnedText()
    {
        int owned = CurrencySystem.GetCurrencyAmount(item.Type);

        ownedText.text = "Owned: " + owned;
    }

    public void UpdateStudyButtonState()
    {
        bool canAfford = CurrencySystem.Instance.HasEnoughCurrency(item.StudyCurrency, item.StudyCost);

        bool noStudyRunning = !UniversitySystem.Instance.IsStudyInProgress;

        studyButton.interactable = canAfford && noStudyRunning;
    }
}