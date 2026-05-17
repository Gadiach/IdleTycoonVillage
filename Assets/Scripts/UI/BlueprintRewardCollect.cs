using UnityEngine;

public class BlueprintRewardCollect : MonoBehaviour
{
    [SerializeField] private BuildingProductionController productionController;

    [SerializeField] private AudioSource audioSource;

    public void OnClick()
    {
        audioSource.Play();

        BlueprintItem blueprint = productionController.CurrentBlueprint;

        EventManager.Instance.QueueEvent(new RequestCurrencyChangeEvent(1,blueprint.Type));

        productionController.ClearCurrentBlueprint();

        UniversityManager.Instance.FinishStudy();

        productionController.HideReward();
    }
}