using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weak Status Effect", menuName = "StatusEffects/Weak")]
public class WeakStatusEffect : OnHitStatusEffect
{
    public float damageIncrease = 0.1f;

    private void OnValidate()
    {
        description = $"Next hit against this target deals +{damageIncrease * 100}% damage per stack, consumes all stacks.";
    }

    public override void Execute(HitData hitData)
    {
        if (hitData.statusEffects.PersistentEffectsApplied.ContainsKey(this)) return;
        hitData.effectsApplied[this] = hitData.effectsApplied.GetValueOrDefault(this) + 1;
    }

    public override void OnPersistentHit(HitData hitData, int stacks)
    {
        hitData.damageMarkerPunchEffect = damageMarkerPunchEffect;
        hitData.colors.Add(color);
        hitData.damageMarkerSizeMult *= damageMarkerSizeMult;
        hitData.finalDamage = CalculateDamage.MultIncrease(damageIncrease * stacks, hitData.finalDamage, 1);
        RemoveAllStacks(hitData);
    }
    
}