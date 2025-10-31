using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class GridObjectPlacement : MonoBehaviour
{
    private Vector2[] preferredPositions = new Vector2[2];
    private Vector2Int[] preferredGridPositions = new Vector2Int[2];
    private GridManager gridManager;
    private GridObjectButton gridObjectButton;
    private void Awake()
    {
        gridManager = GridManager.Instance;
        gridObjectButton = GetComponent<GridObjectButton>();
    }

    private void OnEnable()
    {
        var desiredPosition = FindDesiredPosition();
        transform.position = desiredPosition;
    }

    private Vector2 FindDesiredPosition()
    {
        preferredPositions = gridObjectButton.Sprites.Select(s => (Vector2)s.transform.position).ToArray();
        for (var i = 0; i < preferredPositions.Length; i++)
        {
            gridManager.Grid.GetXY(preferredPositions[i], out int x, out int y);
            preferredGridPositions[i] = new Vector2Int(x, y);
        }
        foreach(var gridPos in preferredGridPositions)
        {
            var id = gridManager.Grid.GetValue(gridPos.x, gridPos.y).id;
            if (id == 0) continue;
            List<Vector2Int> surroundingGridPositions = new();
            for(int i = preferredGridPositions[0].x - 1; i <= preferredGridPositions[1].x + 1; i++)
            {
                for(int j = preferredGridPositions[0].y - 1; j <= preferredGridPositions[0].y + 1; j++)
                {
                    surroundingGridPositions.Add(new Vector2Int(i, j));
                }
            }
            HashSet<(Vector2Int left, Vector2Int right)> possiblePositions = new();
            foreach(var pos in surroundingGridPositions)
            {
                Vector2Int left = new Vector2Int(pos.x - 1, pos.y);
                Vector2Int right = new Vector2Int(pos.x + 1, pos.y);
                if(surroundingGridPositions.Contains(left))
                    possiblePositions.Add((left, pos));
                if(surroundingGridPositions.Contains(right))
                    possiblePositions.Add((pos, right));
            }

            foreach (var pair in possiblePositions)
            {
                var leftId = gridManager.Grid.GetValue(pair.left.x, pair.left.y).id;
                var rightId = gridManager.Grid.GetValue(pair.right.x, pair.right.y).id;
                if (rightId == 0 && leftId == 0)
                {
                    (Vector2 left, Vector2 right) desiredWorldPositions = (gridManager.Grid.GetWorldPosition(pair.left.x, pair.left.y),
                        gridManager.Grid.GetWorldPosition(pair.right.x, pair.right.y));
                    return ((desiredWorldPositions.left + desiredWorldPositions.right) / 2) + new
                        Vector2(gridManager.Grid.CellSize * 0.5f, gridManager.Grid.CellSize * 0.5f);
                }
                Debug.LogError("NO SPACE FOR SHOP NOT GOOD");
            }
            break;
        }
        return (preferredPositions[0] + preferredPositions[1]) / 2;
    }
}
