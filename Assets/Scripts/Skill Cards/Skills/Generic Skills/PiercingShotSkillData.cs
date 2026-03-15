using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Piercing Shot Data", menuName = "SkillData/Generic/Piercing Shot")]
public class PiercingShotSkillData : SkillData
{
    public int maxEnemiesPierced = 3;

    private void OnValidate()
    {
        description = $"Allows projectiles to pierce through +{maxEnemiesPierced - 1} additional enemies.";
    }

    public override SkillInstance CreateInstance()
    {
        return new PiercingShotSkillInstance(this);
    }
}

public class PiercingShotSkillInstance : SkillInstance<PiercingShotSkillData>, IProjectileModifier, IPlayCountPolicy<IProjectileModifier>
{
    public PiercingShotSkillInstance(PiercingShotSkillData data) : base(data)
    {
    }

    public IEnumerator Modify(ProjectileShotData shotData)
    {
        shotData.maxEnemiesPierced += 1 + ((Data.maxEnemiesPierced - 1)* PlayCount);
        yield return null;
    }

    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}