using System;
using System.Linq;
using CodeMonkey.Utils;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IUsesHealth
{
    public HealthSystem HealthSystem { get; private set; }
    public Transform Transform => this != null ? transform : null;
    public event Action OnDeath;
    public static event Action<bool> OnDeathStatic; // bool indicates if final enemy of wave
    public event Action OnHit;
    public event Action<int> OnTakeDamage;
    private FunctionTimer deathTimer;
    private float damageBonus;
    private EnemyDataHolder dataHolder;
    private void Start()
    {
        HealthSystem = new HealthSystem(GetComponent<EnemyDataHolder>().MaxHP);
        dataHolder = GetComponent<EnemyDataHolder>();
    }

    public bool TakeDamage(int amount)
    {
        if (deathTimer != null) return false;
        if(HealthSystem.Health <= 0) return false;
        bool isDead = HealthSystem.Damage(amount);
        OnHit?.Invoke();
        OnTakeDamage?.Invoke(amount);
        if (isDead)
        {
            if (GetComponent<EnemyDataHolder>().Data.coins > 1)
                PlayerInventory.Instance.CoinAnimation(GetComponent<EnemyDataHolder>().Data.coins, transform.position);
            
            GetComponent<IMovementOverride>().SetSpeed(0);
            if (dataHolder.EnemiesUnderneath != null && dataHolder.EnemiesUnderneath.Length > 0)
            {
                EnemyManager.Instance.SpawnEnemyAtPosition(dataHolder.EnemiesUnderneath[0].EnemyData, transform.position, dataHolder.EnemiesUnderneath.Skip(1).ToArray());
            }
            bool finalEnemy = EnemyManager.Instance.CurrentEnemies.Count == 1 && dataHolder.EnemiesUnderneath.Length == 0 && EnemyManager.Instance.WaveState is EnemyManager.WaveStates.DoneSpawning;
            OnDeathStatic?.Invoke(finalEnemy);
            OnDeath?.Invoke();
            deathTimer = FunctionTimer.Create(() =>
            {
                Destroy(gameObject);
            }, KillEffects.freezeFrameDuration, true);
        }
        return isDead;
    }

    public bool IsDeadFromDamage(int damage)
    {
        return HealthSystem.Health - damage <= 0;
    }

    public void ApplyDamageBonus(float percentIncrease)
    {
        damageBonus += percentIncrease;
    }

    public float GetDamageBonus() => damageBonus;

    public void Heal(int amount) => HealthSystem.Heal(amount);

    private void OnDestroy()
    {
        FunctionTimer.RemoveTimer(deathTimer);
    }
}