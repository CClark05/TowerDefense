using System;
using System.Collections;
using UnityEngine;

public enum TargetingModes
{
    First,
    Close,
    Strong
}

public class TowerShooting : MonoBehaviour
{
    private ProjectileData projectileData;
    private TowerDataHolder towerDataHolder;
    private float shootTimer;

    public event Action<int> OnDealDamage;
    public static event Action<Vector2, DamageData> OnDealDamageStatic;
    public event Action OnKillEnemy;

    private Coroutine shootCoroutine;
    private float timeBetweenShots => towerDataHolder.RuntimeData.timeBetweenShots;
    private float range => towerDataHolder.RuntimeData.Range;
    [SerializeField] private TargetingModes targetingMode = TargetingModes.First;
    public TargetingModes TargetingMode => targetingMode;
    private void Awake()
    {
        towerDataHolder = GetComponent<TowerDataHolder>();
        projectileData = towerDataHolder.ProjectileData;
    }

    private void OnEnable()
    {
        EnemyStatusEffects.OnTakeDamageStatic += EnemyStatusEffectsOnTakeDamage;
        GetComponentInChildren<TargetingModeUI>().OnTargetingModeUpdated += mode =>
        {
            targetingMode = mode;
        };
    }

    private void EnemyStatusEffectsOnTakeDamage(DamageData data, TowerShooting tower, Vector2 position)
    {
        if (tower != this) return;
        OnDealDamage?.Invoke(data.finalDamage);
        OnDealDamageStatic?.Invoke(position, data);
        if (data.didKill) OnKillEnemy?.Invoke();
    }

    private void Update()
    {
        if (towerDataHolder.RuntimeData.stunned) return;
        shootTimer += Time.deltaTime;

        if (shootCoroutine != null && shootTimer >= timeBetweenShots)
        {
            shootTimer = timeBetweenShots;
            return;
        }

        if (shootCoroutine == null && shootTimer >= timeBetweenShots)
        {
            var closestEnemy = TargetEnemy();
            if (closestEnemy != null)
            {
                shootCoroutine = StartCoroutine(ShootProjectile());
                shootTimer = 0f;
            }
        }
    }

    private IEnumerator ShootProjectile()
    {
        var shotData = new ProjectileShotData(Time.time);
        yield return ProjectileService.ModifyProjectile(shotData, towerDataHolder.SkillContext);
        var target = TargetEnemy();
        if (target != null && target.TryGetComponent<IDamageable>(out var damageable))
        {
            var projectile = Projectile.CreateProjectile(projectileData, shotData, transform.position, damageable, towerDataHolder);

            projectile.OnDealDamage += (HitData hitData, Vector2 pos) =>
            {
                OnDealDamage?.Invoke(hitData.finalDamage);
                OnDealDamageStatic?.Invoke(pos, hitData);
                if (hitData.didKill) OnKillEnemy?.Invoke();
            };
        }

        shootCoroutine = null;
    }

    private GameObject TargetEnemy()
    {
        var enemies = EnemyManager.Instance.CurrentEnemies;
        (GameObject enemy, float bestValue) best = (null, targetingMode == TargetingModes.Close ? Mathf.Infinity : float.NegativeInfinity);

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > range) continue;
            switch (targetingMode)
            {
                case TargetingModes.First:
                    var enemyMovement = enemy.GetComponent<EnemyMovement>();
                    if (enemyMovement.Progress > best.bestValue)
                    {
                        best.enemy = enemy;
                        best.bestValue = enemyMovement.Progress;
                    }

                    break;
                case TargetingModes.Close:
                    if (distance < best.bestValue)
                    {
                        best.enemy = enemy;
                        best.bestValue = distance;
                    }

                    break;
                case TargetingModes.Strong:
                    var enemyHealth = enemy.GetComponent<IUsesHealth>();
                    if (enemyHealth.HealthSystem.MaxHealth > best.bestValue)
                    {
                        best.enemy = enemy;
                        best.bestValue = enemyHealth.HealthSystem.MaxHealth;
                    }

                    break;
            }
        }

        return best.enemy;
    }

    private void OnDisable()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }

        EnemyStatusEffects.OnTakeDamageStatic -= EnemyStatusEffectsOnTakeDamage;
    }
    
}