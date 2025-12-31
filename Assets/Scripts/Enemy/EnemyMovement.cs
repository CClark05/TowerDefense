using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, IPathPredictor, IMovementOverride, IJumpable, IMovementListener
{
    private List<Vector2> centerPath;
    private List<Vector2> projectedPath;
    private int currentIndex;
    
    private float baseSpeed;
    private float speedMult = 1;
    private float effectiveSpeed => baseSpeed * speedMult;
    public event Action OnReachedEnd;
    public float Progress => (float)currentIndex / (projectedPath.Count - 1);
    private Rigidbody2D rb;
    private Vector2 velocity;
    public event Action<float> OnJump;
    private bool stopMovement;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        baseSpeed = GetComponent<EnemyDataHolder>().Data.speed;
        CalculatePath();
    }
    private void CalculatePath()
    {
        /**
        
        if(centerPath == null) Debug.LogError("No path found");
        var segment = AStarPathfinding.GetSegment(centerPath, transform.position);
        projectedPath = AStarPathfinding.BuildOffsetPath(centerPath, segment.signedOffset);
        currentIndex =  Mathf.Clamp(segment.seg + (segment.t > 0.5f ? 1 : 0), 0, projectedPath.Count - 1);
        */
        centerPath = AStarPathfinding.Instance.GetPath();
        if(centerPath == null) Debug.LogError("No path found");
        var seg = AStarPathfinding.GetSegment(centerPath, transform.position);
        projectedPath = AStarPathfinding.BuildOffsetPath(centerPath, seg.signedOffset);

        Vector2 pos = transform.position;
        int start = Mathf.Clamp(currentIndex, 0, projectedPath.Count - 1);

        int best = start;
        float bestDist2 = float.PositiveInfinity;

        for (int i = start; i < projectedPath.Count; i++)
        {
            float d2 = ((Vector2)projectedPath[i] - pos).sqrMagnitude;
            if (d2 < bestDist2)
            {
                bestDist2 = d2;
                best = i;
            }
        }
        currentIndex = Mathf.Clamp(best + 1, 0, projectedPath.Count - 1);
    }
    private void Update()
    {
        if (currentIndex >= projectedPath.Count) 
        {
            OnReachedEnd?.Invoke();
            Destroy(gameObject);
            return;
        }

        if (stopMovement) return;
        Vector2 target = projectedPath[currentIndex];
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        velocity = direction * effectiveSpeed;
        if (Vector2.Distance(transform.position, target) < 0.01f)
            currentIndex++;
    }

    
    private void FixedUpdate()
    {
        rb.linearVelocity = velocity;
    }


    public void MoveTo(Vector2 position, float duration)
    {
        if (stopMovement) return;
        stopMovement = true;
        velocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        Tween moveTween = rb.DOMove(position, duration)
            .SetEase(Ease.OutQuad)        
            .SetUpdate(UpdateType.Fixed)
            .SetTarget(gameObject)
            .OnComplete(() =>
            {
                CalculatePath();
                stopMovement = false;
            });
    }
    public bool TryPosVelAt(float t, out Vector2 pos, out Vector2 vel)
    {
        if (centerPath == null || centerPath.Count == 0) { pos = transform.position; vel = Vector2.zero; return false; }

        float remaining = effectiveSpeed * Mathf.Max(0f, t);
        Vector2 cur = transform.position;
        int idx = currentIndex;

        while (true)
        {
            if (idx >= centerPath.Count) { pos = cur; vel = Vector2.zero; return true; }
            Vector2 tgt = centerPath[idx];
            Vector2 seg = tgt - cur;
            float segLen = seg.magnitude;

            if (segLen < 1e-5f) { idx++; continue; }

            if (remaining <= segLen)
            {
                Vector2 dir = seg / segLen;
                pos = cur + dir * remaining;
                vel = dir * effectiveSpeed;
                return true;
            }

            remaining -= segLen;
            cur = tgt;
            idx++;
        }
    }

    private Coroutine speedRoutine;
    public void SetSpeed(float targetMult, float duration = 0f)
    {
        if (speedRoutine != null) 
            StopCoroutine(speedRoutine); speedRoutine = null;
        if (duration <= 0f)
        {
            speedMult = targetMult;  
            return;
        }
        speedRoutine = StartCoroutine(SmoothMultiplier(targetMult, duration));
    }
    public void ResetSpeed(float duration = 0f) => SetSpeed(1f, duration);
    public void AddSpeed(float percentIncrease)
    {
        speedMult += percentIncrease;
    }

    

    public void Jump(float height, float duration)
    {
        StartCoroutine(JumpCoroutine(height, duration));
    }
    private IEnumerator JumpCoroutine(float height, float duration)
    {
        Vector2 originalPos = transform.position;
        float t = 0;
        OnJump?.Invoke(duration);
        while (t < duration)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / duration);
            float h = 4f * height * u * (1f - u);
            transform.position = originalPos + Vector2.up * h;
            yield return null; 
        }
        transform.position = originalPos; 
    }
    private IEnumerator SmoothMultiplier(float target, float duration)
    {
        float start = speedMult;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            speedMult = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        speedMult = target;
        speedRoutine = null;
    }
    
}