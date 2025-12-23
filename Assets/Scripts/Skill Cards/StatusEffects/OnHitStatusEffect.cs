using UnityEngine;

public abstract class PersistentStatusEffect : StatusEffectData
{
    public static void AddPersistentEffect(PersistentStatusEffect statusEffect, SkillContext skillContext, IUsesStatusEffects enemy, HitData hitData, int stacks)
    {
        var effectData = new ModifyEffectData();
        CallModifier.Call<IOnEffectApplied>(skillContext, (mod, instance) =>
        {
            mod.Modify(effectData, hitData);
        });
        enemy.AddPersistentEffect(statusEffect, hitData, stacks, effectData);
    }
}
public abstract class OnHitStatusEffect : PersistentStatusEffect
{
    public abstract void OnPersistentHit(HitData hitData, int stacks);

    protected void RemoveAllStacks(HitData hitData)
    {
        hitData.statusEffects.RemoveAllStacks(this);
    }

    protected void RemoveStacks(HitData hitData, int stacks)
    {
        hitData.statusEffects.RemoveStacks(this, stacks);
    }
}