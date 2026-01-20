using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI ownedText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Button studyButton;
    [SerializeField] private TextMeshProUGUI studyPriceText;

    private BlueprintItem item;

    public void Initialize(BlueprintItem blueprintItem)
    {
        item = blueprintItem;

        icon.sprite = item.icon;
        ownedText.text = $"Owned: {item.owned}";
        timeText.text = $"{item.studyTime}s";
        studyPriceText.text = item.studyCost.ToString();
    }
}