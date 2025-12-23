using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileData data;
    [SerializeField] private float maxLeadSeconds = 0.3f;
    [SerializeField] private float steerGain = 12;
    private IDamageable target;
    private TowerDataHolder towerData;

    private Vector2? direction;
    public Vector2 Origin { get; private set; }
    private Transform targetTransform;
    public event Action<HitData, Vector2> OnDealDamage;
    private int maxEnemiesPierced;
    private float speedIncrease = 1;
    private HashSet<IDamageable> enemiesHit = new();
    private bool homing;
    private ProjectileShotData shotData;
    public float EffectiveSpeed => data.speed * speedIncrease;
    
    private void Init(IDamageable target, ProjectileShotData shotData)
    {
        this.target = target;
        this.shotData = shotData;
        if(shotData.projectileSprite != null) 
            GetComponent<SpriteRenderer>().sprite = shotData.projectileSprite;
        shotData.projectile = this;
        homing = shotData.homing;
        GetComponent<ProjectileVisual>().SetColor(shotData.projectileColor);
        maxEnemiesPierced = shotData.maxEnemiesPierced;
        targetTransform = target.Transform;
        speedIncrease *= shotData.speedIncrease;
        CalculateAim(transform.position, EffectiveSpeed);

    }

    public static Projectile CreateProjectile(ProjectileData data, ProjectileShotData shotData, Vector2 position, IDamageable target, TowerDataHolder tower)
    {
        var projectile = Instantiate(data.prefab, position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Origin = position;
        projectile.towerData = tower;
        projectile.Init(target, shotData);
        return projectile;
    }

    private bool shotDestroyed;
    private void Update()
    {
        if (homing && targetTransform != null)
            direction = (targetTransform.position - transform.position).normalized;
        
        if(direction != null)
            transform.position += (Vector3)direction * (EffectiveSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() == null) return;
        ApplyDamage(other.GetComponent<IDamageable>());
    }

    private void ApplyDamage(IDamageable damageable)
    {
        var statusEffects = damageable.Transform.GetComponent<IUsesStatusEffects>();
        enemiesHit.Add(damageable);
        if (damageable.Transform.GetComponent<IUsesShields>() != null)
        {
            if (damageable.Transform.GetComponent<IUsesShields>().TryRemoveShield(1))
            {
                if (enemiesHit.Count >= maxEnemiesPierced)
                {
                    shotData.ShotDestroyed();
                    direction = null;
                    return;
                }
            }
        }
        int baseDamage = data.damage + towerData.RuntimeData.BaseDamage;
        var hitData = new HitData(baseDamage, towerData.GetComponent<TowerShooting>(), damageable, statusEffects);

        CallModifier.Call<IOnHit>(towerData.SkillContext, (mod, _) =>
        {
            mod.OnHit(hitData);
        });

        DamageService.ApplyDamage(hitData, damageable, statusEffects, towerData.SkillContext, OnDealDamage);
        if (enemiesHit.Count >= maxEnemiesPierced)
        {
            shotData.ShotDestroyed();
            direction = null;
        }
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
                    if (Mathf.Sign(gm) == Mathf.Sign(gPrev)) { a = m; gPrev = gm; }
                    else { b = m; }
                }
                tHit = 0.5f * (a + b);
                predictor.TryPosVelAt(tHit, out aim, out _);
                return true;
            }
            tPrev = t; gPrev = g;
        }
        float bestT = 0f, bestAbs = Mathf.Abs(gPrev);
        for (int i = 1; i <= samples; i++)
        {
            float t = tMax * i / samples;
            float a = Mathf.Abs(G(t));
            if (a < bestAbs) { bestAbs = a; bestT = t; }
        }

        tHit = bestT;
        predictor.TryPosVelAt(bestT, out aim, out _);
        return false;
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
}