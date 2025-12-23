using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun Status Effect", menuName = "StatusEffects/Stun")]
public class StunStatusEffect : OnHitStatusEffect
{
    public float StunDuration = 0.5f;
    private void OnValidate()
    {
        description = $"Enemy is unable to move for {StunDuration} seconds per stack.";
    }
    public override void Execute(HitData hitData)
    {
    }
    public override void OnPersistentHit(HitData hitData, int stacks)
    {
        
    }
    public void AddStacks(HitData hitData, int stacks)
    {
        hitData.effectsApplied[this] = hitData.effectsApplied.GetValueOrDefault(this) + stacks;
        hitData.damageable.Transform.GetComponent<IMovementOverride>().SetSpeed(0);
        FunctionTimer.Create(() =>
        {
            hitData.damageable.Transform.GetComponent<IMovementOverride>().ResetSpeed();
            RemoveStacks(hitData, stacks);
        }, StunDuration * stacks);
    }
}
