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
    [SerializeField] private Transform shootOrigin;
    private ProjectileData projectileData;
    private TowerDataHolder towerDataHolder;
    private float shootTimer;
    public event Action<int> OnDealDamage;
    public static event Action<Vector2, DamageData> OnDealDamageStatic;
    public event Action OnKillEnemy;
    public event Action OnShoot;
    public float TotalUptime { get; private set; }
    private Coroutine shootCoroutine;
    private float timeBetweenShots => towerDataHolder.RuntimeData.TimeBetweenShots;
    private float range => towerDataHolder.RuntimeData.Range == 0 ? Mathf.Infinity : towerDataHolder.RuntimeData.Range;
    [SerializeField] private TargetingModes targetingMode = TargetingModes.First;
    public TargetingModes TargetingMode => targetingMode;
    public int ShotsThisRound { get; private set; }
    public int HitsThisRound { get; private set; }
    private EnemyManager enemyManager;
    public Vector2? Direction { get; private set; }
    private GameObject target;
    public GameObject Target => target;
    public event Action<GameObject> OnTargetChanged;
    private bool disableShooting;
    private TowerAnimation towerAnimation;
    public event Action<bool> OnToggleShooting;

    private ITowerAimOverride aimOverride;
    private void Awake()
    {
        towerDataHolder = GetComponent<TowerDataHolder>();
        projectileData = towerDataHolder.ProjectileData;
        towerAnimation = GetComponent<TowerAnimation>();
        aimOverride = GetComponent<ITowerAimOverride>();
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
        if (towerDataHolder.RuntimeData.stunned || disableShooting) return;
        shootTimer += Time.deltaTime;

        var closestEnemy = TargetEnemy();
        SetTarget(closestEnemy);
        Direction = target != null ? (target.transform.position - transform.position) : null;
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
            if (aimOverride != null && !aimOverride.CanShoot(closestEnemy)) return;
            shootCoroutine = StartCoroutine(ShootProjectile());
            shootTimer = 0f;
        }
    }

    private IEnumerator ShootProjectile()
    {
        if (projectileData == null) yield break;
        SetTarget(TargetEnemy());
        var shotTarget = target;
        if (shotTarget == null)
            yield break;
        Vector2 origin = shootOrigin.position;
        var shotData = new ProjectileShotData(Time.time)
        {
            originalDirection = aimOverride?.GetShotDirection(origin, shotTarget) ?? ((Vector2)shotTarget.transform.position - origin).normalized
        };
        CallModifier.Call<IShotModifier>(towerDataHolder.SkillContext, (mod, instance) => mod.Modify(shotData));
        yield return ProjectileService.ModifyProjectile(shotData, towerDataHolder.SkillContext);
        if (shotData.shootMainProjectile) shotData.projectiles++;

        shotData.directionOverrides.ForEach((_) => shotData.projectiles++);
        if (shotTarget != null && shotTarget.TryGetComponent<IDamageable>(out var damageable))
        {
            OnShoot?.Invoke();
            Action OnShootComplete = null;
            OnShootComplete = () =>
            {
                towerAnimation.OnShootAnimationComplete -= OnShootComplete;
                if (shotData.shootMainProjectile)
                {
                    var projectile = Projectile.CreateProjectile(projectileData, shotData, shootOrigin.position, damageable, towerDataHolder, shotData.originalDirection);
                    Hook(projectile);
                }

                foreach (var dir in shotData.directionOverrides)
                {
                    var p = Projectile.CreateProjectile(projectileData, shotData, shootOrigin.position, dir, towerDataHolder);
                    Hook(p);
                }
                ShotsThisRound++;

                void Hook(Projectile p)
                {
                    shotData.RegisterProjectile(p);
                    p.OnDealDamage += (HitData hitData, Vector2 pos) =>
                    {
                        HitsThisRound++;
                        OnDealDamage?.Invoke(hitData.finalDamage);
                        OnDealDamageStatic?.Invoke(pos, hitData);
                        if (hitData.didKill) OnKillEnemy?.Invoke();
                    };

                }
                shootCoroutine = null;
            };
            towerAnimation.OnShootAnimationComplete += OnShootComplete;
        }
        else
            shootCoroutine = null;

    }

    public void ToggleShooting(bool disable)
    {
        disableShooting = disable;
        OnToggleShooting?.Invoke(disableShooting);
    }
    public void DealtDamage(HitData hitData, Vector2 pos)
    {
        OnDealDamage?.Invoke(hitData.finalDamage);
        OnDealDamageStatic?.Invoke(pos, hitData);
        if (hitData.didKill) OnKillEnemy?.Invoke();
    }

    public void AddHit(int amount = 1) => HitsThisRound += amount;
    private void SetTarget(GameObject newTarget)
    {
        if (ReferenceEquals(newTarget, target)) return;
        target = newTarget;
        OnTargetChanged?.Invoke(target);
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
        towerAnimation.OnShootAnimationComplete = null;
        enemyManager.OnWaveStarted -= OnWaveStarted;
    }
}