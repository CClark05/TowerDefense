using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Relentless Exposure Data", menuName = "SkillData/Weakness/Relentless Exposure")]
public class RelentlessExposureSkillData : SkillData
{
    public int additionalStacks = 1;

    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Re-applying <color=#{hex}>Weak</color> to an enemy grants additional stacks, increasing by +1 each re-apply.";
    }

    public override SkillInstance CreateInstance()
    {
        return new RelentlessExposureSkillInstance(this);
    }
}

public class RelentlessExposureSkillInstance : SkillInstance<RelentlessExposureSkillData>, IOnEffectApplied
{
    private IUsesStatusEffects statusEffects;
    public RelentlessExposureSkillInstance(RelentlessExposureSkillData data) : base(data)
    {
    }
    public EffectData Effect => Data.statusEffects[0].data;
    public void Modify(ModifyEffectData effectData, HitData hitData)
    {
        if(hitData.statusEffects.PersistentEffectTotalApplications.TryGetValue(Data.statusEffects[0].data as PersistentStatusEffect, out var applications))
        {
            PlayCard();
            effectData.additionalStacks += (applications) * Data.additionalStacks;
        }
    }

    
}
