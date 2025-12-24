using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire Status Effect", menuName = "StatusEffects/Fire")]
public class FireStatusEffect : OnTickStatusEffect
{
    public int damagePerTick = 1;

    private void OnValidate()
    {
        description = $"Deals {damagePerTick} damage per second per stack.";
    }

    public override void Execute(HitData hitData)
    {
        hitData.effectsApplied[this] = hitData.effectsApplied.GetValueOrDefault(this) + 1;
    }
    
    public override void OnTick(TickData tickData)
    {
        foreach (var kvp in tickData.stacks)
        {
            var skillContext = kvp.Key;
            int tickDamage = damagePerTick;
            List<Color> colors = new();
            foreach (var mod in skillContext.GetSkillInstancesWith<IFireModifier>())
            {
                tickDamage = CalculateDamage.MultIncrease(mod.modifier.PlusMult, tickDamage, 1);
                if(mod.modifier.Color != default)
                    colors.Add(mod.modifier.Color);
            }
            SetDamageData(tickData, kvp.Key, tickDamage * kvp.Value.Count, colors);
        }
    }
    
}