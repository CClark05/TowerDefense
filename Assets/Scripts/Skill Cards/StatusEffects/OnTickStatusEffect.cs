using System;
using UnityEngine;

public abstract class OnTickStatusEffect : PersistentStatusEffect
{
    public abstract void OnTick(TickData tickData, int stacks);
    public float tickInterval = 1;
    protected void SetDamageData(TickData tickData, int damage)
    {
        var data = new DamageData();
        data.finalDamage += damage;
        data.colors.Add(color);
        data.damageMarkerPunchEffect = damageMarkerPunchEffect;
        data.damageMarkerSizeMult = damageMarkerSizeMult;
        data.didKill = tickData.damageable.TakeDamage(data.finalDamage);
        if (data.didKill)
        {
            foreach (var onKill in tickData.hitData.tower.GetComponent<TowerDataHolder>().SkillContext.GetSkillInstancesWith<IOnKill>())
            {
                onKill.modifier.OnKill(tickData.hitData);
                if(onKill.instance.PlayTwice)
                    onKill.modifier.OnKill(tickData.hitData);
            }
        }
        tickData.statusEffects.OnTakeDamage?.Invoke(data, tickData.hitData.tower);
    }
}