using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    public static TutorialSystem Instance;

    [Header("Settings")]
    [SerializeField] private bool playTutorial = true;

    [Header("References")]
    [SerializeField] private TutorialDialogueUI dialogueUI;
    [SerializeField] private TutorialDragHint dragHint;
    [SerializeField] private TutorialTapHint tapHint;

    [Header("Build Farm")]
    [SerializeField] private RectTransform farmPlacementHint;

    private TutorialStep currentStep;
    private BuildingData tutorialFarm;

    [Header("Hire Worker")]
    [SerializeField] private float workerDragTargetYOffset = 2f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        if (!playTutorial)
            return;

        EventManager.Instance.AddListener<ShopItemDragStartedEvent>(OnShopItemDragStarted);
        EventManager.Instance.AddListener<BuildingPlacedEvent>(OnBuildingPlaced);
        EventManager.Instance.AddListener<CurrencyVFXCompletedEvent>(OnCurrencyVFXCompleted);
        EventManager.Instance.AddListener<WorkerAssignedToBuildingEvent>(OnWorkerAssigned);
        EventManager.Instance.AddListener<BuildingIncomeCollectedEvent>(OnIncomeCollected);
        EventManager.Instance.AddListener<BuildingClickedEvent>(OnBuildingClicked);
    }

    private void OnDisable()
    {
        if (!playTutorial)
            return;

        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<ShopItemDragStartedEvent>(OnShopItemDragStarted);
        EventManager.Instance.RemoveListener<BuildingPlacedEvent>(OnBuildingPlaced);
        EventManager.Instance.RemoveListener<CurrencyVFXCompletedEvent>(OnCurrencyVFXCompleted);
        EventManager.Instance.RemoveListener<WorkerAssignedToBuildingEvent>(OnWorkerAssigned);
        EventManager.Instance.RemoveListener<BuildingIncomeCollectedEvent>(OnIncomeCollected);
        EventManager.Instance.RemoveListener<BuildingClickedEvent>(OnBuildingClicked);
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

    #region Intro

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

    private void OnBuildingClicked(BuildingClickedEvent info)
    {
        if (currentStep != TutorialStep.AutomateFarm)
            return;

        if (info.Building != tutorialFarm)
            return;

        tapHint.Stop();

        Debug.Log("Tutorial: Farm clicked");
    }

    #endregion

    #region Build Farm

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
        ShopItemUI farmItem = ShopSystem.Instance.GetBuildingItem(BusinessType.Farming);

        if (farmItem == null)
            return;

        dragHint.Play(
            farmItem.ItemIcon,
            farmPlacementHint,
            farmItem.ItemIconSprite
        );
    }

    private void OnBuildingPlaced(BuildingPlacedEvent info)
    {
        if (currentStep != TutorialStep.BuildFarm)
            return;

        if (info.Building.BusinessType != BusinessType.Farming)
            return;

        tutorialFarm = info.Building;

        CompleteBuildFarmStep();
    }

    private void CompleteBuildFarmStep()
    {
        dragHint.Stop();

        currentStep = TutorialStep.ClaimMissionReward;

        ShowMissionRewardDialogue();
    }

    #endregion

    #region Mission Reward

    private void ShowMissionRewardDialogue()
    {
        dialogueUI.Show(
            "Great! You've completed your first mission!",
            ShowClaimRewardDialogue
        );
    }

    private void ShowClaimRewardDialogue()
    {
        dialogueUI.SetDialogue(
            "Your reward is waiting. Go ahead and claim it!",
            OpenMissions
        );
    }

    private void OpenMissions()
    {
        dialogueUI.Hide(ShowMissionsPanel);
    }

    private void ShowMissionsPanel()
    {
        EventManager.Instance.QueueEvent(new ShowMissionPanelEvent());
    }

    private void OnCurrencyVFXCompleted(CurrencyVFXCompletedEvent info)
    {
        if (currentStep != TutorialStep.ClaimMissionReward)
            return;

        StartHireWorkerStep();
    }

    #endregion

    #region Hire Worker

    private void StartHireWorkerStep()
    {
        currentStep = TutorialStep.HireWorker;

        ShopSystem.Instance.CloseShop();

        if (tutorialFarm != null)
        {
            PanZoom.current.FocusOnObject(tutorialFarm.transform);
        }

        dialogueUI.Show(
            "A business won't run itself! Let's hire a worker.",
            StartWorkerTutorial
        );
    }

    private void StartWorkerTutorial()
    {
        dialogueUI.Hide(OpenWorkerShop);
    }

    private void OpenWorkerShop()
    {
        ShopSystem.Instance.OpenShop(
            ShopCategory.Workers,
            StartWorkerDragHint
        );
    }

    private void StartWorkerDragHint()
    {
        ShopItemUI workerItem = ShopSystem.Instance.GetWorkerItem(BusinessType.Farming);

        if (workerItem == null || tutorialFarm == null)
            return;

        Vector3 targetWorldPosition =
            tutorialFarm.transform.position + Vector3.up * workerDragTargetYOffset;

        Vector3 farmScreenPosition = Camera.main.WorldToScreenPoint(targetWorldPosition);

        dragHint.Play(
            workerItem.ItemIcon,
            farmScreenPosition,
            workerItem.ItemIconSprite
        );
    }

    private void OnWorkerAssigned(WorkerAssignedToBuildingEvent info)
    {
        if (currentStep != TutorialStep.HireWorker)
            return;

        if (info.Building != tutorialFarm)
            return;

        CompleteHireWorkerStep();
    }

    private void CompleteHireWorkerStep()
    {
        dragHint.Stop();

        currentStep = TutorialStep.CollectIncome;

        ShopSystem.Instance.CloseShop();

        dialogueUI.Show(
            "Great! Your farm is running! Now wait for your first income and collect it.",
            WaitForIncome
        );
    }

    private void WaitForIncome()
    {
        dialogueUI.Hide();
    }

    #endregion

    #region Collect Income

    private void OnIncomeCollected(BuildingIncomeCollectedEvent info)
    {
        if (currentStep != TutorialStep.CollectIncome)
            return;

        CompleteCollectIncomeStep();
    }

    private void CompleteCollectIncomeStep()
    {
        currentStep = TutorialStep.AutomateFarm;

        ShowAutomationDialogue();
    }

    #endregion

    #region Automate Farm

    private void ShowAutomationDialogue()
    {
        if (tutorialFarm != null)
        {
            PanZoom.current.FocusOnObject(tutorialFarm.transform);
        }

        dialogueUI.Show(
            "Nice! But collecting income manually every time will slow us down. Let's automate the farm!",
            HideAutomationDialogue
        );
    }

    private void HideAutomationDialogue()
    {
        dialogueUI.Hide(StartFarmTapHint);
    }

    private void StartFarmTapHint()
    {
        if (tutorialFarm == null)
            return;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(
            tutorialFarm.transform.position
        );

        tapHint.Play(screenPosition);
    }

    #endregion

    #region Shop Drag

    private void OnShopItemDragStarted(ShopItemDragStartedEvent info)
    {
        if (currentStep == TutorialStep.BuildFarm)
        {
            if (info.ShopItem.Type != ShopCategory.Buildings)
                return;

            if (info.ShopItem.BusinessType != BusinessType.Farming)
                return;

            dragHint.Stop();
            return;
        }

        if (currentStep == TutorialStep.HireWorker)
        {
            if (info.ShopItem.Type != ShopCategory.Workers)
                return;

            if (info.ShopItem.BusinessType != BusinessType.Farming)
                return;

            dragHint.Stop();
        }
    }

    #endregion
}