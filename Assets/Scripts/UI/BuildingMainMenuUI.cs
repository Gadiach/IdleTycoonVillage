using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMainMenuUI : MonoBehaviour
{
    public static BuildingMainMenuUI Instance;

    [Header("Common UI Elements")]
        
    public GameObject buildingPanel;
    [SerializeField] private GameObject workerPanel;
    [SerializeField] private GameObject noWorkerPanel;
    [SerializeField] private Sprite upgradeButtonActiveSprite;
    [SerializeField] private Sprite upgradeButtonInactiveSprite;
    [SerializeField] private Sprite activeUpgradeArrows;
    [SerializeField] private Sprite inactiveUpgradeArrows;
    [SerializeField] private GameObject blackBackground;

    [Header("Upgrade Indicators")]

    [SerializeField] private GameObject buildingStarUpgradeIndicator;
    [SerializeField] private GameObject workerStarUpgradeIndicator;

    [Header("Automation")]

    [SerializeField] private Button automationInfoButton;
    [SerializeField] private GameObject automationInfoPopup;
    [SerializeField] private TextMeshProUGUI automationInfoText;
    [SerializeField] private AutomationUnlockVFX automationUnlockVFX;
    public TextMeshProUGUI AutomationStatusText;

    [Header("Income")]

    [SerializeField] private TextMeshProUGUI incomeChangeText;
    [SerializeField] private TextMeshProUGUI IncomeText;

    [Header("Time")]

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI timeChangeText;

    [Header("Building Star Upgrade Button UI")]

    [SerializeField] private Button buildingStarPanelOpenButton;
    [SerializeField] private Image[] colorStarsBuilding;
    public Image BuildingImage;

    [Header("Building Level Upgrade Button UI")]

    [SerializeField] private Button buildingLevelUpgradeButton;

    [SerializeField] private GameObject buildingCanUpgradeVisuals;
    [SerializeField] private GameObject buildingNotEnoughCurrencyVisuals;

    [SerializeField] private TextMeshProUGUI buildingUpgradePriceText;
    [SerializeField] private TextMeshProUGUI buildingInactiveUpgradePriceText;

    [SerializeField] private Image buildingUpgradeArrowImage;

    [SerializeField] private TextMeshProUGUI buildingLevelText;

    [SerializeField] private Button buildingGetCurrencyButton;

    [Header("Worker Level Upgrade Button UI")]

    [SerializeField] private Button workerLevelUpgradeButton;

    [SerializeField] private GameObject workerCanUpgradeVisuals;
    [SerializeField] private GameObject workerNotEnoughCurrencyVisuals;

    [SerializeField] private TextMeshProUGUI workerLevelText;

    [SerializeField] private TextMeshProUGUI workerUpgradePriceText;
    [SerializeField] private TextMeshProUGUI workerInactiveUpgradePriceText;

    [SerializeField] private Image workerUpgradeArrowImage;

    [Header("Worker Star Upgrade Button UI")]
    
    [SerializeField] private Button workerStarPanelOpenButton;
    [SerializeField] private Image[] colorStarsWorker;
    public Image WorkerImage;

    [Header("Stats Change VFX")]

    [SerializeField] private float popScale = 1.2f;
    [SerializeField] private float popDuration = 0.15f;
    [SerializeField] private float floatDistance = 30f;
    [SerializeField] private float floatDuration = 0.5f;
    [SerializeField] private Color improvementColor = Color.green;

    private BuildingData currentBuilding;
    private WorkerData currentWorker;

    private Vector3 incomeInitialScale;
    private Vector3 timeInitialScale;

    private Color incomeInitialColor;
    private Color timeInitialColor;

    private Vector2 incomeChangeInitialPosition;
    private Vector2 timeChangeInitialPosition;

    private Vector3 automationStatusInitialScale;

    private void Awake()
    {
        Instance = this;

        automationStatusInitialScale = AutomationStatusText.rectTransform.localScale;

        incomeInitialScale = IncomeText.rectTransform.localScale;
        timeInitialScale = timeText.rectTransform.localScale;

        incomeInitialColor = IncomeText.color;
        timeInitialColor = timeText.color;

        incomeChangeInitialPosition = incomeChangeText.rectTransform.anchoredPosition;
        timeChangeInitialPosition = timeChangeText.rectTransform.anchoredPosition;

        incomeChangeText.gameObject.SetActive(false);
        timeChangeText.gameObject.SetActive(false);

        buildingPanel.SetActive(false);

        automationInfoPopup.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildingUpgradedEvent>(OnBuildingUpgraded);
        EventManager.Instance.AddListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
        EventManager.Instance.AddListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
        EventManager.Instance.AddListener<BuildingTierOrRarityChangedEvent>(OnBuildingTierOrRarityChanged);
        EventManager.Instance.AddListener<WorkerTierOrRarityChangedEvent>(OnWorkerTierOrRarityChanged);
        automationInfoButton.onClick.AddListener(OnAutomationInfoClicked);
        EventManager.Instance.AddListener<CurrencyChangedEvent>(OnCurrencyChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<BuildingUpgradedEvent>(OnBuildingUpgraded);
        EventManager.Instance.RemoveListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
        EventManager.Instance.RemoveListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
        EventManager.Instance.RemoveListener<BuildingTierOrRarityChangedEvent>(OnBuildingTierOrRarityChanged);
        EventManager.Instance.RemoveListener<WorkerTierOrRarityChangedEvent>(OnWorkerTierOrRarityChanged);
        automationInfoButton.onClick.RemoveListener(OnAutomationInfoClicked);
        EventManager.Instance.RemoveListener<CurrencyChangedEvent>(OnCurrencyChanged);
    }

    private void OnCurrencyChanged(CurrencyChangedEvent evt)
    {
        if (!buildingPanel.activeSelf)
            return;

        EvaluateBuildingUpgradeState();

        //if (HasWorker())
        //{
        //    EvaluateWorkerUpgradeState();
        //}

        UpdateUpgradeIndicators();
    }

    private void UpdateUpgradeIndicators()
    {
        if (currentBuilding == null)
            return;

        buildingStarUpgradeIndicator.SetActive(
            currentBuilding.CanUpgradeTierOrRarity
        );

        bool hasWorker = currentWorker != null;

        workerStarUpgradeIndicator.SetActive(
            hasWorker &&
            currentWorker.CanUpgradeTierOrRarity
        );
    }

    private void OnBuildingUpgraded(BuildingUpgradedEvent evt)
    {
        if (currentBuilding != evt.Building)
            return;

        PlayStatChangeVFX(
            IncomeText,
            incomeChangeText,
            incomeInitialScale,
            incomeInitialColor,
            incomeChangeInitialPosition,
            $"+{evt.Building.LastIncomeIncrease}"
        );
    }

    private void OnWorkerUpgraded(WorkerUpgradedEvent evt)
    {
        currentBuilding = evt.Worker.AssignedBuilding;

        if (currentBuilding == null || currentBuilding.Placeable == null)
            return;

        WorkerData assignedWorker = currentBuilding.Placeable.GetAssignedWorker();

        if (assignedWorker != evt.Worker)
            return;

        currentBuilding.CheckAutomationState();

        if (currentWorker != evt.Worker)
            return;

        PlayStatChangeVFX(
            timeText,
            timeChangeText,
            timeInitialScale,
            timeInitialColor,
            timeChangeInitialPosition,
            $"-{evt.Worker.LastCycleDurationDecrease:F2}s"
        );
    }

    private void OnBuildingTierOrRarityChanged(BuildingTierOrRarityChangedEvent evt)
    {
        if (currentBuilding != evt.Building)
            return;

        EvaluateBuildingUpgradeState();
        UpdateBuildingStarUI(currentBuilding);
        UpdateUpgradeIndicators();
    }

    private void OnWorkerTierOrRarityChanged(WorkerTierOrRarityChangedEvent evt)
    {
        if (currentWorker != evt.Worker)
            return;

        //EvaluateWorkerUpgradeState();
        UpdateWorkerStarUI(currentWorker);
        UpdateUpgradeIndicators();
    }

    private void SetUpgradeButtonState(Button button,bool interactable)
    {
        button.interactable = interactable;

        UpdateUpgradeButtonSprite(button,interactable);
    }

    private void UpdateUpgradeArrowSprite(Image arrowImage,bool active)
    {
        arrowImage.sprite = active ? activeUpgradeArrows : inactiveUpgradeArrows;
    }

    private void UpdateUpgradeButtonSprite(Button button, bool interactable)
    {
        button.image.sprite = interactable ? upgradeButtonActiveSprite : upgradeButtonInactiveSprite;
    }

    public void OpenWorkerShop()
    {
        BusinessType workerType =
            currentBuilding.Placeable.AcceptedBusinessType;

        CloseBuildingPanel();

        ShopSystem.Instance.OpenShop(ShopCategory.Workers);

        ShopItemUI targetItem =
        ShopSystem.Instance.GetWorkerItem(workerType);

        if (targetItem != null)
        {
            TutorialHighlightSystem.Instance.Highlight(
                targetItem.IconAndArrow
            );
        }
    }

    private void OnAutomationInfoClicked()
    {
        if (currentBuilding == null)
            return;

        automationInfoPopup.SetActive(!automationInfoPopup.activeSelf);
    }

    private void UpdateAutomationInfo()
    {
        automationInfoPopup.SetActive(false);

        automationInfoText.text = $"Need Worker Lv. {currentBuilding.LevelOfWorkerNeededForAutomation}";
    }

    public void OpenMainBuildingPanel(BuildingData building)
    {
        currentBuilding = building;
        currentWorker = building.Placeable.GetAssignedWorker();

        UpdateAutomationInfo();

        UpdateIncomeText();
        UpdateTimeText();

        UpdateBuildingUpgradePriceText(building);
        BuildingImage.sprite = building.Icon;
        EvaluateBuildingUpgradeState();

        if (HasWorker())
        {
            workerPanel.SetActive(true);
            noWorkerPanel.SetActive(false);

            //UpdateWorkerUpgradePriceText();
            //UpdateWorkerImage();
            //EvaluateWorkerUpgradeState();

            UpdateWorkerStarUI(currentWorker);
        }
        else
        {
            workerPanel.SetActive(false);
            noWorkerPanel.SetActive(true);
        }

        buildingPanel.SetActive(true);
        blackBackground.SetActive(true);

        UpdateBuildingStarUI(building);

        UpdateUpgradeIndicators();

        UpdateAutomationUI(building);

        buildingStarPanelOpenButton.onClick.RemoveAllListeners();
        buildingStarPanelOpenButton.onClick.AddListener(() =>
        {
            BuildingUI.Instance.OpenBuildingPanel(building);
        });

        workerStarPanelOpenButton.onClick.RemoveAllListeners();

        workerStarPanelOpenButton.onClick.AddListener(() =>
        {
            WorkerUI.Instance.OpenWorkerPanel(currentWorker);
        });
    }


    public void OnUpgradeBuildingLvlBtnClicked()
    {
        currentBuilding.UpgradeBuildingLvl();

        UpdateIncomeText();

        UpdateBuildingUpgradePriceText(currentBuilding);

        EventManager.Instance.QueueEvent(new XPAddedEvent(currentBuilding.CurrentLevel - 1));
    }

    public void OnUpgradeWorkerLvlBtnClicked()
    {
        currentWorker.UpgradeWorkerLvl();

        UpdateTimeText();

        UpdateWorkerUpgradePriceText();

        currentBuilding.CheckAutomationState();
    }

    private void UpdateWorkerImage()
    {
        WorkerImage.sprite = currentWorker.Icon;
    }

    private void UpdateWorkerUpgradePriceText()
    {
        string price = currentWorker.PriceToUpgradeLevel.ToString();

        workerUpgradePriceText.text = price;
        workerInactiveUpgradePriceText.text = price;
    }

    private void UpdateBuildingUpgradePriceText(BuildingData building)
    {
        string price = building.PriceToUpgradeLevel.ToString();

        buildingUpgradePriceText.text = price;
        buildingInactiveUpgradePriceText.text = price;
    }

    private void UpdateIncomeText()
    {
        if (currentWorker == null)
        {
            IncomeText.text = "Income: --";
            return;
        }

        IncomeText.text = $"Income: {currentBuilding.IncomePerCycle}";
    }

    private void UpdateTimeText()
    {
        if (currentWorker == null)
        {
            timeText.text = "Time: --";
            return;
        }

        timeText.text = $"Time: {currentWorker.CycleDuration:F2}s";
    }

    private void PlayStatChangeVFX(
        TextMeshProUGUI valueText,
        TextMeshProUGUI changeText,
        Vector3 initialScale,
        Color initialColor,
        Vector2 initialChangePosition,
        string change)
    {
        valueText.DOKill();
        valueText.rectTransform.DOKill();

        changeText.DOKill();
        changeText.rectTransform.DOKill();

        valueText.rectTransform.localScale = initialScale;
        valueText.color = improvementColor;

        valueText.rectTransform
            .DOScale(initialScale * popScale, popDuration)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo);

        valueText
            .DOColor(initialColor, floatDuration)
            .SetEase(Ease.OutQuad);

        changeText.text = change;
        changeText.color = improvementColor;
        changeText.gameObject.SetActive(true);
        changeText.rectTransform.anchoredPosition = initialChangePosition;

        Color color = changeText.color;
        color.a = 1f;
        changeText.color = color;

        Sequence sequence = DOTween.Sequence();

        sequence.Join(
            changeText.rectTransform
                .DOAnchorPosY(initialChangePosition.y + floatDistance, floatDuration)
                .SetEase(Ease.OutQuad)
        );

        sequence.Join(
            changeText
                .DOFade(0f, floatDuration)
                .SetEase(Ease.InQuad)
        );

        sequence.OnComplete(() =>
        {
            changeText.gameObject.SetActive(false);
            changeText.rectTransform.anchoredPosition = initialChangePosition;
        });
    }

    private void EvaluateBuildingUpgradeState()
    {
        if (!currentBuilding.HasEnoughCurrencyForLevelUpgrade)
        {
            SetBuildingUpgradeState(UpgradeUIState.NotEnoughCurrency);

            return;
        }

        if (currentBuilding.IsMaxLevel)
        {
            SetBuildingUpgradeState(UpgradeUIState.NeedTierUpgrade);

            return;
        }

        SetBuildingUpgradeState(UpgradeUIState.CanUpgradeLevel);
    }

    private void EvaluateWorkerUpgradeState()
    {
        if (!currentWorker.HasEnoughCurrencyForLevelUpgrade)
        {
            SetWorkerUpgradeState(UpgradeUIState.NotEnoughCurrency);

            return;
        }

        if (currentWorker.IsMaxLevel)
        {
            SetWorkerUpgradeState(UpgradeUIState.NeedTierUpgrade);

            return;
        }

        SetWorkerUpgradeState(UpgradeUIState.CanUpgradeLevel);
    }

    private void SetWorkerUpgradeState(UpgradeUIState state)
    {
        switch (state)
        {
            case UpgradeUIState.CanUpgradeLevel:
                ApplyCanUpgradeWorkerLevelUI();
                break;

            case UpgradeUIState.NotEnoughCurrency:
                ApplyNotEnoughWorkerCurrencyUI();
                break;

            case UpgradeUIState.NeedTierUpgrade:
                ApplyNeedWorkerTierUpgradeUI();
                break;
        }
    }

    private void ApplyCanUpgradeWorkerLevelUI()
    {
        SetWorkerLevelTextWithRedMaxLevel();

        SetUpgradeButtonState(workerLevelUpgradeButton,true);

        workerCanUpgradeVisuals.SetActive(true);
        workerNotEnoughCurrencyVisuals.SetActive(false);

        UpdateUpgradeArrowSprite(workerUpgradeArrowImage,true);
    }

    private void ApplyNotEnoughWorkerCurrencyUI()
    {
        SetWorkerLevelTextWithRedMaxLevel();

        SetUpgradeButtonState(workerLevelUpgradeButton,false);

        workerCanUpgradeVisuals.SetActive(false);
        workerNotEnoughCurrencyVisuals.SetActive(true);

        UpdateUpgradeArrowSprite(workerUpgradeArrowImage,false);
    }

    private void ApplyNeedWorkerTierUpgradeUI()
    {
        SetWorkerLevelTextMaxed();

        SetUpgradeButtonState(workerLevelUpgradeButton,false);

        workerCanUpgradeVisuals.SetActive(true);
        workerNotEnoughCurrencyVisuals.SetActive(false);

        UpdateUpgradeArrowSprite(workerUpgradeArrowImage,false);
    }

    private void SetBuildingUpgradeState(UpgradeUIState state)
    {
        switch (state)
        {
            case UpgradeUIState.CanUpgradeLevel:
                ApplyCanUpgradeBuildingLevelUI();
                break;

            case UpgradeUIState.NotEnoughCurrency:
                ApplyNotEnoughBuildingCurrencyUI();
                break;

            case UpgradeUIState.NeedTierUpgrade:
                ApplyNeedBuildingTierUpgradeUI();
                break;
        }
    }

    private void ApplyCanUpgradeBuildingLevelUI()
    {
        SetBuildingLevelTextWithRedMaxLevel();

        SetUpgradeButtonState(buildingLevelUpgradeButton,true);

        buildingCanUpgradeVisuals.SetActive(true);
        buildingNotEnoughCurrencyVisuals.SetActive(false);

        UpdateUpgradeArrowSprite(buildingUpgradeArrowImage,true);
    }

    private void ApplyNotEnoughBuildingCurrencyUI()
    {
        SetBuildingLevelTextWithRedMaxLevel();

        SetUpgradeButtonState(buildingLevelUpgradeButton,false);

        buildingCanUpgradeVisuals.SetActive(false);
        buildingNotEnoughCurrencyVisuals.SetActive(true);

        UpdateUpgradeArrowSprite(buildingUpgradeArrowImage,false);
    }

    private void ApplyNeedBuildingTierUpgradeUI()
    {
        SetBuildingLevelTextMaxed();

        SetUpgradeButtonState(buildingLevelUpgradeButton,false);

        buildingCanUpgradeVisuals.SetActive(true);
        buildingNotEnoughCurrencyVisuals.SetActive(false);

        UpdateUpgradeArrowSprite(buildingUpgradeArrowImage,false);
    }

    private void SetBuildingLevelTextWithRedMaxLevel()
    {
        buildingLevelText.text = $"Lv: {currentBuilding.CurrentLevel} / " + $"<color=red>{currentBuilding.CurrentProgressionMaxLevel}</color>";
    }

    private void SetWorkerLevelTextWithRedMaxLevel()
    {
        workerLevelText.text = $"Lv: {currentWorker.CurrentLevel} / " + $"<color=red>{currentWorker.CurrentProgressionMaxLevel}</color>";
    }

    private void SetBuildingLevelTextMaxed()
    {
        buildingLevelText.text = $"Lv: <color=red>{currentBuilding.CurrentLevel} / " +
                                 $"{currentBuilding.CurrentProgressionMaxLevel}</color>";
    }

    private void SetWorkerLevelTextMaxed()
    {
        workerLevelText.text = $"Lv: <color=red>{currentWorker.CurrentLevel} / " +
                               $"{currentWorker.CurrentProgressionMaxLevel}</color>";
    }

    private Color GetColorByRarity(Rarities rarity)
    {
        return rarity switch
        {
            Rarities.Primitive => Color.blue,
            Rarities.Developed => Color.green,
            Rarities.Industrial => Color.yellow,
            Rarities.Modern => new Color(0.5f, 0, 1),
            Rarities.Futuristic => Color.red,
            _ => Color.grey
        };
    }

    private void UpdateWorkerStarUI(WorkerData worker)
    {
        int tierValue = (int)worker.CurrentTier;
        int maxStars = colorStarsWorker.Length;
        Color activeColor = GetColorByRarity(worker.CurrentRarity);
        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
        {
            colorStarsWorker[i].color = (i < tierValue) ? activeColor : inactiveColor;
        }
    }

    private void UpdateBuildingStarUI(BuildingData building)
    {
        int tierValue = (int)building.CurrentTier;
        int maxStars = colorStarsBuilding.Length;
        Color activeColor = GetColorByRarity(building.CurrentRarity);
        Color inactiveColor = Color.grey;

        for (int i = 0; i < maxStars; i++)
        {
            colorStarsBuilding[i].color = (i < tierValue) ? activeColor : inactiveColor;
        }
    }

    public void CloseBuildingPanel()
    {
        automationInfoPopup.SetActive(false);
        buildingPanel.SetActive(false);
        blackBackground.SetActive(false);
    }

    private void UpdateAutomationUI(BuildingData building)
    {
        if (building.IsAutomated)
        {
            AutomationStatusText.text = "ON";
            AutomationStatusText.color = Color.green;

            automationInfoButton.gameObject.SetActive(false);
            automationInfoPopup.SetActive(false);
        }
        else
        {
            AutomationStatusText.text = "OFF";
            AutomationStatusText.color = Color.red;

            automationInfoButton.gameObject.SetActive(true);
            automationInfoPopup.SetActive(false);
        }
    }

    private void OnAutomationChanged(BuildingAutomationChangedEvent evt)
    {
        if (currentBuilding != evt.Building)
            return;

        UpdateAutomationUI(currentBuilding);

        if (currentBuilding.IsAutomated)
        {
            PlayAutomationStatusVFX(); 
            automationUnlockVFX.Play();
        }
    }

    private void PlayAutomationStatusVFX()
    {
        AutomationStatusText.rectTransform.DOKill();

        AutomationStatusText.rectTransform.localScale = automationStatusInitialScale;

        AutomationStatusText.rectTransform
            .DOScale(automationStatusInitialScale * popScale, popDuration)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo);
    }

    private bool HasWorker()
    {
        return currentWorker != null;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}