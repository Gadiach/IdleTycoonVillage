using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private TextMeshProUGUI studyPriceText;
    [SerializeField] private TextMeshProUGUI studyTimeText;
    [SerializeField] private Sprite studyButtonActiveSprite;
    [SerializeField] private Sprite studyButtonInactiveSprite;

    [SerializeField] private Sprite studyIconActive;
    [SerializeField] private Sprite studyIconInactive;

    [SerializeField] private Image studyIcon;
    [SerializeField] private Button studyButton;
    [SerializeField] private Image pazzleMainImg;

    private BlueprintItem item;

    public BlueprintItem Item => item;


    public void Initialize(BlueprintItem blueprintItem, Sprite blueprintIcon)
    {
        item = blueprintItem;

        studyPriceText.text = item.StudyCost.ToString();
        studyTimeText.text = item.StudyTime + "m";

        pazzleMainImg.sprite = blueprintIcon;

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

        SetStudyButtonState(canAfford && noStudyRunning);
    }

    private void SetStudyButtonState(bool interactable)
    {
        studyButton.interactable = interactable;

        UpdateStudyButtonSprite(interactable);
        UpdateStudyIcon(interactable);
    }

    private void UpdateStudyButtonSprite(bool interactable)
    {
        studyButton.image.sprite = interactable ? studyButtonActiveSprite : studyButtonInactiveSprite;
    }

    private void UpdateStudyIcon(bool interactable)
    {
        studyIcon.sprite = interactable ? studyIconActive : studyIconInactive;
    }
}