using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public TabGroup TabGroup;

    [Header("UI")]
    public Image background;
    public Image icon;

    [Header("Icons")]
    [SerializeField] private Sprite selectedIcon;
    [SerializeField] private Sprite unselectedIcon;

    [NonSerialized] public bool IsSelected;

    private void Awake()
    {
        if (background == null) background = GetComponent<Image>();

        TabGroup.Subscribe(this);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        icon.sprite = selected ? selectedIcon : unselectedIcon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TabGroup.OnTabSelected(this);
    }
}