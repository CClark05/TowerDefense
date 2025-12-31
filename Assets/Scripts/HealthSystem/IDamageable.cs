using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public Transform Transform { get; }
    public bool TakeDamage(int amount);
    public bool IsDeadFromDamage(int damage);
    public void ApplyDamageBonus(float percentIncrease);
    public float GetDamageBonus();
}