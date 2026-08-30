using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    public static TutorialSystem Instance;

    [Header("Settings")]
    [SerializeField] private bool playTutorial = true;

    [Header("References")]
    [SerializeField] private TutorialDialogueUI dialogueUI;

    private TutorialStep currentStep;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!playTutorial)
            return;

        StartTutorial();
    }

    private void StartTutorial()
    {
        currentStep = TutorialStep.Intro;

        ShowIntro();
    }

    private void ShowIntro()
    {
        dialogueUI.Show(
            "Look at all this empty land...",
            ShowBuildFarmDialogue
        );
    }

    private void ShowBuildFarmDialogue()
    {
        dialogueUI.SetDialogue(
            "Let's turn this desert into something extraordinary! First, let's build a farm.",
            StartBuildFarmStep
        );
    }

    private void StartBuildFarmStep()
    {
        currentStep = TutorialStep.BuildFarm;

        dialogueUI.Hide(OpenBuildingShop);
    }

    private void OpenBuildingShop()
    {
        ShopSystem.Instance.OpenShop(ShopCategory.Buildings);
    }
}