using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Seeking Shot Data", menuName = "SkillData/Generic/Seeking Shot")]
public class SeekingShotSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"+1 of your projectiles fired home in on enemies.";
    }
    public override SkillInstance CreateInstance()
    {
        return new SeekingShotSkillInstance(this);
    }
}

public class SeekingShotSkillInstance : SkillInstance<SeekingShotSkillData>, IProjectileModifier, IPlayCountPolicy<IProjectileModifier>
{
    public SeekingShotSkillInstance(SeekingShotSkillData data) : base(data)
    {
    }

    public IEnumerator Modify(ProjectileShotData shotData)
    {
        PlayCard();
        shotData.homingProjectiles += PlayCount;
        yield break;
    }

    public bool DelayShot { get; }
    public int SetPlayCount() => 1;
}

