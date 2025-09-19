using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform follow;
    private Transform player;

    [Header("Scroll Margin (Dead Zone in Tiles)")]
    [SerializeField] private float preWaveMarginX = 4f;
    [SerializeField] private float waveMarginX = 8f;
    [SerializeField] private float preWaveMarginY = 3f;
    [SerializeField] private float waveMarginY = 6f;

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.15f;

    private float cellSize;
    private Vector2Int worldSize;
    private Vector2 cameraCenter;
    private Vector3 velocity;
    private Camera camera;

    private float marginX;
    private float marginY;
    private bool isZoomedOut;
    private float baseOrthoSize;
    private float zoomedOutSize;

    private void Start()
    {
        player = follow;
        cellSize = GridManager.Instance.CellSize;
        worldSize = new Vector2Int(GridManager.Instance.Grid.Width, GridManager.Instance.Grid.Height);
        camera = Camera.main;

        baseOrthoSize = (GridManager.Instance.CellSize * 8f) / 2f; // base from viewHeight = 8
        zoomedOutSize = baseOrthoSize * 1.406f;
        camera.orthographicSize = baseOrthoSize;

        SnapTo(follow.position);

        EnemyManager.Instance.OnWaveStarted += () =>
        {
            ZoomTo(zoomedOutSize);
            isZoomedOut = true;
            StartCoroutine(FollowClosestEnemy());
        };

        EnemyManager.Instance.OnWaveComplete += _ =>
        {
            ZoomTo(baseOrthoSize);
            isZoomedOut = false;
            StopCoroutine(FollowClosestEnemy());
            follow = player;
            SnapTo(follow.position);
        };

        BuildingUI.Instance.OnEnterBuildMode += () => { ZoomTo(zoomedOutSize); isZoomedOut = true; };
        BuildingUI.Instance.OnExitBuildMode += () => { ZoomTo(baseOrthoSize); isZoomedOut = false; };
    }

    private void LateUpdate()
    {
        if (follow == null) return;

        GridManager.Instance.Grid.GetXY(follow.position, out int x, out int y);
        Vector2Int gridPos = new Vector2Int(x, y);

        float visibleTilesY = camera.orthographicSize * 2f / cellSize;
        float visibleTilesX = visibleTilesY * camera.aspect;

        marginX = isZoomedOut ? waveMarginX : preWaveMarginX;
        marginY = isZoomedOut ? waveMarginY : preWaveMarginY;

        float camMinX = cameraCenter.x - visibleTilesX / 2f + marginX;
        float camMaxX = cameraCenter.x + visibleTilesX / 2f - marginX;
        float camMinY = cameraCenter.y - visibleTilesY / 2f + marginY;
        float camMaxY = cameraCenter.y + visibleTilesY / 2f - marginY;

        bool outsideX = gridPos.x < camMinX || gridPos.x > camMaxX;
        bool outsideY = gridPos.y < camMinY || gridPos.y > camMaxY;

        if (outsideX || outsideY)
        {
            int camX = Mathf.Clamp(gridPos.x, Mathf.FloorToInt(visibleTilesX / 2f), worldSize.x - Mathf.FloorToInt(visibleTilesX / 2f));
            int camY = Mathf.Clamp(gridPos.y, Mathf.FloorToInt(visibleTilesY / 2f), worldSize.y - Mathf.FloorToInt(visibleTilesY / 2f));
            cameraCenter = new Vector2(camX, camY);
        }

        Vector3 targetPos = GridManager.Instance.Grid.GetWorldPosition((int)cameraCenter.x, (int)cameraCenter.y);
        targetPos.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        
    }

    private void SnapTo(Vector3 worldPos)
    {
        GridManager.Instance.Grid.GetXY(worldPos, out int x, out int y);

        int halfWidth = Mathf.FloorToInt(camera.orthographicSize * camera.aspect / cellSize);
        int halfHeight = Mathf.FloorToInt(camera.orthographicSize / cellSize);

        int camX = Mathf.Clamp(x, halfWidth, worldSize.x - halfWidth);
        int camY = Mathf.Clamp(y, halfHeight, worldSize.y - halfHeight);

        cameraCenter = new Vector2(camX, camY);

        Vector3 centerWorld = GridManager.Instance.Grid.GetWorldPosition(camX, camY);
        centerWorld.z = transform.position.z;
        transform.position = centerWorld;
    }

    private void ZoomTo(float targetSize)
    {
        LeanTween.value(gameObject, f => camera.orthographicSize = f, camera.orthographicSize, targetSize, 0.5f).setEaseOutCubic();
    }

    private List<Vector2> path = new();
    int GetProgressIndex(Vector2 enemyPos)
    {
        float minDist = float.MaxValue;
        int index = 0;
        for (int i = 0; i < path.Count; i++)
        {
            float dist = Vector2.Distance(enemyPos, path[i]);
            if (dist < minDist)
            {
                minDist = dist;
                index = i;
            }
        }
        return index;
    }
    private IEnumerator FollowClosestEnemy()
    {
        path = AStarPathfinding.Instance.GetPath();
        while (EnemyManager.Instance.CurrentEnemies.Count == 0)
            yield return null;

        int lastFollowProgress = -1;

        while (EnemyManager.Instance.CurrentEnemies.Count > 0)
        {
            List<Vector2> path = AStarPathfinding.Instance.GetPath();
            if (path == null || path.Count == 0)
            {
                yield return null;
                continue;
            }

            GameObject furthestEnemy = null;
            int maxProgress = -1;

            foreach (var enemy in EnemyManager.Instance.CurrentEnemies)
            {
                if (enemy == null) continue;

                int progress = GetProgressIndex(enemy.transform.position);
                if (progress > maxProgress)
                {
                    maxProgress = progress;
                    furthestEnemy = enemy;
                }
            }

            // Require new enemy to be significantly ahead
            const int minLead = 2; // tweak this: how many path steps ahead needed to switch
            int currentFollowProgress = (follow != null) ? GetProgressIndex(follow.position) : -1;

            if (furthestEnemy != null && (follow == null || maxProgress - currentFollowProgress >= minLead))
            {
                follow = furthestEnemy.transform;
                lastFollowProgress = maxProgress;
            }

            yield return null;
        }

        follow = player;
    }
}
