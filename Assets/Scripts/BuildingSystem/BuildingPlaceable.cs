using UnityEngine;
using UnityEngine.UI;

public class BuildingPlaceable : MonoBehaviour, IPlaceable
{
    public bool Placed { get; private set; }

    public BoundsInt area;

    [SerializeField] private GameObject moveButton;
    [SerializeField] private float timeToOpenDragBtn = 0f;

    [SerializeField] private Timer timer;
    [SerializeField] private TimerTooltip timerTooltip;

    [SerializeField] private BusinessType buildingType;
    [SerializeField] private BusinessType acceptedBusinessType;

    [SerializeField] private Image buildingRoundIconImage;

    [SerializeField] private float autoWorkDurationHours = 8f;  
    private float autoWorkTimer = 0f;                            
    private bool isAutomated = false;

    private WorkerData assignedWorker;

    private int objectPrice;
    private CurrencyType currencyType;

    private SpriteRenderer spriteRenderer;
    private Color validColor = Color.white;
    private Color invalidColor = Color.red;

    private bool touching;
    private float time = 0f;
    private Vector3 origin;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                EventManager.Instance.QueueEvent(new BuildingAutomationChangedGameEvent(
                    GetComponent<BuildingData>(), false)); 
            }
        }
        else
        {
            CheckPlacedAndSetTimer();
        }
    }

    public void Initialize(int price, CurrencyType currency)
    {
        objectPrice = price;
        currencyType = currency;
    }

    private void CheckPlacedAndSetTimer()
    {
        if(Placed)
        {
            StartTimer();
        }   
        else
        {
            StopTimer();
        }
    }

    private void ReplaceObject()
    {
        if (!touching && Placed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                time = 0f;
            }
            else if (Input.GetMouseButton(0))
            {
                time += Time.deltaTime;

                if (time > timeToOpenDragBtn)
                {
                    ShowMoveButton();

                    touching = true;
                    gameObject.AddComponent<ObjectDrag>();

                    Vector3Int positionInt = BuildingSystem.current.gridLayout.WorldToCell(transform.position);
                    BoundsInt areaTemp = area;
                    areaTemp.position = positionInt;

                    BuildingSystem.current.ClearArea(areaTemp, BuildingSystem.current.MainTilemap);
                }
            }
        }

        if (touching && Input.GetMouseButtonUp(0))
        {
            touching = false;
        }
    }

    private void ShowMoveButton()
    {
        if (moveButton != null)
        {
            moveButton.SetActive(true); 
            moveButton.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
        }
    }

    public void OnWorkerIconClick()
    {
        if (assignedWorker != null)
        {
            WorkerUI.Instance.OpenWorkerPanel(assignedWorker);
        }
    }

    public void OnMoveButtonClick()
    {
        //moveButton.SetActive(false); 
        gameObject.AddComponent<ObjectDrag>(); 
    }

    public bool CanBePlaced(Vector3 position)
    {
        Vector3Int cellPos = BuildingSystem.current.gridLayout.LocalToCell(position);
        BoundsInt areaTemp = area;
        areaTemp.position = cellPos;

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
        Vector3Int positionInt = BuildingSystem.current.gridLayout.LocalToCell(transform.position);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        Placed = true;

        BuildingSystem.current.TakeArea(areaTemp);

        PanZoom.current.UnfollowObject();
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
        EventManager.Instance.AddListener<BuildingAutomationChangedGameEvent>(OnAutomationChanged);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<BuildingAutomationChangedGameEvent>(OnAutomationChanged);
    }

    private void OnAutomationChanged(BuildingAutomationChangedGameEvent evt)
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
        timer.enabled = true;
        timerTooltip.enabled = true;
    }

    private void StopTimer()
    {
        timer.enabled = false;
        timerTooltip.enabled = false;
    }  
}