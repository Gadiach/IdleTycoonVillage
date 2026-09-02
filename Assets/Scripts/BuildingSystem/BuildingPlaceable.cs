using UnityEngine;
using UnityEngine.UI;

public class BuildingPlaceable : MonoBehaviour, IPlaceable
{
    public bool Placed { get; private set; }

    public BoundsInt area;

    [SerializeField] private GameObject moveButton;
    [SerializeField] private float timeToOpenDragBtn = 0f;

    [SerializeField] private Timer timer;
    [SerializeField] private TimerUI timerUI;
    [SerializeField] private BuildingProductionController productionController;

    [SerializeField] private BusinessType buildingType;
    [SerializeField] private BusinessType acceptedBusinessType;

    [SerializeField] private GameObject addWorkerObject;
    [SerializeField] private GameObject assignedWorkerObject;

    private Image assignedWorkerImage;

    private WorkerIconAnimation workerIconAnimation;
    public BusinessType AcceptedBusinessType => acceptedBusinessType;

    public bool UseGridSnapping => true;

    [SerializeField] private Image buildingRoundIconImage;

    [SerializeField] private float autoWorkDurationHours = 8f;  
    private float autoWorkTimer = 0f;                            
    private bool isAutomated = false;

    private WorkerData assignedWorker;
    public BuildingData buildingData { get; private set; }

    private int objectPrice;
    private CurrencyType currencyType;

    private SpriteRenderer spriteRenderer;
    private Color validColor = Color.white;
    private Color invalidColor = Color.red;

    private Vector3 origin;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildingData = GetComponent<BuildingData>();
        buildingData.SetPlaceable(this);
        assignedWorkerImage = assignedWorkerObject.GetComponent<Image>();
        addWorkerObject.SetActive(true);
        assignedWorkerObject.SetActive(false);
        workerIconAnimation = GetComponentInChildren<WorkerIconAnimation>();
        workerIconAnimation?.StartAnimation();
    }

    private void Update()
    {
        CheckPlacementAndSetColor();
        
        if (isAutomated)
        {
            autoWorkTimer += Time.deltaTime / 3600f; 

            if (autoWorkTimer >= autoWorkDurationHours)
            {
                StopTimer();
                isAutomated = false;
                EventManager.Instance.QueueEvent(new BuildingAutomationChangedEvent(GetComponent<BuildingData>(), false)); 
            }
        }
    }

    public void Initialize(int price, CurrencyType currency)
    {
        objectPrice = price;
        currencyType = currency;
    }

    public void OnWorkerIconClick()
    {
        if (assignedWorker != null)
        {
            WorkerUI.Instance.OpenWorkerPanel(assignedWorker);
        }
        else
        {
            ShopSystem.Instance.OpenShop(ShopCategory.Workers);

            ShopItemUI targetItem = ShopSystem.Instance.GetWorkerItem(acceptedBusinessType);

            if (targetItem != null)
            {
                TutorialHighlightSystem.Instance.Highlight(targetItem.IconAndArrow);
            }
        }
    }

    //public void OnBuildingIconClick()
    //{
    //    if (assignedWorker != null)
    //    {
    //        Debug.Log("BUILDING CLICKED");

    //        BuildingUI.Instance.OpenBuildingPanel(buildingData);

    //        EventManager.Instance.QueueEvent(new BuildingClickedEvent(buildingData));
    //    }
    //}

    public void OnMoveButtonClick()
    {
        gameObject.AddComponent<ObjectDrag>(); 
    }

    public bool CanBePlaced(Vector3 position)
    {
        Vector3Int cellPos = GridPlacementSystem.current.gridLayout.LocalToCell(position);
        BoundsInt areaTemp = area;
        areaTemp.position = cellPos + area.position;

        return GridPlacementSystem.current.CanTakeArea(areaTemp);
    }

    private void CheckPlacementAndSetColor()
    {
        if (!Placed)
        {
            spriteRenderer.color = CanBePlaced(transform.position) ? validColor : invalidColor;
            return;
        }
    }

    public void Place(Vector3 position)
    {
        Vector3Int cellPos = GridPlacementSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = cellPos + area.position;

        Placed = true;

        GridPlacementSystem.current.TakeArea(areaTemp);
    }

    public void CheckPlacement()
    {
        if (!Placed)
        {
            if (CanBePlaced(transform.position))
            {
                Place(transform.position);
                origin = transform.position;

                CurrencySystem.Instance.SpendCurrency(CurrencyType.Coins, objectPrice);
                EntityRegistry.Instance.AddBuilding(buildingData);
                EventManager.Instance.QueueEvent(new BuildingPlacedEvent(buildingData));
            }
            else
            {
                Destroy(gameObject);
                ShopSystem.Instance.OpenShop(ShopCategory.Buildings);
            }

            return;
        }

        if (CanBePlaced(transform.position))
        {
            Place(transform.position);
            origin = transform.position;
        }
        else
        {
            transform.position = origin;
            Place(transform.position);
        }
    }

    public bool AcceptsWorkerType(BusinessType type) => type == acceptedBusinessType;

    public bool HasWorker() => assignedWorker != null;

    public WorkerData GetAssignedWorker() => assignedWorker;

    public void AssignWorker(WorkerData worker)
    {
        assignedWorker = worker;

        Sprite workerIcon = WorkerSystem.Instance.GetRoundIcon(worker.Type);

        assignedWorkerImage.sprite = workerIcon;

        addWorkerObject.SetActive(false);
        assignedWorkerObject.SetActive(true);

        workerIconAnimation?.StopAnimation();

        productionController.SetWorker(worker);
        productionController.StartProduction();
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<BuildingAutomationChangedEvent>(OnAutomationChanged);
    }

    private void OnAutomationChanged(BuildingAutomationChangedEvent evt)
    {
        if (evt.Building.Placeable == this)
        {
            isAutomated = evt.IsAutomated;
            autoWorkTimer = 0f; 

            if (isAutomated)
                StartTimer();    
            else
                StopTimer();   
        }
    }

    private void StartTimer()
    {
        timer.StartTimer();
    }

    private void StopTimer()
    {
        timer.StopTimer();

        timerUI.ResetUI();
    }
}