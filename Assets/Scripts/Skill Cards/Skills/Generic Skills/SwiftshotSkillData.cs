using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Swiftshot Data", menuName = "SkillData/Generic/Swiftshot")]
public class SwiftshotSkillData : SkillData
{
    public float speedIncrease = 1.25f;

    private void OnValidate()
    {
        description = $"Increases projectile speed by +{speedIncrease * 100}%.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SwiftshotSkillInstance(this);
    }
}

public class SwiftshotSkillInstance : SkillInstance<SwiftshotSkillData>, IProjectileModifier, IPlayCountPolicy<IProjectileModifier>
{
    public SwiftshotSkillInstance(SwiftshotSkillData data) : base(data)
    {
    }

    public IEnumerator Modify(ProjectileShotData shotData)
    {
        shotData.speedIncrease *= 1 + Data.speedIncrease * PlayCount;
        PlayCard();
        yield return null;
    }

    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}