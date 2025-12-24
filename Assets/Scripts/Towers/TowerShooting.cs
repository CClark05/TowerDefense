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
    public float TotalUptime { get; private set; }
    private Coroutine shootCoroutine;
    private float timeBetweenShots => towerDataHolder.RuntimeData.TimeBetweenShots;
    private float range => towerDataHolder.RuntimeData.Range;
    [SerializeField] private TargetingModes targetingMode = TargetingModes.First;
    public TargetingModes TargetingMode => targetingMode;
    public int ShotsThisRound { get; private set; }
    public int HitsThisRound { get; private set; }
    private EnemyManager enemyManager;
    public Vector2? Direction { get; private set; }
    private GameObject target;

    private void Awake()
    {
        towerDataHolder = GetComponent<TowerDataHolder>();
        projectileData = towerDataHolder.ProjectileData;
    }

    private void OnEnable()
    {
        EnemyStatusEffects.OnTakeDamageStatic += EnemyStatusEffectsOnTakeDamage;
        GetComponentInChildren<TargetingModeUI>().OnTargetingModeUpdated += mode => { targetingMode = mode; };
        enemyManager = EnemyManager.Instance;
        enemyManager.OnWaveStarted += OnWaveStarted;
    }

    private void OnWaveStarted()
    {
        ShotsThisRound = 0;
        HitsThisRound = 0;
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

        var closestEnemy = TargetEnemy();
        if (closestEnemy != null)
            TotalUptime += Time.deltaTime;
        if (shootCoroutine != null && shootTimer >= timeBetweenShots)
        {
            shootTimer = timeBetweenShots;
            return;
        }

        if (shootCoroutine == null && shootTimer >= timeBetweenShots)
        {
            if (closestEnemy == null) return;
            shootCoroutine = StartCoroutine(ShootProjectile());
            shootTimer = 0f;
        }

        Direction = target != null ? (target.transform.position - transform.position) : null;
    }

    private IEnumerator ShootProjectile()
    {
        target = TargetEnemy();
        if (target == null)
            yield break;
        var shotData = new ProjectileShotData(Time.time)
        {
            originalDirection = (target.transform.position - transform.position).normalized
        };
        yield return ProjectileService.ModifyProjectile(shotData, towerDataHolder.SkillContext);
        if (target != null && target.TryGetComponent<IDamageable>(out var damageable))
        {
            var projectile = Projectile.CreateProjectile(projectileData, shotData, transform.position, damageable, towerDataHolder);
            Hook(projectile);
            foreach (var dir in shotData.directionOverrides)
            {
                projectile = Projectile.CreateProjectile(projectileData, shotData, transform.position, dir, towerDataHolder);
                Hook(projectile);
            }
            ShotsThisRound++;

            
            void Hook(Projectile p)
            {
                p.OnDealDamage += (HitData hitData, Vector2 pos) =>
                {
                    HitsThisRound++;
                    OnDealDamage?.Invoke(hitData.finalDamage);
                    OnDealDamageStatic?.Invoke(pos, hitData);
                    if (hitData.didKill) OnKillEnemy?.Invoke();
                };
            }
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
                    var movementListener = enemy.GetComponent<IMovementListener>();
                    if (movementListener.Progress > best.bestValue)
                    {
                        best.enemy = enemy;
                        best.bestValue = movementListener.Progress;
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
        enemyManager.OnWaveStarted -= OnWaveStarted;
    }
}