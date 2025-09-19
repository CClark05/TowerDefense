using System;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileData data;
    [SerializeField] private float maxLeadSeconds = 0.3f;
    [SerializeField] private float steerGain = 12;
    [SerializeField] private float homingWindow = 0.1f;
    private IDamageable target;
    private TowerDataHolder towerData;

    private Vector2 direction;
    private Transform targetTransform;
    public event Action<HitData, Vector2> OnDealDamage;
    private int maxEnemiesPierced;
    private float speedIncrease = 1;
    private HashSet<IDamageable> enemiesHit = new();
    private void Init(IDamageable target, ProjectileShotData shotData)
    {
        this.target = target;
        GetComponent<ProjectileVisual>().SetColor(shotData.projectileColor);
        maxEnemiesPierced = shotData.maxEnemiesPierced;
        targetTransform = target.Transform;
        speedIncrease *= shotData.speedIncrease;
        CalculateAim(transform.position, data.speed * speedIncrease);
    }

    public static Projectile CreateProjectile(ProjectileData data, ProjectileShotData shotData, Vector2 position, IDamageable target, TowerDataHolder tower)
    {
        var projectile = Instantiate(data.prefab, position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Init(target, shotData);
        projectile.towerData = tower;
        return projectile;
    }
    
    private float homingTimer;
    private void Update()
    {
        homingTimer += Time.deltaTime;
        if (homingTimer < homingWindow)
        {
            
        }
        transform.position += (Vector3)direction * (data.speed * speedIncrease * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<IDamageable>() == null) return;
        var damageable = other.GetComponent<IDamageable>();
        var statusEffects = other.GetComponent<IUsesStatusEffects>();
        enemiesHit.Add(damageable);
        if (other.GetComponent<IUsesShields>() != null)
        {
            if (other.GetComponent<IUsesShields>().TryRemoveShield(1))
            {
                if (enemiesHit.Count >= maxEnemiesPierced)
                {
                    Destroy(gameObject);
                    return;
                }
            }
        }
        int baseDamage = data.damage + towerData.Data.damage;
        var hitData = new HitData(baseDamage, towerData.GetComponent<TowerShooting>(), damageable, statusEffects);
        bool delayed = false;
        hitData.DelayDamage = (float delay, float multiplier) =>
        {
            delayed = true;
            CoroutineRunner.Instance.StartCoroutine(
                DamageService.ApplyDelayedDamage(hitData, damageable, statusEffects, towerData.SkillContext, delay, multiplier, OnDealDamage));
        };
        var list = towerData.SkillContext.GetSkillInstancesWith<IOnHit>();
        foreach (var mod in towerData.SkillContext.GetSkillInstancesWith<IOnHit>().OrderBy(p => p.modifier.Priority)) 
        {
            mod.modifier.OnHit(hitData);
            if (mod.instance.PlayTwice)
                mod.modifier.OnHit(hitData);
        }
        if (delayed)
        {
            Destroy(gameObject);
            return;
        }
        DamageService.ApplyDamage(hitData, damageable, statusEffects, towerData.SkillContext, OnDealDamage);
        if (enemiesHit.Count >= maxEnemiesPierced)
        {
            Destroy(gameObject);
        }
        
    }
    
    private bool TryPathIntercept(IPathPredictor predictor, Vector2 shooterPos, float projSpeed, float tMax, out Vector2 aim)
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
                float tHit = 0.5f * (a + b);
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
        predictor.TryPosVelAt(bestT, out aim, out _);
        return false;
    }
    private void CalculateAim(Vector2 shooterPos, float projSpeed)
    {
        var predictor = targetTransform.GetComponent<IPathPredictor>();
        TryPathIntercept(predictor, shooterPos, projSpeed, maxLeadSeconds, out var aim);
        Vector2 desired = (aim - shooterPos).normalized;
        float a = 1f - Mathf.Exp(-steerGain * Time.deltaTime);
        direction = Vector2.Lerp(direction, desired, a).normalized;
    }
}