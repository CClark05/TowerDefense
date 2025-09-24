using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ClusterSpawning
{
    private struct ClusterSpawnSettings
    {
        public int laneCount;        // e.g., 3
        public float laneSpacing;    // e.g., 0.5f world units
        public float forwardJitter;  // e.g., 0.75f
        public float radius;         // enemy collision radius for overlap check
        public int maxAttempts;      // e.g., 8
        public LayerMask enemyMask;  // layer of enemies for overlap checks
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
        return TryGetClusteredSpawn(position,slotIndex, settings, out result);
    }
    private static bool TryGetClusteredSpawn(Vector2 position, int slotIndex, ClusterSpawnSettings s, out Vector2 result)
    {
        var path = AStarPathfinding.Instance.GetPath();
        if (path == null || path.Count < 2) { result = Vector2.zero; return false; }
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

    public static Vector2[] CreateRandomCluster(Vector2 position, int cellSize, int count, float padding = 0.75f, float enemyRadius = 0.25f, int attempts = 150, string layerMask = "Enemy")
    {
        List<Vector2> cluster = new List<Vector2>(count);
        var pathPoint = AStarPathfinding.Instance.GetClosestPathPoint(position);
        var pad = padding + enemyRadius;
        float maxOffset = Mathf.Max(0, cellSize / 2f - pad);
        for (int i = 0; i < attempts; i++)
        {
            Vector2 randomPos = pathPoint + new Vector2(UnityEngine.Random.Range(-maxOffset, maxOffset), UnityEngine.Random.Range(-maxOffset, maxOffset));
            if (Physics2D.OverlapCircle(randomPos, enemyRadius, LayerMask.GetMask(layerMask)))
                continue;
            if(cluster.Any(c => (c - randomPos).sqrMagnitude < Math.Pow(enemyRadius * 2, 2)))
                continue;
            if (cluster.Count >= count)
                return cluster.ToArray();
            cluster.Add(randomPos);
        }
        Debug.LogError("Not able to create full cluster");
        return cluster.ToArray();
    }
}
