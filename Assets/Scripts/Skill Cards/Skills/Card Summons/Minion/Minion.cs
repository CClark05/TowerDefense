using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public interface IMinion
{
    public void Init(ProjectileData projectileData, float range, float timeBetweenShots);
    public void Destroy();
    public Transform Transform { get; }
    public event Action<IDamageable, int> OnHit;
}
public class Minion : MonoBehaviour, IHoverable, IMinion
{
    private ProjectileData projectileData;
    [SerializeField] private GameObject rangeVisual;
    private float range = 5;
    private float timeBetweenShots = 1;
    private float shootTimer;
    public event Action<IDamageable, int> OnHit;
    public event Action OnShoot;
    public event Action OnDestroy;
    public Transform Transform => transform;
    public event Action OnEnemyEnteredRange;
    public event Action OnNoTarget;
    public GameObject Target { get; private set; }
    public void Init(ProjectileData projectileData, float range, float timeBetweenShots)
    {
        this.projectileData = projectileData;
        this.range = range;
        this.timeBetweenShots = timeBetweenShots;
        rangeVisual.transform.localScale = new Vector3(range * 2f, range * 2f, 1);
    }

    public void Destroy() => OnDestroy?.Invoke();

    private void Update()
    {
        if (EnemyManager.Instance.WaveState is EnemyManager.WaveStates.Idle or EnemyManager.WaveStates.Complete) return;
        shootTimer += Time.deltaTime;
        var _target = TargetEnemy(TargetingModes.First);
        if(_target == null && Target != null) OnNoTarget?.Invoke();
        Target = _target;
        if (shootTimer >= timeBetweenShots)
        {
            if (Target == null)
                return;
            
            OnShoot?.Invoke();
            GameObject projectile = Instantiate(projectileData.prefab, transform.position, Quaternion.identity);
            projectile.GetComponent<IMinionProjectile>().Init(Target.GetComponent<IDamageable>());
            projectile.GetComponent<IMinionProjectile>().OnHit += (d) => OnHit?.Invoke(d, projectileData.damage);
            shootTimer = 0;
        }
    }
    private GameObject TargetEnemy(TargetingModes targetingMode)
    {
        var enemies = EnemyManager.Instance.CurrentEnemies;
        (GameObject enemy, float bestValue) best = (null, targetingMode == TargetingModes.Close ? Mathf.Infinity : float.NegativeInfinity);

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > range) continue;
            if(Target == null) OnEnemyEnteredRange?.Invoke();
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
    public void OnHover()
    {
        rangeVisual.SetActive(true);
    }

    public void OnLeaveHover()
    {
        rangeVisual.SetActive(false);
    }

    public void OnClick()
    {
        
    }
}