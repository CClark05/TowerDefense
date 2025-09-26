using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Seeking Shot Data", menuName = "SkillData/Generic/Seeking Shot")]
public class SeekingShotSkillData : SkillData
{
    public override SkillInstance CreateInstance()
    {
        return new SeekingShotSkillInstance(this);
    }
}

public class SeekingShotSkillInstance : SkillInstance<SeekingShotSkillData>, IProjectileModifier
{
    public SeekingShotSkillInstance(SeekingShotSkillData data) : base(data)
    {
    }

    public IEnumerator Modify(ProjectileShotData shotData)
    {
        PlayCard();
        shotData.homing = true;
        yield break;
    }

    public bool DelayShot { get; }
}

