using UnityEngine;

public class WorkerPlaceable : MonoBehaviour, IPlaceable
{
    private bool placed = false;
    private BuildingPlaceable assignedBuilding;

    private SpriteRenderer spriteRenderer;
    private Color validColor = Color.white;
    private Color invalidColor = Color.red;

    [SerializeField] private LayerMask placeableLayer;
    [SerializeField] private Sprite workerIcon;

    [SerializeField] private BusinessType workerType = BusinessType.Farming;   

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!placed)
        {
            spriteRenderer.color = CanBePlaced(transform.position) ? validColor : invalidColor;
        }        
    }

    public bool CanBePlaced(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapPoint(position, placeableLayer);
        if (hit == null) return false;

        BuildingPlaceable building = hit.GetComponent<BuildingPlaceable>();
        if (building == null) return false;

        if (!building.AcceptsWorkerType(workerType)) return false;

        if (building.HasWorker()) return false;

        assignedBuilding = building;
        return true;
    }

    public void CheckPlacement()
    {
        if (!placed)
        {
            if (CanBePlaced(transform.position))
            {
                Place(transform.position);
            }
            else
            {
                Destroy(gameObject); 
            }
            ShopManager.current.ShopButton_Click();
        }
    }

    public void Place(Vector3 position)
    {
        placed = true;

        WorkerData worker = WorkerManager.current.CreateWorker(workerType, register: true);
        worker.available = false;

        assignedBuilding.AssignWorker(worker);

        Destroy(gameObject);
    }

    public bool IsPlaced() => placed;
}