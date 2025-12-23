using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnTickStatusEffect : PersistentStatusEffect
{
    public abstract void OnTick(TickData tickData);
    public float tickInterval = 1;
    protected void SetDamageData(TickData tickData, SkillContext skillContext, int damage, List<Color> newColors)
    {
        var data = new DamageData();
        data.finalDamage += damage;
        foreach (var c in newColors)
            data.colors.Add(c);
        if(newColors.Count == 0) 
            data.colors.Add(color);
        data.damageMarkerPunchEffect = damageMarkerPunchEffect;
        data.damageMarkerSizeMult *= damageMarkerSizeMult;
        data.didKill = tickData.damageable.TakeDamage(data.finalDamage);
        if (data.didKill)
        {
            CallModifier.Call<IOnKill>(skillContext, (mod,_) =>
            {
                mod.OnKill(tickData.hitData);
            });
        }
        tickData.statusEffects.OnTakeDamage?.Invoke(data, tickData.hitData.tower);
    }
    
}