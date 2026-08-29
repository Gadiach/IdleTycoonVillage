using UnityEngine;

[CreateAssetMenu(fileName = "WorkerRoundIconDatabase",menuName = "Workers/Worker Round Icon Database")]
public class WorkerRoundIconDatabase : ScriptableObject
{
    [SerializeField] private WorkerRoundIcon[] icons;

    public Sprite GetIcon(BusinessType businessType)
    {
        foreach (WorkerRoundIcon icon in icons)
        {
            if (icon.BusinessType == businessType)
                return icon.Icon;
        }

        Debug.LogWarning($"Worker round icon not found for {businessType}");

        return null;
    }
}

[System.Serializable]
public class WorkerRoundIcon
{
    public BusinessType BusinessType;
    public Sprite Icon;
}