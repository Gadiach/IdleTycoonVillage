using UnityEngine;

public class IsoCanvasSorting : MonoBehaviour
{
    private Canvas canvas;
    public int offset = 5000; // Смещение, чтобы Canvas оставался видимым

    void Start()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("IsoCanvasSorting: Canvas component not found!");
            return;
        }

        canvas.overrideSorting = true; // Включаем ручную сортировку
    }

    void Update()
    {
        // Смещаем Canvas выше, чтобы он не оказался слишком глубоко
        canvas.sortingOrder = offset + Mathf.RoundToInt(transform.position.y * -10);
    }
}