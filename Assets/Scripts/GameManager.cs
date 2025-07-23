using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public GameObject canvas;

    private void Awake()
    {
        current = this;

        ShopItemDrag.canvas = canvas.GetComponent<Canvas>();
    }
}
