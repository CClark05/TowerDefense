using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinding : Singleton<AStarPathfinding>
{
    [SerializeField] private GridObjectData[] walkableTileData;
    private Pathfinding<GridObjectData> pathfinding;
    private Grid<GridObjectData> grid;
    private List<Vector2> path = new();
    private void Start()
    {
        grid = GridManager.Instance.Grid;
        pathfinding = new Pathfinding<GridObjectData>(grid, walkableTileData);
        var startAndEnd = FindStartAndEnd();
        if(startAndEnd.start == null || startAndEnd.end == null) 
            Debug.LogError("No path found"); 
        path = pathfinding.FindPath(startAndEnd.start.Value, startAndEnd.end.Value);
        
    }

    private (Vector2Int? start, Vector2Int? end) FindStartAndEnd()
    {
        Vector2Int? start = null;
        Vector2Int? end = null;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var tileData = grid.GetValue(x, y);
                if(!walkableTileData.Contains(tileData)) continue;
                if (end == null || x < end.Value.x)
                {
                    end = new Vector2Int(x, y);
                }

                if (start == null || x > start.Value.x)
                {
                    start = new Vector2Int(x, y);
                }
            }
        }

        return (start, end);
    }

    public List<Vector2> GetPath() => path;
}
