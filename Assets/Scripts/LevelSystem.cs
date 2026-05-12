using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    private int XPNow;
    public static int Level;
    private int xpToNext;

    [SerializeField] private LevelConfig levelConfig; 
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject lvlWindowPrefab;

    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI lvlText;
    [SerializeField] private Image starImage;

    private static Dictionary<int, LevelConfig.LevelData> levelDataDict;

    private void Awake()
    {
        if (levelConfig == null)
        {
            Debug.LogError("LevelConfig is not set in inspector");
            return;
        }

        levelDataDict = levelConfig.GetLevelDictionary();

        if (!levelDataDict.TryGetValue(Level, out LevelConfig.LevelData data))
        {
            Debug.LogError($"No data for level {Level}!");
            return;
        }

        xpToNext = data.xpToNext;
    }

    private void Start()
    {
        Debug.Log("LevelSystem Start() called");
        EventManager.Instance.AddListener<XPAddedEvent>(OnXPAdded);
        EventManager.Instance.AddListener<LevelChangedEvent>(OnLevelChanged);

        UpdateUI();
    }

    private void UpdateUI()
    {
        float fill = (float)XPNow / xpToNext;
        slider.value = fill;
        xpText.text = XPNow + "/" + xpToNext;
    }

    private void OnXPAdded(XPAddedEvent info)
    {
        XPNow += info.amount;
        UpdateUI();

        if (XPNow >= xpToNext)
        {
            Level++;

            if (levelDataDict.TryGetValue(Level, out LevelConfig.LevelData data))
            {
                XPNow -= xpToNext;
                xpToNext = data.xpToNext;
                LevelChangedEvent levelChange = new LevelChangedEvent(Level);
                EventManager.Instance.QueueEvent(levelChange);
            }
            else
            {
                Debug.LogError($"No data for level {Level}!");
            }
        }
    }

    private void OnLevelChanged(LevelChangedEvent info)
    {
        if (!levelDataDict.TryGetValue(info.newLvl, out LevelConfig.LevelData data))
        {
            Debug.LogError($"No data for level {info.newLvl}!");
            return;
        }

        xpToNext = data.xpToNext; 
        XPNow = Mathf.Max(0, XPNow); 
        lvlText.text = (info.newLvl + 1).ToString();
        UpdateUI();

        GameObject window = Instantiate(lvlWindowPrefab, GameManager.current.canvas.transform);

        window.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(window);
        });

        EventManager.Instance.QueueEvent(new RequestCurrencyChangeEvent(data.coinsReward, CurrencyType.Coins));
        EventManager.Instance.QueueEvent(new RequestCurrencyChangeEvent(data.crystalsReward, CurrencyType.Crystals));
    }
}