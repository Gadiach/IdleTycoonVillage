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
                EventManager.Instance.QueueEvent(new BuildingAutomationChangedEvent(
                    GetComponent<BuildingData>(), false)); 
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
            Debug.Log("Open");
        }
        else
        {
            ShopManager.current.OpenShop(ObjectType.Workers);
        }
    }

    public void OnBuildingIconClick()
    {
        if (assignedWorker != null)
        {
            BuildingUI.Instance.OpenBuildingPanel(buildingData);
        }
    }

    public void OnMoveButtonClick()
    {
        gameObject.AddComponent<ObjectDrag>(); 
    }

    public bool CanBePlaced(Vector3 position)
    {
        Vector3Int cellPos = BuildingSystem.current.gridLayout.LocalToCell(position);
        BoundsInt areaTemp = area;
        areaTemp.position = cellPos + area.position;

        return BuildingSystem.current.CanTakeArea(areaTemp);
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
        Vector3Int cellPos = BuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = cellPos + area.position;

        Placed = true;

        BuildingSystem.current.TakeArea(areaTemp);

        PanZoom.current.UnfollowObject();

        BuildingManager.Instance.AddBuildingToList(buildingData);
    }

    public void CheckPlacement()
    {
        if (!Placed)
        {
            if (CanBePlaced(transform.position))
            {
                Place(transform.position);
                origin = transform.position;

                CurrencySystem.Instance.TrySpendCurrency(CurrencyType.Coins, objectPrice);
                EventManager.Instance.QueueEvent(new BuildingPlacedEvent(buildingData));
            }
            else
            {
                Destroy(transform.gameObject);
            }           

            ShopManager.current.ShopButton_Click();
        }
        else
        {
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
    }

    public bool AcceptsWorkerType(BusinessType type) => type == acceptedBusinessType;

    public bool HasWorker() => assignedWorker != null;

    public WorkerData GetAssignedWorker() => assignedWorker;

    public void AssignWorker(WorkerData worker)
    {
        assignedWorker = worker;
        buildingRoundIconImage.sprite = worker.roundIcon;
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