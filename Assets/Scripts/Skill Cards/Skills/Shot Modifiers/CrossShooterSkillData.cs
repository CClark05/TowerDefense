using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Cross Shooter Data", menuName = "SkillData/Generic/Cross Shooter")]
public class CrossShooterSkillData : SkillData
{
    private void OnValidate()
    {
        description = "Fires 3 additional projectiles in a cross pattern relative to the original projectile.";
    }

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
        Vector2 left = Vector2.Perpendicular(shotData.originalDirection);
        Vector2 right = -left;
        Vector2 back = -shotData.originalDirection;
        shotData.directionOverrides.Add(left);
        shotData.directionOverrides.Add(right);
        shotData.directionOverrides.Add(back);
        yield return null;
    }

    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}