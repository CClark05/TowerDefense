using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public Transform Transform { get; }
    public bool TakeDamage(int amount);
}