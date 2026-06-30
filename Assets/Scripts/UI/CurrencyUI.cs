using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [System.Serializable]
    public class CurrencyTextBinding
    {
        public CurrencyType CurrencyType;
        public TextMeshProUGUI Text;
    }

    [SerializeField] private CurrencyTextBinding[] currencyBindings;

    private Dictionary<CurrencyType, TextMeshProUGUI> currencyTexts = new();

    private void OnEnable()
    {
        EventManager.Instance.AddListener<CurrencyAddedEvent>(OnCurrencyChanged);
        EventManager.Instance.AddListener<CurrencySpentEvent>(OnCurrencyChanged);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<CurrencyAddedEvent>(OnCurrencyChanged);
        EventManager.Instance.RemoveListener<CurrencySpentEvent>(OnCurrencyChanged);
    }

    private void Awake()
    {
        InitializeCurrencyTextBindings();
    }

    private void Start()
    {
        UpdateAllCurrencies();
    }

    private void InitializeCurrencyTextBindings()
    {
        foreach (var binding in currencyBindings)
        {
            if (binding == null || binding.Text == null)
                continue;

            currencyTexts[binding.CurrencyType] = binding.Text;
        }
    }

    private void UpdateAllCurrencies()
    {
        foreach (var pair in currencyTexts)
        {
            pair.Value.text = CurrencySystem.GetCurrencyAmount(pair.Key).ToString();
        }
    }

    private void OnCurrencyChanged(CurrencyAddedEvent info)
    {
        UpdateCurrency(info.CurrencyType);
    }

    private void OnCurrencyChanged(CurrencySpentEvent info)
    {
        UpdateCurrency(info.CurrencyType);
    }

    private void UpdateCurrency(CurrencyType currencyType)
    {
        if (!currencyTexts.TryGetValue(currencyType, out var text))
        {
            Debug.LogWarning($"No UI binding found for currency: {currencyType}");
            return;
        }

        text.text = CurrencySystem.GetCurrencyAmount(currencyType).ToString();
    }
}