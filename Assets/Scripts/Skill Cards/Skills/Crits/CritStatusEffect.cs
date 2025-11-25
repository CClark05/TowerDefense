using UnityEngine;

[CreateAssetMenu(fileName = "Crit Status Effect", menuName = "StatusEffects/Crit")]
public class CritStatusEffect : OnHitStatusEffect
{
    private void OnValidate()
    {
        description = $"Deals +{CritStats.baseCritMult * 100}% damage on hit.";
    }
    public override void OnPersistentHit(HitData hitData, int stacks)
    {
        //noop
    }
}
