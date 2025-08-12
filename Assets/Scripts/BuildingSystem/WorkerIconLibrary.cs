using UnityEngine;

[CreateAssetMenu(fileName = "WorkerIconLibrary", menuName = "Game/Worker Icon Library")]
public class WorkerIconLibrary : ScriptableObject
{
    [Header("Worker Icons")]
    public Sprite farmingIcon;
    public Sprite engineeringIcon;
    public Sprite scienceIcon;

    [Header("Worker Round Icons")]
    public Sprite farmingRoundIcon;
    public Sprite engineeringRoundIcon;
    public Sprite scienceRoundIcon;

    public Sprite GetIcon(BusinessType type)
    {
        switch (type)
        {
            case BusinessType.Farming:
                return farmingIcon;
            case BusinessType.Engineering:
                return engineeringIcon;
            case BusinessType.Science:
                return scienceIcon;
            default:
                return null;
        }
    }

    public Sprite GetRoundIcon(BusinessType type)
    {
        switch (type)
        {
            case BusinessType.Farming:
                return farmingRoundIcon;
            case BusinessType.Engineering:
                return engineeringRoundIcon;
            case BusinessType.Science:
                return scienceRoundIcon;
            default:
                return null;
        }
    }
}