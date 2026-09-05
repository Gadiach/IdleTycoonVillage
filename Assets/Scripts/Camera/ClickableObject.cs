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
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (buildingData == null)
            return;

        if (TutorialSystem.Instance != null &&
            !TutorialSystem.Instance.CanOpenBuilding(buildingData))
        {
            return;
        }

        ShopSystem.Instance.TryCloseShop();

        if (PanZoom.current != null)
        {
            PanZoom.current.FocusOnObject(transform);
        }

        if (buildingData.BusinessType == BusinessType.Science)
        {
            UniversityUI.Instance.OpenUniversityPanel();
        }
        else
        {
            BuildingMainMenuUI.Instance.OpenMainBuildingPanel(buildingData);

            EventManager.Instance.QueueEvent(
                new BuildingClickedEvent(buildingData)
            );
        }
    }
}