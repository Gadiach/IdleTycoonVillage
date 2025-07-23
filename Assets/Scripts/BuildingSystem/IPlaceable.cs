using UnityEngine;

public interface IPlaceable
{
    bool CanBePlaced(Vector3 position);
    void Place(Vector3 position);

    void CheckPlacement();
}