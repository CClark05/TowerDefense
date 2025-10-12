using System;
using CodeMonkey.Utils;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IUsesHealth
{
    public HealthSystem HealthSystem { get; private set; }
    public Transform Transform => transform;
    public event Action OnDeath;
    public static event Action<bool> OnDeathStatic; // bool indicates if final enemy of wave
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
            bool finalEnemy = EnemyManager.Instance.CurrentEnemies.Count == 1 && EnemyManager.Instance.WaveState is EnemyManager.WaveStates.DoneSpawning;
            OnDeathStatic?.Invoke(finalEnemy);
            FunctionTimer.Create(() =>
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }, finalEnemy ? KillEffects.finalFreezeFrameDration : KillEffects.freezeFrameDuration, true);
        }
        return isDead;
    }

    public bool IsDeadFromDamage(int damage)
    {
        return HealthSystem.Health - damage <= 0;
    }
}