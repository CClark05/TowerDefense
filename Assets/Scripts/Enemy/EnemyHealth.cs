using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable, IUsesHealth
{
    public HealthSystem HealthSystem { get; private set; }
    public Transform Transform => transform;
    public event Action OnDeath;
    private void Start()
    {
        HealthSystem = new HealthSystem(GetComponent<EnemyDataHolder>().Data.health);
    }

    public bool TakeDamage(int amount)
    {
        bool isDead = HealthSystem.Damage(amount);
        if (isDead)
        {
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
        return isDead;
    }


    
}