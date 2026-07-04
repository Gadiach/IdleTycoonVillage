using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout;
    public Tilemap MainTilemap;
    public TileBase takenTile;

    Dictionary<Vector3Int, BuildingData> occupiedTiles = new Dictionary<Vector3Int, BuildingData>();

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Tilemap Management

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }

        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileBase tileBase, Tilemap tilemap)
    {
        TileBase[] tileArray = new TileBase[area.size.x * area.size.y];
        FillTiles(tileArray, tileBase);
        tilemap.SetTilesBlock(area, tileArray);
    }

    private static void FillTiles(TileBase[] arr, TileBase tileBase)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = tileBase;
        }
    }

    public void ClearArea(BoundsInt area, Tilemap tilemap)
    {
        SetTilesBlock(area, null, tilemap);
    }

    #endregion

    #region Building Placement


    public void InitializeWithObject(GameObject prefab, Vector3 pos, ShopItem item)
    {
        pos.z = 0;

        Vector3Int cellPos = gridLayout.WorldToCell(pos);
        Vector3 position = gridLayout.CellToLocalInterpolated(cellPos);

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        obj.AddComponent<ObjectDrag>();

        var placeable = obj.GetComponent<BuildingPlaceable>();
        if (placeable != null)
        {
            var data = obj.GetComponent<BuildingData>();
            if (data != null)
            {
                Debug.Log("Has BuildingData: True");
                data.Initialize(item);
                data.SetPlaceable(placeable);
            }
            else
            {
                Debug.LogError("Building prefab missing BuildingData!");
            }

            placeable.Initialize(item.PurchasePrice, item.Currency);
            //PanZoom.current.FollowObject(obj.transform);
            return;
        }

        var workerPlaceable = obj.GetComponent<WorkerPlaceable>();
        if (workerPlaceable != null)
        {
            //PanZoom.current.FollowObject(obj.transform);
            return;
        }
    }

    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);

        foreach (var b in baseArray)
        {
            if (b == takenTile)
            {
                return false;
            }
        }

        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, takenTile, MainTilemap);

    }

    #endregion
}


