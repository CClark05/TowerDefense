using System;
using CodeMonkey.Utils;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IUsesHealth
{
    public HealthSystem HealthSystem { get; private set; }
    public Transform Transform => transform;
    public event Action OnDeath;
    public static event Action OnFinalEnemyDeath;
    public event Action OnHit;
    private void Start()
    {
        HealthSystem = new HealthSystem(GetComponent<EnemyDataHolder>().Data.health);
    }

    public bool TakeDamage(int amount)
    {
        OnHit?.Invoke();
        bool isDead = HealthSystem.Damage(amount);
        if (isDead)
        {
            if (EnemyManager.Instance.CurrentEnemies.Count == 1)
            {
                OnFinalEnemyDeath?.Invoke();
            }
            FunctionTimer.Create(() =>
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }, EnemyManager.Instance.CurrentEnemies.Count == 1 ? SlowmotionEffect.freezeFrameDration : 0f, true);
        }
        return isDead;
    }

    public bool IsDeadFromDamage(int damage)
    {
        return HealthSystem.Health - damage <= 0;
    }
}