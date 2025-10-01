using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ClusterSpawning
{
    private struct ClusterSpawnSettings
    {
        public int laneCount; // e.g., 3
        public float laneSpacing; // e.g., 0.5f world units
        public float forwardJitter; // e.g., 0.75f
        public float radius; // enemy collision radius for overlap check
        public int maxAttempts; // e.g., 8
        public LayerMask enemyMask; // layer of enemies for overlap checks
    }

    public static bool TryGetClusteredSpawn(Vector2 position, int slotIndex, out Vector2 result)
    {
        var settings = new ClusterSpawnSettings
        {
            laneCount = 3,
            laneSpacing = 1.2f,
            forwardJitter = 0.75f,
            radius = 0.3f,
            maxAttempts = 8,
            enemyMask = LayerMask.GetMask("Enemy")
        };
        return TryGetClusteredSpawn(position, slotIndex, settings, out result);
    }

    private static bool TryGetClusteredSpawn(Vector2 position, int slotIndex, ClusterSpawnSettings s, out Vector2 result)
    {
        var path = AStarPathfinding.Instance.GetPath();
        if (path == null || path.Count < 2)
        {
            result = Vector2.zero;
            return false;
        }

        var closestIndex = AStarPathfinding.Instance.GetPathIndex(position);
        Vector2 p0 = path[closestIndex];
        Vector2 p1 = (closestIndex + 1 < path.Count) ? path[closestIndex + 1] : path[closestIndex];
        Vector2 dir = (p1 - p0).normalized;
        Vector2 n = new Vector2(-dir.y, dir.x); // left-normal

        // Distribute enemies across lanes deterministically by slotIndex
        int lane = (s.laneCount <= 0) ? 0 : (slotIndex % s.laneCount);
        float laneCenter = (s.laneCount - 1) * 0.5f;
        float laneOffset = (lane - laneCenter) * s.laneSpacing;

        // Try a few samples with slight forward jitter and tiny extra lateral jitter
        for (int attempt = 0; attempt < s.maxAttempts; attempt++)
        {
            float forward = UnityEngine.Random.Range(0f, s.forwardJitter);
            float lateralJitter = UnityEngine.Random.Range(-0.15f, 0.15f);

            Vector2 candidate = p0 + dir * forward + n * (laneOffset + lateralJitter);

            // Avoid spawning inside another enemy
            if (!Physics2D.OverlapCircle(candidate, s.radius, s.enemyMask))
            {
                result = candidate;
                return true;
            }
        }

        // Fallback: just use p0; movement/separation will resolve quickly
        result = p0;
        return true;
    }
    /**
    public static Vector2[] CreateRandomCluster(Vector2 position, int cellSize, int count, float padding = 0.75f, float enemyRadius = 0.5f, int attempts = 200, string layerMask = "Enemy")
    {
        List<Vector2> cluster = new List<Vector2>(count);
        var pathPoint = AStarPathfinding.Instance.GetClosestPathPoint(position);
        var pad = padding + enemyRadius;
        float maxOffset = Mathf.Max(0, cellSize / 2f - pad);
        float separationMult = 1.5f;
        for (int i = 0; i < attempts; i++)
        {
            Vector2 randomPos = pathPoint + new Vector2(UnityEngine.Random.Range(-maxOffset, maxOffset), UnityEngine.Random.Range(-maxOffset, maxOffset));
            if (Physics2D.OverlapCircle(randomPos, enemyRadius, LayerMask.GetMask(layerMask)))
                continue;
            if (cluster.Any(c => (c - randomPos).sqrMagnitude < Math.Pow(enemyRadius * 2 * separationMult, 2)))
                continue;
            if (cluster.Count >= count)
                return cluster.ToArray();
            cluster.Add(randomPos);
        }

        Debug.LogError("Not able to create full cluster");
        while (cluster.Count < count)
            cluster.Add(pathPoint);
        return cluster.ToArray();
    }
    */
    public static Vector2[] CreateRandomCluster(
        Vector2 position,
        int cellSize,
        int count,
        float padding = 0.6f,
        float enemyRadius = 0.4f,
        int kCandidates = 24,
        int maxFailures = 200,
        string layerMask = "Enemy")
    {
        var results = new List<Vector2>(count);
        var pathPoint = AStarPathfinding.Instance.GetClosestPathPoint(position);

        // Square bounds centered at pathPoint
        float half = cellSize * 0.5f;
        float pad = padding + enemyRadius; // keep center at least this far from the wall
        float halfExtent = Mathf.Max(0f, half - pad); // usable half-size

        if (halfExtent <= 0f)
            return Enumerable.Repeat(pathPoint, count).ToArray();

        int failures = 0;
        int layer = LayerMask.GetMask(layerMask);

        // Helper: distance to nearest cell edge for an offset from center
        float EdgeClear(Vector2 offset)
        {
            // Chebyshev distance to boundary of a square (how much margin remains)
            float maxAbs = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y));
            return halfExtent - maxAbs;
        }

        // Helper: minimum center-to-center distance to prior points (minus 2R for clearance)
        float MinClearToOthers(Vector2 p)
        {
            float min = float.PositiveInfinity;
            for (int i = 0; i < results.Count; i++)
                min = Mathf.Min(min, Vector2.Distance(p, results[i]) - (enemyRadius * 2f));
            return results.Count == 0 ? float.PositiveInfinity : min;
        }

        // Main selection loop
        while (results.Count < count && failures < maxFailures)
        {
            Vector2 best = pathPoint;
            float bestScore = float.NegativeInfinity;
            bool foundAny = false;

            for (int k = 0; k < kCandidates; k++)
            {
                // Random candidate inside the padded square
                var offset = new Vector2(
                    UnityEngine.Random.Range(-halfExtent, halfExtent),
                    UnityEngine.Random.Range(-halfExtent, halfExtent)
                );
                Vector2 candidate = pathPoint + offset;

                // Physics overlap check
                if (Physics2D.OverlapCircle(candidate, enemyRadius, layer))
                    continue;

                // Compute clearances
                float edgeClear = EdgeClear(offset);
                if (edgeClear < enemyRadius) // too close to wall
                    continue;

                float neighborClear = MinClearToOthers(candidate);
                if (neighborClear < 0f) // would overlap another enemy's radius
                    continue;

                // Score = how safe this spot is considering both edges and neighbors
                float score = Mathf.Min(edgeClear - enemyRadius, neighborClear); // maximize the bottleneck

                if (score > bestScore)
                {
                    bestScore = score;
                    best = candidate;
                    foundAny = true;
                }
            }

            if (foundAny)
            {
                results.Add(best);
            }
            else
            {
                // Couldn’t find a valid point this round; relax gradually
                failures++;
                // Light relaxation: shrink required neighbor spacing a bit
                //enemyRadius = Mathf.Max(0.05f, enemyRadius * 0.98f);
            }
        }

        // If still short, pad with center (won’t happen often)
        while (results.Count < count)
        {
            Debug.LogError("Unable to create full cluster");
            results.Add(pathPoint);
        }
        return results.ToArray();
    }
}