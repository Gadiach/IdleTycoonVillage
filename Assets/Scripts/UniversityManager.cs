using UnityEngine;

public class UniversityManager : MonoBehaviour
{
    [System.Serializable]
    private class BlueprintBinding
    {
        public BlueprintItem Item;
        public BlueprintItemUI UI;
    }
    public static UniversityManager Instance;

    [SerializeField]
    private BlueprintBinding[] bindings;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
                binding.UI.UpdateStudyButtonState();
            }

            if (binding.Item.Type == info.CurrencyType)
            {
                binding.UI.UpdateOwnedText();
            }
        }
    }
}