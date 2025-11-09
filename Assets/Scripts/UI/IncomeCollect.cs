using UnityEngine;

public class IncomeCollect : MonoBehaviour
{
    private int incomeAmount;
    [SerializeField] private GameObject thisHouseObject;
    [SerializeField] private TimerTooltip timerTooltip;
    [SerializeField] private AudioSource audioSource;

    public void OnClick()
    {
        audioSource.Play();

        BuildingData buildingData = thisHouseObject.GetComponent<BuildingData>(); 

        if (buildingData != null)
        {
            incomeAmount = 5 * buildingData.LevelOfBuilding; 
            EventManager.Instance.QueueEvent(new CurrencyChangeGameEvent(incomeAmount, CurrencyType.Coins));            

            timerTooltip.ResetTimer();            
        }
    }
}