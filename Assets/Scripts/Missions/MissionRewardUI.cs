using UnityEngine;
using UnityEngine.UI;

public class MissionRewardUI : MonoBehaviour
{
    public static MissionRewardUI Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button claimButton;

    private MissionRuntime currentMission;

    private void Awake()
    {
        Instance = this;

        panel.SetActive(false);

        claimButton.onClick.AddListener(ClaimReward);
    }

    public void Open(MissionRuntime mission)
    {
        currentMission = mission;

        panel.SetActive(true);

        // Тут можна показати нагороду:
        // іконку
        // кількість
        // опис
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    private void ClaimReward()
    {
        if (currentMission == null)
            return;

        // Тут буде видача нагороди

        Close();
    }
}