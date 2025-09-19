using System.Collections;
using UnityEngine;

public class TickData
{
    public IDamageable damageable;
    public IUsesStatusEffects statusEffects;
    public int stacks;
    public float tickInterval;
    public float timer;
    public HitData hitData;
    public TickData(IDamageable damageable, IUsesStatusEffects statusEffects, TowerShooting tower, HitData hitData, float tickInterval, int stacks)
    {
        this.damageable = damageable;
        this.statusEffects = statusEffects;
        this.tickInterval = tickInterval;
        this.hitData = hitData;
        this.stacks = stacks;
    }
    
}
