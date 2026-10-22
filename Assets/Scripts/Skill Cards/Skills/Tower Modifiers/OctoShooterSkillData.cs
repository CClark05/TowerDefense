using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Cross Shooter Data", menuName = "SkillData/Shot Modifiers/Cross Shooter")]
public class OctoShooterSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"Tower fires a ring of 8 arrows in all directions.";
    }

    public override SkillInstance CreateInstance()
    {
        return new OctoShooterSkillInstance(this);
    }
}

public class OctoShooterSkillInstance : SkillInstance<OctoShooterSkillData>, IShotModifier, IPlayCountPolicy<IShotModifier>
{
    public OctoShooterSkillInstance(OctoShooterSkillData data) : base(data)
    {
    }

    public void Modify(ProjectileShotData shotData)
    {
        shotData.shootMainProjectile = false;
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            shotData.directionOverrides.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
        }
    }

    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}