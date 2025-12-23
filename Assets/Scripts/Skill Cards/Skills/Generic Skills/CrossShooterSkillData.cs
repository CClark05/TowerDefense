using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Cross Shooter Data", menuName = "SkillData/Generic/Cross Shooter")]
public class CrossShooterSkillData : SkillData
{
    public override SkillInstance CreateInstance()
    {
        return new CrossShooterSkillInstance(this);
    }
}
public class CrossShooterSkillInstance : SkillInstance<CrossShooterSkillData>, IProjectileModifier, IPlayCountPolicy<IProjectileModifier>
{
    public CrossShooterSkillInstance(CrossShooterSkillData data) : base(data)
    {
    }

    public IEnumerator Modify(ProjectileShotData shotData)
    {
        
        yield return null;
    }

    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}
