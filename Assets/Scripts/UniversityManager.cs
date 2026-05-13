using UnityEngine;

public class UniversityManager : MonoBehaviour
{
    [System.Serializable]
    private class BlueprintBinding
    {
        public BlueprintItem Item;
        public BlueprintItemUI UI;
    }

    [SerializeField]
    private BlueprintBinding[] bindings;

    private void Start()
    {
        EventManager.Instance.AddListener<CurrencyChangedEvent>(UpdateUI);

        foreach (var binding in bindings)
        {
            binding.UI.Initialize(binding.Item);
        }
    }

    private void UpdateUI(CurrencyChangedEvent info)
    {
        foreach (var binding in bindings)
        {
            if (binding.Item.StudyCurrency == info.CurrencyType)
            {
                binding.UI.Refresh();
            }
        }
    }
}