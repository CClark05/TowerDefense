using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Soft Spot Data", menuName = "SkillData/Weakness/Soft Spot")]
public class SoftSpotSkillData : SkillData
{
    public int weakStacks = 2;

    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"First hit on an enemy applies {weakStacks} <color=#{hex}>Weak</color>.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SoftSpotSkillInstance(this);
    }
}

public class SoftSpotSkillInstance : SkillInstance<SoftSpotSkillData>
{
    public SoftSpotSkillInstance(SoftSpotSkillData data) : base(data)
    {
    }
}