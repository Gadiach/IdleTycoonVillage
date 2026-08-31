using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MissionPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform content;
    [SerializeField] private MissionItemUI missionPrefab;
    [SerializeField] private MissionIconDatabase missionIconDatabase;

    [Header("Animation")]
    [SerializeField] private float hiddenOffset = 300f;
    [SerializeField] private float animationDuration = 0.4f;

    private readonly List<MissionItemUI> missionItems = new();

    private RectTransform missionPanel;
    private Vector2 visiblePosition;
    private Vector2 hiddenPosition;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<MissionListChangedEvent>(OnMissionListChanged);
        EventManager.Instance.AddListener<ShowMissionPanelEvent>(OnShowMissionPanel);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener<MissionListChangedEvent>(OnMissionListChanged);
        EventManager.Instance.RemoveListener<ShowMissionPanelEvent>(OnShowMissionPanel);

        missionPanel.DOKill();
    }

    private void Awake()
    {
        missionPanel = GetComponent<RectTransform>();

        visiblePosition = missionPanel.anchoredPosition;
        hiddenPosition = visiblePosition + new Vector2(0f, hiddenOffset);

        missionPanel.anchoredPosition = hiddenPosition;
    }

    private void OnMissionListChanged(MissionListChangedEvent info)
    {
        UpdateMissionUI(info.Missions);
    }

    private void UpdateMissionUI(List<MissionRuntime> missions)
    {
        RemoveInactiveMissions(missions);
        AddNewMissions(missions);
    }

    private void OnShowMissionPanel(ShowMissionPanelEvent info)
    {
        Show();
    }

    public void Show()
    {
        missionPanel.DOKill();

        missionPanel
            .DOAnchorPos(visiblePosition, animationDuration)
            .SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        missionPanel.DOKill();

        missionPanel
            .DOAnchorPos(hiddenPosition, animationDuration)
            .SetEase(Ease.InBack);
    }

    private void RemoveInactiveMissions(List<MissionRuntime> missions)
    {
        for (int i = missionItems.Count - 1; i >= 0; i--)
        {
            MissionItemUI item = missionItems[i];

            if (missions.Contains(item.Mission))
                continue;

            missionItems.RemoveAt(i);
            Destroy(item.gameObject);
        }
    }

    private void AddNewMissions(List<MissionRuntime> missions)
    {
        foreach (MissionRuntime mission in missions)
        {
            if (HasMissionItem(mission))
                continue;

            MissionItemUI item = Instantiate(missionPrefab, content);

            Sprite missionIcon = missionIconDatabase.GetIcon(
                mission.Data.missionType,
                mission.Data.TargetBusinessType,
                mission.Data.TargetRarity
            );

            item.Initialize(mission, missionIcon);

            missionItems.Add(item);
        }
    }

    private bool HasMissionItem(MissionRuntime mission)
    {
        foreach (MissionItemUI item in missionItems)
        {
            if (item.Mission == mission)
                return true;
        }

        return false;
    }
}