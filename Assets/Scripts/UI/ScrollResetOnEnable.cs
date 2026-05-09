using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Resets the target ScrollRect to the top whenever this GameObject is enabled.
/// </summary>
public class ScrollResetOnEnable : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    private void OnEnable()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
