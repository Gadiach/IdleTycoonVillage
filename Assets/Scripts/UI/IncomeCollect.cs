using UnityEngine;

public class IncomeCollect : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    [SerializeField] private BuildingProductionController productionController;
    [SerializeField] private AudioSource audioSource;

    public void OnClick()
    {
        audioSource.Play();

        if (buildingData != null)
        {
            buildingData.CollectIncome();

            productionController.CollectReward();
        }
    }
}