using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Azure Flame Data", menuName = "SkillData/Fire/Azure Flame")]
public class AzureFlameSkillData : SkillData
{
    public float burnDamageMult = 1f;
    public Color color;
    public int stackDuration = 5;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"<color=#{hex}>Fire</color> burns hotter, dealing +{burnDamageMult * 100}% damage. However each stack only lasts {stackDuration} seconds.";
    }

    public override SkillInstance CreateInstance()
    {
        return new AzureFlameSkillInstance(this);
    }
}
public class AzureFlameSkillInstance : SkillInstance<AzureFlameSkillData>, IFireModifier, IOnEffectApplied
{
    public AzureFlameSkillInstance(AzureFlameSkillData data) : base(data)
    {
    }

    public float PlusMult => Data.burnDamageMult * PlayCount;
    public Color Color => Data.color;
    public void Modify(ModifyEffectData effectData, HitData hitData)
    {
        effectData.duration = 0;
        effectData.duration += Data.stackDuration;
        Debug.Log("Modified duration : " + effectData.duration);
    }
    public bool PlayOnce => true;
    public EffectData Effect => Data.statusEffects[0].data;
    
}

public interface IFireModifier
{
    public float PlusMult { get; }
    public Color Color { get; }
}