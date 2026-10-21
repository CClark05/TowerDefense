using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Cross Shooter Data", menuName = "SkillData/Shot Modifiers/Cross Shooter")]
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

public class CrossShooterSkillInstance : SkillInstance<CrossShooterSkillData>, IShotModifier, IPlayCountPolicy<IShotModifier>
{
    public CrossShooterSkillInstance(CrossShooterSkillData data) : base(data)
    {
    }

    public void Modify(ProjectileShotData shotData)
    {
        Vector2 left = Vector2.Perpendicular(shotData.originalDirection);
        Vector2 right = -left;
        Vector2 back = -shotData.originalDirection;
        shotData.directionOverrides.Add(left);
        shotData.directionOverrides.Add(right);
        shotData.directionOverrides.Add(back);
    }

    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}