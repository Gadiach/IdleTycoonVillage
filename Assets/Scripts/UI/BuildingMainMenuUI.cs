using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMainMenuUI : MonoBehaviour
{
    public static BuildingMainMenuUI Instance;

    [Header("Common UI Elements")]
    public GameObject buildingPanel;
    public TextMeshProUGUI AutomationStatusText;
    [SerializeField] private TextMeshProUGUI IncomeText;
    [SerializeField] private TextMeshProUGUI TimeText;
    [SerializeField] private TextMeshProUGUI incomeChangeText;
    [SerializeField] private TextMeshProUGUI timeChangeText;
    [SerializeField] private GameObject workerPanel;
    [SerializeField] private GameObject noWorkerPanel;
    [SerializeField] private Sprite upgradeButtonActiveSprite;
    [SerializeField] private Sprite upgradeButtonInactiveSprite;
    [SerializeField] private Sprite activeUpgradeArrows;
    [SerializeField] private Sprite inactiveUpgradeArrows;
    [SerializeField] private GameObject blackBackground;
    [SerializeField] private AutomationUnlockVFX automationUnlockVFX;

    [Header("Building UI Elements")]

    public TextMeshProUGUI BuildingLevelText;
    public TextMeshProUGUI BuildingUpgradePriceText;
    public Image BuildingImage;
    [SerializeField] private Image BuildingUpgradeArrowImage;
    [SerializeField] private Button BuildingUpgradeButton;
    [SerializeField] private Image[] colorStarsBuilding;

    [Header("Worker UI Elements")]

    public TextMeshProUGUI WorkerLevelText;
    public TextMeshProUGUI WorkerUpgradePriceText;
    public Image WorkerImage;
    [SerializeField] private Image WorkerUpgradeArrowImage;
    [SerializeField] private Button WorkerUpgradeButton;
    [SerializeField] private Image[] colorStarsWorker;

    [Header("Buttons to open panels")]

    [SerializeField] private Button buildingButton;
    [SerializeField] private Button workerButton;

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
        timeInitialScale = TimeText.rectTransform.localScale;

        incomeInitialColor = IncomeText.color;
        timeInitialColor = TimeText.color;

        incomeChangeInitialPosition = incomeChangeText.rectTransform.anchoredPosition;
        timeChangeInitialPosition = timeChangeText.rectTransform.anchoredPosition;

        incomeChangeText.gameObject.SetActive(false);
        timeChangeText.gameObject.SetActive(false);

        buildingPanel.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildingUpgradedEvent>(OnBuildingUpgraded);
        EventManager.Instance.AddListener<WorkerUpgradedEvent>(OnWorkerUpgraded);
        EventManager.Instance.AddListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
        EventManager.Instance.AddListener<BuildingTierOrRarityChangedEvent>(OnBuildingTierOrRarityChanged);
        EventManager.Instance.AddListener<WorkerTierOrRarityChangedEvent>(OnWorkerTierOrRarityChanged);
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
            TimeText,
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
    }

    private void OnWorkerTierOrRarityChanged(WorkerTierOrRarityChangedEvent evt)
    {
        if (currentWorker != evt.Worker)
            return;

        EvaluateWorkerUpgradeState();
        UpdateWorkerStarUI(currentWorker);
    }

    private void SetUpgradeButtonState(Button button, Image arrowImage, bool interactable)
    {
        button.interactable = interactable;

        UpdateUpgradeButtonSprite(button, interactable);
        UpdateUpgradeArrowSprite(arrowImage, interactable);
    }

    private void UpdateUpgradeButtonSprite(Button button, bool interactable)
    {
        button.image.sprite = interactable ? upgradeButtonActiveSprite : upgradeButtonInactiveSprite;
    }

    private void UpdateUpgradeArrowSprite(Image arrowImage, bool interactable)
    {
        arrowImage.sprite = interactable ? activeUpgradeArrows : inactiveUpgradeArrows;
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

    public void OpenMainBuildingPanel(BuildingData building)
    {
        currentBuilding = building;
        currentWorker = building.Placeable.GetAssignedWorker();

        UpdateIncomeText();
        UpdateTimeText();

        UpdateBuildingUpgradePriceText(building);
        BuildingImage.sprite = building.Icon;
        EvaluateBuildingUpgradeState();

        if (HasWorker())
        {
            workerPanel.SetActive(true);
            noWorkerPanel.SetActive(false);

            UpdateWorkerUpgradePriceText();
            UpdateWorkerImage();
            EvaluateWorkerUpgradeState();

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

        UpdateAutomationUI(building);

        buildingButton.onClick.RemoveAllListeners();
        buildingButton.onClick.AddListener(() =>
        {
            BuildingUI.Instance.OpenBuildingPanel(building);
        });

        workerButton.onClick.RemoveAllListeners();

        workerButton.onClick.AddListener(() =>
        {
            WorkerUI.Instance.OpenWorkerPanel(currentWorker);
        });
    }

    public void OnUpgradeBuildingLvlBtnClicked()
    {
        currentBuilding.UpgradeBuildingLvl();

        UpdateIncomeText();

        EvaluateBuildingUpgradeState();

        UpdateBuildingUpgradePriceText(currentBuilding);

        EventManager.Instance.QueueEvent(new XPAddedEvent(currentBuilding.CurrentLevel - 1));
    }

    public void OnUpgradeWorkerLvlBtnClicked()
    {
        currentWorker.UpgradeWorkerLvl();

        UpdateTimeText();

        EvaluateWorkerUpgradeState();

        UpdateWorkerUpgradePriceText();

        currentBuilding.CheckAutomationState();
    }

    private void UpdateWorkerImage()
    {
        WorkerImage.sprite = currentWorker.Icon;
    }

    private void UpdateWorkerUpgradePriceText()
    {
        WorkerUpgradePriceText.text = currentWorker.PriceToUpgrade.ToString();
    }

    private void UpdateBuildingUpgradePriceText(BuildingData building)
    {
        BuildingUpgradePriceText.text = building.PriceToUpgrade.ToString();
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
            TimeText.text = "Time: --";
            return;
        }

        TimeText.text = $"Time: {currentWorker.CycleDuration:F2}s";
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
        if (currentBuilding.CurrentLevel >= currentBuilding.CurrentProgressionMaxLevel)
        {
            SetBuildingUpgradeState(UpgradeUIState.NeedTierUpgrade);
        }
        else
        {
            SetBuildingUpgradeState(UpgradeUIState.CanUpgradeLevel);
        }
    }

    private void EvaluateWorkerUpgradeState()
    {
        if (currentWorker.CurrentLevel >= currentWorker.CurrentProgressionMaxLevel)
        {
            SetWorkerUpgradeState(UpgradeUIState.NeedTierUpgrade);
        }
        else
        {
            SetWorkerUpgradeState(UpgradeUIState.CanUpgradeLevel);
        }
    }

    private void SetWorkerUpgradeState(UpgradeUIState state)
    {
        switch (state)
        {
            case UpgradeUIState.CanUpgradeLevel:
                ApplyCanUpgradeWorkerLevelUI();
                break;

            case UpgradeUIState.NeedTierUpgrade:
                ApplyNeedWorkerTierUpgradeUI();
                break;
        }
    }

    private void ApplyNeedWorkerTierUpgradeUI()
    {
        SetWorkerLevelTextMaxed();

        SetUpgradeButtonState(WorkerUpgradeButton, WorkerUpgradeArrowImage, false);

        WorkerUpgradePriceText.color = Color.gray;
    }

    private void ApplyCanUpgradeWorkerLevelUI()
    {
        SetWorkerLevelTextWithRedMaxLevel();

        bool canAfford = CanAffordWorkerUpgrade();

        WorkerUpgradePriceText.color = canAfford ? Color.white : Color.red;

        SetUpgradeButtonState(WorkerUpgradeButton, WorkerUpgradeArrowImage, canAfford);
    }

    private void SetBuildingUpgradeState(UpgradeUIState state)
    {
        switch (state)
        {
            case UpgradeUIState.CanUpgradeLevel:
                ApplyCanUpgradeBuildingLevelUI();
                break;

            case UpgradeUIState.NeedTierUpgrade:
                ApplyNeedBuildingTierUpgradeUI();
                break;
        }
    }

    private void ApplyCanUpgradeBuildingLevelUI()
    {
        SetBuildingLevelTextWithRedMaxLevel();

        bool canAfford = CanAffordBuildingUpgrade();

        BuildingUpgradePriceText.color = canAfford ? Color.white : Color.red;

        SetUpgradeButtonState(BuildingUpgradeButton, BuildingUpgradeArrowImage, canAfford);
    }

    private void SetBuildingLevelTextWithRedMaxLevel()
    {
        BuildingLevelText.text = $"Lv: {currentBuilding.CurrentLevel} / " +
                                 $"<color=red>{currentBuilding.CurrentProgressionMaxLevel}</color>";
    }

    private void SetWorkerLevelTextWithRedMaxLevel()
    {
        WorkerLevelText.text = $"Lv: {currentWorker.CurrentLevel} / " +
                               $"<color=red>{currentWorker.CurrentProgressionMaxLevel}</color>";
    }

    private void ApplyNeedBuildingTierUpgradeUI()
    {
        SetBuildingLevelTextMaxed();

        SetUpgradeButtonState(BuildingUpgradeButton, BuildingUpgradeArrowImage, false);

        BuildingUpgradePriceText.color = Color.gray;
    }

    private void SetBuildingLevelTextMaxed()
    {
        BuildingLevelText.text = $"Lv: <color=red>{currentBuilding.CurrentLevel} / " +
                                 $"{currentBuilding.CurrentProgressionMaxLevel}</color>";
    }

    private void SetWorkerLevelTextMaxed()
    {
        WorkerLevelText.text = $"Lv: <color=red>{currentWorker.CurrentLevel} / " +
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
        buildingPanel.SetActive(false);
        blackBackground.SetActive(false);
    }

    private void UpdateAutomationUI(BuildingData building)
    {
        if (building.IsAutomated)
        {
            AutomationStatusText.text = "ON";
            AutomationStatusText.color = Color.green;
        }
        else
        {
            AutomationStatusText.text = "OFF";
            AutomationStatusText.color = Color.red;
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

    private bool CanAffordBuildingUpgrade()
    {
        return CurrencySystem.Instance.HasEnoughCurrency(
            currentBuilding.Currency,
            currentBuilding.PriceToUpgrade
        );
    }

    private bool HasWorker()
    {
        return currentWorker != null;
    }

    private bool CanAffordWorkerUpgrade()
    {
        return CurrencySystem.Instance.HasEnoughCurrency(
            currentWorker.Currency,
            currentWorker.PriceToUpgrade
        );
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}