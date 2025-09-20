using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, IPathPredictor, IMovementOverride
{
    private List<Vector2> path;
    private int currentIndex;
    private float baseSpeed;
    private float originalMoveSpeed;
    private float speedMult = 1;
    private float effectiveSpeed => originalMoveSpeed * speedMult;
    public static event Action<int> OnReachedEndStatic;
    public event Action OnReachedEnd;
    public float Progress => (float)currentIndex / path.Count;
    private Rigidbody2D rb;
    private Vector2 velocity;
    public event Action<float> OnJump;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        baseSpeed = GetComponent<EnemyDataHolder>().Data.speed;
        originalMoveSpeed = baseSpeed;
        path = AStarPathfinding.Instance.GetPath();
        if(path == null) Debug.LogError("No path found");
    }
    private void Update()
    {
        if (currentIndex >= path.Count)
        {
            OnReachedEndStatic?.Invoke(GetComponent<EnemyDataHolder>().Data.livesCost);
            OnReachedEnd?.Invoke();
            Destroy(gameObject);
            return;
        }
        
        Vector2 target = path[currentIndex];
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        velocity = direction * effectiveSpeed;
        if (Vector2.Distance(transform.position, target) < 0.1f)
            currentIndex++;
    }
    
    private void FixedUpdate()
    {
        rb.linearVelocity = velocity;
    }
    
    public bool TryPosVelAt(float t, out Vector2 pos, out Vector2 vel)
    {
        if (path == null || path.Count == 0) { pos = transform.position; vel = Vector2.zero; return false; }

        float remaining = effectiveSpeed * Mathf.Max(0f, t);
        Vector2 cur = transform.position;
        int idx = currentIndex;

        while (true)
        {
            if (idx >= path.Count) { pos = cur; vel = Vector2.zero; return true; }
            Vector2 tgt = path[idx];
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