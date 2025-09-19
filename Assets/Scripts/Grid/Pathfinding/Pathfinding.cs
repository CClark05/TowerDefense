using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding<T>
{
    private class PathNode
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public float gCost;
        public float hCost;
        public float fCost => gCost + hCost;
        public PathNode parent;
        public PathNode(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return x + "," + y;
        }
    }
    
    private int width;
    private int height;
    private HashSet<T> walkableTiles;
    private Grid<T> grid;
    public Pathfinding(Grid<T> grid, T[] walkableTiles)
    {
        this.grid = grid;
        width = grid.Width;
        height = grid.Height;
        this.walkableTiles = new HashSet<T>(walkableTiles);
    }

    public List<Vector2> FindPath(Vector2Int start, Vector2Int end)
    {
        PathNode[,] nodes = new PathNode[width, height];
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
            nodes[x, y] = new PathNode(x, y);

        PathNode startNode = nodes[start.x, start.y];
        PathNode endNode = nodes[end.x, end.y];

        List<PathNode> openList = new() { startNode };
        HashSet<PathNode> closedSet = new();

        startNode.gCost = 0;
        startNode.hCost = Heuristic(startNode, endNode);

        while (openList.Count > 0)
        {
            PathNode current = openList.OrderBy(n => n.fCost).First();

            if (current == endNode)
                return ReconstructPath(current, grid);

            openList.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current, nodes, grid))
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeG = current.gCost + 1;

                if (!openList.Contains(neighbor) || tentativeG < neighbor.gCost)
                {
                    neighbor.gCost = tentativeG;
                    neighbor.hCost = Heuristic(neighbor, endNode);
                    neighbor.parent = current;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return null; 
    }
    
    private List<PathNode> GetNeighbors(PathNode node, PathNode[,] nodes, Grid<T> grid)
    {
        List<PathNode> neighbors = new();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in directions)
        {
            int nx = node.x + dir.x;
            int ny = node.y + dir.y;

            if (nx >= 0 && ny >= 0 && nx < grid.Width && ny < grid.Height)
            {
                T tile = grid.GetValue(nx, ny);
                if (tile != null && walkableTiles.Contains(tile)) 
                    neighbors.Add(nodes[nx, ny]);
            }
        }

        return neighbors;
    }
    
    private float Heuristic(PathNode a, PathNode b) =>
        Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    
    private static List<Vector2> ReconstructPath(PathNode endNode, Grid<T> grid)
    {
        List<Vector2> path = new();
        PathNode current = endNode;

        while (current != null)
        {
            path.Add(grid.GetWorldPosition(current.x, current.y) + Vector2.one * (grid.CellSize / 2f));
            current = current.parent;
        }

        path.Reverse();
        return path;
    }
}

