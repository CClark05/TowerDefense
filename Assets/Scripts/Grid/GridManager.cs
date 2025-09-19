using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    private Grid<GridObjectData> grid;
    public Grid<GridObjectData> Grid => grid;
    
    private LevelData levelData;
    private Dictionary<Vector2Int, GameObject> spawnedObjects = new();
    [SerializeField] private GridObjectData emptyData;
    public int CellSize => levelData.cellSize;
    private void Start()
    {
        levelData = LevelDataHolder.Instance.Data;
        grid = new Grid<GridObjectData>(levelData.width, levelData.height, levelData.cellSize, levelData.origin, (_grid, x, y) =>
            {
                var data = levelData.GetTile(x, y);
                if (data != null && data.id != 0)
                {
                    Vector2 worldPos = _grid.GetWorldPosition(x, y) + Vector2.one * (levelData.cellSize / 2f);
                    GameObject newObject = Instantiate(data.prefab, worldPos, Quaternion.identity);
                    spawnedObjects[new Vector2Int(x, y)] = newObject;
                }

                return data;
            }
        );

        Resource.OnDeath += gridPosition =>
        {
            grid.SetValue(gridPosition.x, gridPosition.y, emptyData);
            spawnedObjects.Remove(gridPosition);
        };
    }

    public void SetEmpty(int x, int y)
    {
        grid.SetValue(x, y, emptyData);
    }
}