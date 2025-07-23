using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public TabGroup TabGroup;
    [NonSerialized] public Image background;

    private void Awake()
    {
        background = GetComponent<Image>();
        TabGroup.Subscribe(button: this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TabGroup.OnTabSelected(button: this);
    }
}
