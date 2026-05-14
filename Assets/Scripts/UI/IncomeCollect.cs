using UnityEngine;

public class IncomeCollect : MonoBehaviour
{
    private int incomeAmount;
    [SerializeField] private GameObject thisHouseObject;
    [SerializeField] private BuildingProductionController productionController;
    [SerializeField] private AudioSource audioSource;

    public void OnClick()
    {
        audioSource.Play();

        BuildingData buildingData = thisHouseObject.GetComponent<BuildingData>(); 

        if (buildingData != null)
        {
            incomeAmount = 5 * buildingData.LevelOfBuilding * buildingData.TotalIncomeCircles;
            buildingData.ResetTotalIncomeCircles();
            EventManager.Instance.QueueEvent(new RequestCurrencyChangeEvent(incomeAmount, CurrencyType.Coins));
            productionController.ResetProduction();
        }
    }
}