using System;
using UnityEngine;
using UnityEngine.Serialization;
[CreateAssetMenu(fileName = "Slow Domain Data", menuName = "SkillData/Support/Slow Domain")]
public class SlowDomainSkillData : SkillData
{
    public float SlowBonus = 0.2f;
    private void OnValidate()
    {
        description = $"Enemies inside your range move +{SlowBonus * 100}% slower.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SlowDomainSkillInstance(this);
    }
}
public class SlowDomainSkillInstance : SkillInstance<SlowDomainSkillData>, IOnEnemyEnteredRange, IPlayCountPolicy<IOnEnemyEnteredRange>
{
    public SlowDomainSkillInstance(SlowDomainSkillData data) : base(data)
    {
    }

    public void OnEnemyEnteredRange(IDamageable enemy)
    {
        enemy.Transform.GetComponent<IMovementOverride>().AddSpeed(-(Data.SlowBonus * PlayCount));
    }

    public void OnEnemyLeftRange(IDamageable enemy)
    {
        enemy.Transform.GetComponent<IMovementOverride>().AddSpeed(Data.SlowBonus * PlayCount);
    }

    public int SetPlayCount() => 1;
}
