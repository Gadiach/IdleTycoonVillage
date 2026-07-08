using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private IPlaceable placeable;

    private Vector3 startPos;
    private float deltaX, deltaY;

    private void Start()
    {
        startPos = Input.mousePosition;
        startPos = Camera.main.ScreenToWorldPoint(startPos);

        deltaX = startPos.x - transform.position.x;
        deltaY = startPos.y - transform.position.y;

        placeable = GetComponent<IPlaceable>();
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 pos = new Vector3(mousePos.x - deltaX, mousePos.y - deltaY);

        if (placeable.UseGridSnapping)
        {
            Vector3Int cellPos = GridPlacementSystem.current.gridLayout.WorldToCell(pos);
            transform.position = GridPlacementSystem.current.gridLayout.CellToLocalInterpolated(cellPos);
        }
        else
        {
            transform.position = pos;
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            placeable?.CheckPlacement();

            Destroy(this);
        }
    }
}
