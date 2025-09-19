using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire Status Effect", menuName = "StatusEffects/Fire")]
public class FireStatusEffect : OnTickStatusEffect
{
    public int damagePerTick = 1;

    private void OnValidate()
    {
        description = $"Deals {damagePerTick} damage per second for each stack.";
    }

    public override void Execute(HitData hitData)
    {
        hitData.effectsApplied[this] = hitData.effectsApplied.GetValueOrDefault(this) + 1;
    }
    
    public override void OnTick(TickData tickData, int stacks)
    {
        SetDamageData(tickData, damagePerTick * stacks);
    }
}