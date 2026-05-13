using UnityEngine;

public class BlueprintRewardCollect : MonoBehaviour
{  
    private BlueprintItem blueprintItem;

    [SerializeField]
    private BuildingProductionController productionController;

    [SerializeField]
    private AudioSource audioSource;

    public void OnClick()
    {
        audioSource.Play();

        EventManager.Instance.QueueEvent(
            new RequestCurrencyChangeEvent(
                1,
                blueprintItem.Type
            )
        );

        productionController.ResetProduction();
    }
}