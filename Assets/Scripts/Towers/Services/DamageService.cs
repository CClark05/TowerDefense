using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

public static class DamageService
{
    public static void ApplyDamage(
        HitData hitData,
        IDamageable damageable,
        IUsesStatusEffects statusEffects,
        SkillContext skillContext,
        Action<HitData, Vector2> onDealDamage,
        float finalMult = 1)
    {
        var onHitEffects = statusEffects.PersistentEffectsApplied.Where(kvp => kvp.Key is OnHitStatusEffect).ToList();
        if (onHitEffects.Count > 0)
        {
            foreach (var kvp in onHitEffects)
            {
                ((OnHitStatusEffect)kvp.Key).OnPersistentHit(hitData, kvp.Value);
            }
        }

        foreach (var mod in skillContext.GetSkillInstancesWith<IHitModifier>())
        {
            for(int i = 0; i < mod.instance.PlayCount; i++)
            {
                mod.modifier.Modify(hitData, damageable);
            }
        }

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
                for(int i = 0; i < mod.instance.PlayCount; i++)
                {
                    mod.modifier.Modify(effectData, hitData);
                }
            }

            statusEffects.AddPersistentEffect(persistentEffect, hitData, kvp.Value, effectData);
        }

        hitData.finalDamage = Mathf.RoundToInt(hitData.finalDamage * finalMult);
        foreach (var mod in skillContext.GetSkillInstancesWith<IAfterHitModifier>())
        {
            for(int i = 0; i < mod.instance.PlayCount; i++)
            {
                mod.modifier.Modify(hitData);
            }
        }
        bool isDead = damageable.TakeDamage(hitData.finalDamage);
        hitData.didKill = isDead;
        onDealDamage?.Invoke(hitData, damageable.Transform.position);
        if (isDead)
        {
            foreach (var onKill in skillContext.GetSkillInstancesWith<IOnKill>())
            {
                for(int i = 0; i < onKill.instance.PlayCount; i++)
                {
                    onKill.modifier.OnKill(hitData);
                }
            }
        }
        hitData.RetriggerDamage = mult =>
        {
            CoroutineRunner.Instance.StartCoroutine(Retrigger());
            IEnumerator Retrigger()
            {
                float delay = 0.4f;
                yield return new WaitForSeconds(delay);
                var clone = new HitData(hitData);
                DamageService.ApplyDamage(clone, damageable, statusEffects, skillContext, onDealDamage, mult);
            }
        };
    }
    /**
    public static int CalculateDamage(
        HitData hitData,
        IDamageable damageable,
        IUsesStatusEffects statusEffects,
        SkillContext skillContext, out bool didKill, float finalMult = 1)
    {
        hitData.ghost = true;
        var onHitEffects = statusEffects.PersistentEffectsApplied.Where(kvp => kvp.Key is OnHitStatusEffect).ToList();
        if (onHitEffects.Count > 0)
        {
            foreach (var kvp in onHitEffects)
            {
                ((OnHitStatusEffect)kvp.Key).OnPersistentHit(hitData, kvp.Value);
            }
        }

        foreach (var mod in skillContext.GetSkillInstancesWith<IHitModifier>())
        {
            for(int i = 0; i < mod.instance.PlayCount; i++)
            {
                mod.modifier.Modify(hitData, damageable);
            }
        }

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
                for(int i = 0; i < mod.instance.PlayCount; i++)
                {
                    mod.modifier.Modify(effectData, hitData);
                }
            }

            statusEffects.AddPersistentEffect(persistentEffect, hitData, kvp.Value, effectData, true);
        }

        hitData.finalDamage = Mathf.RoundToInt(hitData.finalDamage * finalMult);
        bool isDead = damageable.IsDeadFromDamage(hitData.finalDamage);
        hitData.didKill = isDead;
        if (isDead)
        {
            foreach (var onKill in skillContext.GetSkillInstancesWith<IOnKill>())
            {
                for(int i = 0; i < onKill.instance.PlayCount; i++)
                {
                    onKill.modifier.OnKill(hitData);
                }
            }
        }
        didKill = isDead;
        return hitData.finalDamage;
    }
    */
    public static IEnumerator ApplyDelayedDamage(
        HitData hitData,
        IDamageable damageable,
        IUsesStatusEffects statusEffects,
        SkillContext skillContext,
        float delay,
        float multiplier,
        Action<HitData, Vector2> onDealDamage)
    {
        yield return new WaitForSeconds(delay);
        if (damageable == null) yield break;
        ApplyDamage(hitData, damageable, statusEffects, skillContext, onDealDamage, multiplier);
    }
    
}