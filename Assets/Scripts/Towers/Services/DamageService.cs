using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class DamageService
{
    public static void ApplyDamage(
        HitData hitData,
        IDamageable damageable,
        IUsesStatusEffects statusEffects,
        SkillContext skillContext,
        Action<HitData, Vector2> onDealDamage)
    {
        var onHitEffects = statusEffects.PersistentEffectsApplied.Where(kvp => kvp.Key is OnHitStatusEffect).ToList();
        if (onHitEffects.Count > 0)
        {
            foreach (var kvp in onHitEffects)
            {
                ((OnHitStatusEffect)kvp.Key).OnPersistentHit(hitData, kvp.Value);
            }
        }
        
        CallModifier.Call<IHitModifier>(skillContext, (mod, _) => { mod.Modify(hitData, damageable); });
        foreach (var status in hitData.effectsApplied.OfType<IHitModifier>())
        {
            status.Modify(hitData, damageable);
        }

        foreach (var kvp in hitData.effectsApplied)
        {
            if (kvp.Key is not PersistentStatusEffect persistentEffect) continue;
            var onEffects = skillContext.GetSkillInstancesWith<IOnEffectApplied>().Where(e => e.modifier.Effect == kvp.Key);
            var effectData = new ModifyEffectData();
            foreach (var mod in onEffects)
            {
                if (mod.instance.IsDisabled) continue;
                for (int i = 0; i < (mod.modifier.PlayOnce ? 1 : mod.instance.PlayCount); i++)
                {
                    mod.modifier.Modify(effectData, hitData);
                }
            }

            statusEffects.AddPersistentEffect(persistentEffect, hitData, kvp.Value, effectData);
        }

        // hitData.finalDamage = Mathf.RoundToInt(hitData.finalDamage);
        hitData.finalDamage = CalculateDamage.MultIncrease(damageable.GetDamageBonus(), hitData.finalDamage, 1);
        CallModifier.Call<IAfterHitModifier>(skillContext, (mod, _) => { mod.Modify(hitData); });
        bool isDead = damageable.IsDeadFromDamage(hitData.finalDamage);
        hitData.didKill = isDead;
        onDealDamage?.Invoke(hitData, damageable.Transform.position);
        if (isDead)
        {
            CallModifier.Call<IOnKill>(skillContext, (mod, _) => { mod.OnKill(hitData); });
        }
        
        damageable.TakeDamage(hitData.finalDamage);
    }
}