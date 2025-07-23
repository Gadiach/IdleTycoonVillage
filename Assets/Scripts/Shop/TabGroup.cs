using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TabGroup : MonoBehaviour
{
    public Sprite tabIdle;
    public Sprite tabActive;
    public ScrollRect itemView;

    public List<RectTransform> tabContents = new List<RectTransform>();
    public List<TabButton> tabButtons = new List<TabButton>();
    public List<GameObject> objectsToSwap = new List<GameObject>();

    [NonSerialized]
    public TabButton selectedTab;

    private void Start()
    {
        OnTabSelected(tabButtons[0]);
    }

    public void Subscribe(TabButton button)
    {
        tabButtons.Add(button);
        SortButtonsByHierarchy();
    }

    private void SortButtonsByHierarchy()
    {
        tabButtons.Sort((a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));
    }

    private void ResetTabs()
    {
        foreach (var button in tabButtons)
        {
            if(selectedTab != null && button == selectedTab)
            {
                continue;
            }
            button.background.sprite = tabIdle;
        }
    }

    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;

        int index = button.transform.GetSiblingIndex();

        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }

        if (itemView != null && tabContents.Count > index && tabContents[index] != null)
        {
            itemView.content = tabContents[index];
            itemView.normalizedPosition = new Vector2(0, 1); 
        }
    }
}
