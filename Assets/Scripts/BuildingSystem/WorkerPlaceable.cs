using UnityEngine;

public class WorkerPlaceable : MonoBehaviour, IPlaceable
{
    private bool placed = false;
    private PlaceableObject assignedBuilding;
    private Worker workerData;

    private SpriteRenderer spriteRenderer;
    private Color validColor = Color.white;
    private Color invalidColor = Color.red;

    [SerializeField] private LayerMask placeableLayer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        workerData = GetComponent<Worker>();
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
        if (!workerData.available) return false;

        Collider2D hit = Physics2D.OverlapPoint(position, placeableLayer);
        if (hit == null) return false;
        Debug.Log("Hit object: " + hit.name);

        PlaceableObject building = hit.GetComponent<PlaceableObject>();
        if (building == null) return false;

        if (!building.AcceptsWorkerType(workerData.type)) return false;
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
        workerData.available = false;

        assignedBuilding.AssignWorker(this);
        // треба щоб префаб зникав а в placeableObject назначався в картинці працівника цей працівник. Треба ліст в якому будуть картинки і брати звідти
        //transform.position = assignedBuilding.transform.position + new Vector3(0, 1f, 0);
        Destroy(gameObject);
    }

    public bool IsPlaced() => placed;
}