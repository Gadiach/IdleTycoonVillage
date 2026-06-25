using UnityEngine;

public class BlueprintRewardCollect : MonoBehaviour
{
    [SerializeField] private BuildingProductionController productionController;

    [SerializeField] private AudioSource audioSource;

    public void OnClick()
    {
        audioSource.Play();

        BlueprintItem blueprint = productionController.CurrentBlueprint;

        CurrencySystem.Instance.AddCurrency(blueprint.Type, 1);

        productionController.ClearCurrentBlueprint();

        UniversityManager.Instance.FinishStudy();

        productionController.HideReward();
    }
}