using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Searing Shots Data", menuName = "SkillData/Fire/Searing Shots")]
public class SearingShotsSkillData : SkillData
{
    public float stacksPerHit = 1;

    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Applies +{stacksPerHit} <color=#{hex}>Fire</color> per hit.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SearingShotsSkillInstance(this);
    }
}

public class SearingShotsSkillInstance : SkillInstance<SearingShotsSkillData>, IOnHit
{
    public SearingShotsSkillInstance(SearingShotsSkillData data) : base(data)
    {
    }

    public void OnHit(HitData hitData)
    {
        for (int i = 0; i < Data.stacksPerHit; i++)
        {
            Data.statusEffects[0].data.Execute(hitData);
        }
        PlayCard();
    }
}
