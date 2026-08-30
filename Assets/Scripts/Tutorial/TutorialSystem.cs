using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    public static TutorialSystem Instance;

    [Header("Settings")]
    [SerializeField] private bool playTutorial = true;

    [Header("References")]
    [SerializeField] private TutorialDialogueUI dialogueUI;
    [SerializeField] private TutorialDragHint dragHint;

    [Header("Build Farm")]
    [SerializeField] private RectTransform farmPlacementHint;

    private TutorialStep currentStep;


    private void OnEnable()
    {
        if (!playTutorial)
            return;

        EventManager.Instance.AddListener<ShopItemDragStartedEvent>(OnShopItemDragStarted);
        EventManager.Instance.AddListener<BuildingPlacedEvent>(OnBuildingPlaced);
    }

    private void OnDisable()
    {
        if (!playTutorial)
            return;

        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<ShopItemDragStartedEvent>(OnShopItemDragStarted);
        EventManager.Instance.RemoveListener<BuildingPlacedEvent>(OnBuildingPlaced);
    }

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

    private void OnShopItemDragStarted(ShopItemDragStartedEvent info)
    {
        if (currentStep != TutorialStep.BuildFarm)
            return;

        if (info.ShopItem.Type != ShopCategory.Buildings)
            return;

        if (info.ShopItem.BusinessType != BusinessType.Farming)
            return;

        dragHint.Stop();
    }

    private void OnBuildingPlaced(BuildingPlacedEvent info)
    {
        if (currentStep != TutorialStep.BuildFarm)
            return;

        if (info.Building.BusinessType != BusinessType.Farming)
            return;

        CompleteBuildFarmStep();
    }

    private void CompleteBuildFarmStep()
    {
        dragHint.Stop();

        currentStep = TutorialStep.HireWorker;

        Debug.Log("Tutorial: Build Farm completed");
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
        ShopSystem.Instance.OpenShop(
            ShopCategory.Buildings,
            StartFarmDragHint
        );
    }

    private void StartFarmDragHint()
    {
        ShopItemUI farmItem =
            ShopSystem.Instance.GetBuildingItem(BusinessType.Farming);

        if (farmItem == null)
            return;

        dragHint.Play(
            farmItem.ItemIcon,
            farmPlacementHint,
            farmItem.ItemIconSprite
        );
    }
}