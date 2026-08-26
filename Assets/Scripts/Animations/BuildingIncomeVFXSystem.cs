using UnityEngine;

public class BuildingIncomeVFXSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem coinParticlesPrefab;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<BuildingIncomeCollectedEvent>(OnBuildingIncomeCollected);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<BuildingIncomeCollectedEvent>(OnBuildingIncomeCollected);
    }

    private void OnBuildingIncomeCollected(BuildingIncomeCollectedEvent info)
    {
        ParticleSystem particles = Instantiate(coinParticlesPrefab,info.Position,Quaternion.identity);

        particles.Play();

        Destroy(particles.gameObject,particles.main.duration + particles.main.startLifetime.constantMax);
    }
}