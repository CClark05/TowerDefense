using UnityEngine;

[CreateAssetMenu(fileName = "Exposing Shot Data", menuName = "SkillData/Weakness/ExposingShot")]
public class ExposingShotSkillData : SkillData
{
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Applies 1 <color=#{hex}>Weak</color> on hit if target has zero <color=#{hex}>Weak</color> stacks.";
    }
    public override SkillInstance CreateInstance()
    {
        return new ExposingShotSkillInstance(this);
    }
}

public class ExposingShotSkillInstance : SkillInstance<ExposingShotSkillData>, IOnHit
{
    public void OnHit(HitData hitData)
    {
        PlayCard();
        Data.statusEffects[0].data.Execute(hitData);
    }

    public ExposingShotSkillInstance(ExposingShotSkillData data) : base(data)
    {
    }
}