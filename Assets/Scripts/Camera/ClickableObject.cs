using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour
{
    private BuildingData buildingData;

    private void Start()
    {
        buildingData = GetComponent<BuildingData>();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (PanZoom.current != null)
        {
            PanZoom.current.FocusOnObject(transform);
        }

        if (buildingData == null)
        {
            return;
        }

        UIManager.Instance.OpenBuildingPanel(buildingData);
    }
}