using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public ScrollRect itemView;
    public Sprite tabIdle;
    public Sprite tabActive;

    public List<RectTransform> tabContents = new List<RectTransform>();
    public List<TabButton> tabButtons = new List<TabButton>();
    public List<GameObject> objectsToSwap = new List<GameObject>();

    public event Action<int> TabSelected;

    [NonSerialized]
    public TabButton selectedTab;

    private void Awake()
    {
        InitializeTabButtons();
    }

    private void InitializeTabButtons()
    {
        tabButtons.Clear();

        TabButton[] buttons = GetComponentsInChildren<TabButton>(true);

        foreach (TabButton button in buttons)
        {
            if (button.TabGroup == this)
            {
                tabButtons.Add(button);
            }
        }

        SortButtonsByHierarchy();
    }

    public void Subscribe(TabButton button)
    {
        if (tabButtons.Contains(button))
            return;

        tabButtons.Add(button);
        SortButtonsByHierarchy();
    }

    private void SortButtonsByHierarchy()
    {
        tabButtons.Sort(
            (a, b) => a.transform.GetSiblingIndex()
                .CompareTo(b.transform.GetSiblingIndex())
        );
    }

    private void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            button.background.sprite = tabIdle;
            button.SetSelected(false);
        }
    }

    public void SelectTabByIndex(int index)
    {
        if (index < 0 || index >= tabButtons.Count)
        {
            Debug.LogWarning($"TabGroup: invalid tab index {index}");
            return;
        }

        OnTabSelected(tabButtons[index]);
    }

    public void OnTabSelected(TabButton button)
    {
        ResetTabs();

        selectedTab = button;

        button.background.sprite = tabActive;
        button.SetSelected(true);

        int index = tabButtons.IndexOf(button);

        TabSelected?.Invoke(index);

        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            objectsToSwap[i].SetActive(i == index);
        }

        if (itemView != null &&
            tabContents.Count > index &&
            tabContents[index] != null)
        {
            itemView.content = tabContents[index];
            itemView.normalizedPosition = new Vector2(0, 1);
        }
    }
}