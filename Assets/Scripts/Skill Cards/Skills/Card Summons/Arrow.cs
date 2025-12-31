using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public interface IMinionProjectile
{
    public void Init(IDamageable target);
    public event Action<IDamageable> OnHit;
}
public class Arrow : MonoBehaviour, IMinionProjectile
{
    [SerializeField] private float maxLeadSeconds = 0.3f;
    [SerializeField] private float steerGain = 12;
    [SerializeField] private ProjectileData data;
    public event Action<IDamageable> OnHit;
    private Vector2? direction;
    private IDamageable target;
    private Transform targetTransform => target.Transform;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Border>())
        {
            Destroy(gameObject);
            return;
        }
        if (other.GetComponent<IDamageable>() == null) return;
        OnHit?.Invoke(other.GetComponent<IDamageable>());
        Destroy(gameObject);
    }

    public void Init(IDamageable target)
    {
        this.target = target;
        CalculateAim(transform.position,data.speed);
        float angle = Mathf.Atan2(direction.Value.y, direction.Value.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }
    private void Update()
    {
        if (direction != null)
        {
            transform.position += (Vector3)direction.Value * (data.speed * Time.deltaTime);
        }
        
    }

    private void CalculateAim(Vector2 shooterPos, float projSpeed)
    {
        var predictor = targetTransform.GetComponent<IPathPredictor>();
        TryPathIntercept(predictor, shooterPos, projSpeed, maxLeadSeconds, out var aim, out var tHit);
        Vector2 desired = (aim - shooterPos).normalized;
        float a = 1f - Mathf.Exp(-steerGain * Time.deltaTime);
        direction ??= Vector2.zero;
        direction = Vector2.Lerp(direction.Value, desired, a).normalized;
    }
    
    private bool TryPathIntercept(IPathPredictor predictor, Vector2 shooterPos, float projSpeed, float tMax, out Vector2 aim, out float tHit)
    {
        float G(float t)
        {
            predictor.TryPosVelAt(t, out var p, out _);
            return (p - shooterPos).magnitude - projSpeed * t;
        }

        const int samples = 12;
        float tPrev = 0f, gPrev = G(0f);
        for (int i = 1; i <= samples; i++)
        {
            float t = tMax * i / samples;
            float g = G(t);
            if (Mathf.Sign(g) != Mathf.Sign(gPrev))
            {
                float a = tPrev, b = t;
                for (int it = 0; it < 18; it++)
                {
                    float m = 0.5f * (a + b);
                    float gm = G(m);
                    if (Mathf.Sign(gm) == Mathf.Sign(gPrev))
                    {
                        a = m;
                        gPrev = gm;
                    }
                    else
                    {
                        b = m;
                    }
                }

                tHit = 0.5f * (a + b);
                predictor.TryPosVelAt(tHit, out aim, out _);
                return true;
            }

            tPrev = t;
            gPrev = g;
        }

        float bestT = 0f, bestAbs = Mathf.Abs(gPrev);
        for (int i = 1; i <= samples; i++)
        {
            float t = tMax * i / samples;
            float a = Mathf.Abs(G(t));
            if (a < bestAbs)
            {
                bestAbs = a;
                bestT = t;
            }
        }

        tHit = bestT;
        predictor.TryPosVelAt(bestT, out aim, out _);
        return false;
    }
}
