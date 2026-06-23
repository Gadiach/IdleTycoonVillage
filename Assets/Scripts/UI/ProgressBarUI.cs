using UnityEngine;

[ExecuteAlways]
public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private RectTransform fill;

    [Range(0f, 1f)]
    [SerializeField] private float testValue;

    [SerializeField] private float maxWidth = 200f;

    private void Update()
    {
        if (fill == null)
            return;

        fill.sizeDelta = new Vector2(
            maxWidth * testValue,
            fill.sizeDelta.y);
    }
}