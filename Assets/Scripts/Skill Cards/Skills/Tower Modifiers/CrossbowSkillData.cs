using UnityEngine;

[CreateAssetMenu(fileName = "Crossbow Data", menuName = "SkillData/Tower Modifiers/Crossbow")]
public class CrossbowSkillData : SkillData
{
    private void OnValidate()
    {
        description = $"The most basic tower. Fires a single arrow at enemies.";
    }
    public override SkillInstance CreateInstance()
    {
        return new CrossbowSkillInstance(this);
    }
}

public class CrossbowSkillInstance : SkillInstance<CrossbowSkillData>
{
    public CrossbowSkillInstance(CrossbowSkillData data) : base(data)
    {
        
    }
}
