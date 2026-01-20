using System.Collections.Generic;
using UnityEngine;

public class UniversityManager : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject blueprintItemPrefab;
    [SerializeField] private List<BlueprintItem> blueprints;

    public void Initialize()
    {
        Clear();

        foreach (var bp in blueprints)
        {
            Instantiate(blueprintItemPrefab, content)
                .GetComponent<BlueprintItemUI>()
                .Initialize(bp);
        }
    }

    private void Clear()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);
    }
}