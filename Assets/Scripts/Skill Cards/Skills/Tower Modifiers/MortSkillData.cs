
using UnityEngine;

[CreateAssetMenu(fileName = "Mort Data", menuName = "SkillData/Tower Modifiers/Mort")]
public class MortSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"Mort.";
    }
    public override SkillInstance CreateInstance()
    {
        return new MortSkillInstance(this);
    }
}

public class MortSkillInstance : SkillInstance<MortSkillData>
{
    public MortSkillInstance(MortSkillData data) : base(data)
    {
    }
}