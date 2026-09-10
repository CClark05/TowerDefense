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
        if (startAndEnd.start == null || startAndEnd.end == null)
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
                if (!walkableTileData.Contains(tileData)) continue;
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

    public int GetPathIndex(Vector2 position)
    {
        Vector2 closest = GetClosestPathPoint(position);
        return path.IndexOf(closest);
    }
    public Vector2 GetPathPointAhead(Vector2 position, int stepsForward)
    {
        if (path.Count == 0) return Vector2.zero;
        int currentIndex = GetPathIndex(position);
        int targetIndex = Mathf.Clamp(currentIndex + stepsForward, 0, path.Count - 1);
        return path[targetIndex];
    }

    public Vector2 GetClosestPathPoint(Vector2 position)
    {
        if (path.Count == 0) return Vector2.zero;
        Vector2 closest = path[0];
        float closestDistance = Vector2.Distance(position, closest);
        foreach (var point in path)
        {
            float distance = Vector2.Distance(position, point);
            if (distance < closestDistance)
            {
                closest = point;
                closestDistance = distance;
            }
        }

        return closest;
    }

    public static (int seg, float t, float signedOffset) GetSegment(IReadOnlyList<Vector2> path, Vector2 position)
    {
        int bestSeg = 0;
        float bestDist2 = float.PositiveInfinity;
        float bestT = 0;
        float bestSigned = 0;

        for (int i = 0; i < path.Count - 1; i++)
        {
            var a = path[i];
            var b = path[i + 1];
            var ab = b - a;
            var len2 = ab.sqrMagnitude;
            if (len2 < 1e-8f) continue;

            float t = Mathf.Clamp01(Vector2.Dot(position - a, ab) / len2);
            Vector2 c = a + t * ab;
            float d2 = (position - c).sqrMagnitude;
            if (d2 < bestDist2)
            {
                bestDist2 = d2;
                bestSeg = i;
                bestT = t;
                var n = new Vector2(-ab.y, ab.x).normalized;
                bestSigned = Vector2.Dot(position - c, n);
            }
        }

        return (bestSeg, bestT, bestSigned);
    }

    public static List<Vector2> BuildOffsetPath(IReadOnlyList<Vector2> path, float offset, float miterLimit = 8f)
    {
        int n = path.Count;
        List<Vector2> outPts = new(n);

        if (n == 1) return new List<Vector2> { path[0] };
        if (n == 2)
        {
            var (A, B) = (path[0], path[1]);
            var nrm = LeftNormal(B - A);
            outPts.Add(A + offset * nrm);
            outPts.Add(B + offset * nrm);
            return outPts;
        }

        // first cap
        {
            var n0 = LeftNormal(path[1] - path[0]);
            outPts.Add(path[0] + offset * n0);
        }

        // interior corners
        for (int i = 1; i < n - 1; i++)
        {
            var A = path[i - 1];
            var B = path[i];
            var C = path[i + 1];
            var d0 = (B - A).normalized;
            var n0 = LeftNormal(d0);
            var d1 = (C - B).normalized;
            var n1 = LeftNormal(d1);

            // parallel offset lines through B
            var p0 = B + offset * n0; // point on line 0
            var p1 = B + offset * n1; // point on line 1

            if (TryLineIntersection(p0, d0, p1, d1, out var I))
            {
                // miter length check to avoid spikes
                float miter = (I - B).magnitude / Mathf.Max(1e-4f, Mathf.Abs(offset));
                if (miter <= miterLimit)
                    outPts.Add(I); // miter join
                else
                {
                    outPts.Add(p0); // bevel join
                    outPts.Add(p1);
                }
            }
            else
            {
                // parallel (180°) — just average
                outPts.Add((p0 + p1) * 0.5f);
            }
        }

        // last cap
        {
            var nLast = LeftNormal(path[n - 1] - path[n - 2]);
            outPts.Add(path[n - 1] + offset * nLast);
        }

        return outPts;

        static Vector2 LeftNormal(Vector2 v) => new Vector2(-v.y, v.x).normalized;

        static bool TryLineIntersection(Vector2 p, Vector2 dir, Vector2 q, Vector2 e, out Vector2 I)
        {
            // Solve p + t*dir = q + u*e
            float cross = dir.x * e.y - dir.y * e.x;
            if (Mathf.Abs(cross) < 1e-8f)
            {
                I = default;
                return false;
            }

            Vector2 pq = q - p;
            float t = (pq.x * e.y - pq.y * e.x) / cross;
            I = p + t * dir;
            return true;
        }
    }
}