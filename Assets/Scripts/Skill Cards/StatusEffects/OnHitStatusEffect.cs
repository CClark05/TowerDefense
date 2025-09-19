using UnityEngine;

public abstract class PersistentStatusEffect : StatusEffectData
{
    
}
public abstract class OnHitStatusEffect : PersistentStatusEffect
{
    public abstract void OnPersistentHit(HitData hitData, int stacks);

    protected void RemoveAllStacks(HitData hitData)
    {
        hitData.statusEffects.RemoveAllStacks(this);
    }
}