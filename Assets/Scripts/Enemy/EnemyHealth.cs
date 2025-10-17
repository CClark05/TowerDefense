using System;
using CodeMonkey.Utils;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IUsesHealth
{
    public HealthSystem HealthSystem { get; private set; }
    public Transform Transform => transform;
    public event Action OnDeathAnimationDone;
    public event Action OnDeath;
    public static event Action<bool> OnDeathStatic; // bool indicates if final enemy of wave
    public event Action OnHit;
    private void Start()
    {
        HealthSystem = new HealthSystem(GetComponent<EnemyDataHolder>().Data.health);
    }

    public bool TakeDamage(int amount)
    {
        bool isDead = HealthSystem.Damage(amount);
        OnHit?.Invoke();
        if (isDead)
        {
            bool finalEnemy = EnemyManager.Instance.CurrentEnemies.Count == 1 && EnemyManager.Instance.WaveState is EnemyManager.WaveStates.DoneSpawning;
            OnDeathStatic?.Invoke(finalEnemy);
            OnDeath?.Invoke();
            GetComponent<IMovementOverride>().SetSpeed(0);
            FunctionTimer.Create(() =>
            {
                OnDeathAnimationDone?.Invoke();
                Destroy(gameObject);
            }, finalEnemy ? KillEffects.finalFreezeFrameDuration * 3f : KillEffects.freezeFrameDuration, true);
        }
        return isDead;
    }

    public bool IsDeadFromDamage(int damage)
    {
        return HealthSystem.Health - damage <= 0;
    }
}