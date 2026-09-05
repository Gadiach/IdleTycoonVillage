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

    private bool interactable = true;

    private void Awake()
    {
        if (background == null)
            background = GetComponent<Image>();

        TabGroup.Subscribe(this);
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        icon.sprite = selected ? selectedIcon : unselectedIcon;
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable)
            return;

        TabGroup.OnTabSelected(this);
    }
}