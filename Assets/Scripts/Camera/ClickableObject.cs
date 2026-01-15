using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour
{
    private BuildingData buildingData;

    private void Start()
    {
        buildingData = GetComponent<BuildingData>();
    }

    private void OnMouseUpAsButton()
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

        if(buildingData.BusinessType == BusinessType.Science)
        {
            UniversityUI.Instance.OpenUniversityPanel();
        }
        else
        {
            UIManager.Instance.OpenMainBuildingPanel(buildingData);
        }           
    }
}