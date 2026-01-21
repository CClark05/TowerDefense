using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Bigger Bombs Data", menuName = "SkillData/Bomb/Bigger Bombs")]
public class BiggerBombsSkillData : SkillData
{
    public float RadiusIncrease = 0.5f;

    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Your <color=#{hex}>Bombs</color> deal damage in a +{RadiusIncrease * 100}% tile radius.";
    }

    public override SkillInstance CreateInstance()
    {
        return new BiggerBombsSkillInstance(this);
    }
}
public class BiggerBombsSkillInstance : SkillInstance<BiggerBombsSkillData>, IBombModifier, IPlayCountPolicy<IBombModifier>
{
    public BiggerBombsSkillInstance(BiggerBombsSkillData data) : base(data)
    {
    }
    public void Modify(BombEffectData bombData)
    {
        bombData.tileRadius += CalculateDamage.MultIncrease(Data.RadiusIncrease, (Data.statusEffects[0].data as BombEffectData).tileRadius, PlayCount);
    }
    

    public int SetPlayCount() => 1;
    
}