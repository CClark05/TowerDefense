using System;

public class HealthSystem
{
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public event Action OnDead;
    public event Action<int> OnHealthChanged;
    public HealthSystem(int health)
    {
        this.Health = health;
        MaxHealth = health;
    }

    public bool Damage(int damage)
    {
        Health -= damage;
        OnHealthChanged?.Invoke(Health);
        if (Health <= 0)
        {
            OnDead?.Invoke();
            return true;
        }

        return false;
    }

    public void Heal(int amount)
    {
        Health += amount;
        if (Health > MaxHealth) Health = MaxHealth;
        OnHealthChanged?.Invoke(Health);
    }
}