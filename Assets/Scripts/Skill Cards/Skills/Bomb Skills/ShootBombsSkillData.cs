using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Shoot Bomb Data", menuName = "SkillData/Bomb/Shoot Bomb")]
public class ShootBombsSkillData : SkillData
{
    public float PlusDamage = 0.2f;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"If your <color=#{hex}>Bombs</color> are hit by projectiles they explode immediately for +{PlusDamage * 100}% damage."; 
    }

    public override SkillInstance CreateInstance()
    {
        return new ShootBombsSkillInstance(this);
    }
}
public class ShootBombsSkillInstance : SkillInstance<ShootBombsSkillData>, IBombModifier, IPlayCountPolicy<IBombModifier>
{
    public ShootBombsSkillInstance(ShootBombsSkillData data) : base(data)
    {
    }
    public void Modify(BombEffectData bombData)
    {
        bombData.explodeOnShot += () =>
        {
            var damage = CalculateDamage.MultIncrease(Data.PlusDamage, bombData.baseDamage, PlayCount);
            bombData.damage += damage;
            Damage += damage;
        };
    }
    public int SetPlayCount() => 1;
    
}