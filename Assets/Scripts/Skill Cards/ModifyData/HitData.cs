using System;
using System.Collections.Generic;

public class HitData : DamageData
{
    public int baseDamage;
    public TowerShooting tower;
    public IDamageable damageable;
    public IUsesStatusEffects statusEffects;
    public Dictionary<StatusEffectData, int> effectsApplied = new();
    public Action<float, float> DelayDamage; //delay, mult
    public Action<float> RetriggerDamage; //mult
    public HitData(int baseDamage, TowerShooting tower, IDamageable damageable, IUsesStatusEffects statusEffects)
    {
        this.baseDamage = baseDamage;
        this.finalDamage = baseDamage;
        this.damageable = damageable;
        this.statusEffects = statusEffects;
        this.tower = tower;
        damageMarkerSizeMult = 1;
        damageMarkerPunchEffect = false;
    }
    //Clone
    public HitData(HitData other)
    {
        baseDamage = other.baseDamage;
        finalDamage = other.finalDamage;
        tower = other.tower;
        colors = other.colors;
        damageMarkerSizeMult = other.damageMarkerSizeMult;
        damageMarkerPunchEffect = other.damageMarkerPunchEffect;
        damageable = other.damageable;
        statusEffects = other.statusEffects;
        didKill = false;
    }
    
}