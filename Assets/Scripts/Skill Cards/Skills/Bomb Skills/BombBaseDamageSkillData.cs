using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Bomb Base Damage Data", menuName = "SkillData/Bomb/Bomb Base Damage")]
public class BombBaseDamageSkillData : SkillData
{
    public float baseDamageMult = 0.25f;
    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Your <color=#{hex}>Bombs</color> gain base damage of +{baseDamageMult * 100}% of your tower's base damage.";
    }

    public override SkillInstance CreateInstance()
    {
        return new BombBaseDamageSkillInstance(this);
    }
}

public class BombBaseDamageSkillInstance : SkillInstance<BombBaseDamageSkillData>, IBombModifier, IPlayCountPolicy<IBombModifier>, ITowerCardReceivedModifier
{
    public BombBaseDamageSkillInstance(BombBaseDamageSkillData data) : base(data)
    {
        
    }

    private void OnBaseDamageUpdated()
    {
        if (skillContext == null) return;
        RuntimeStat = Mathf.FloorToInt(skillContext.Tower.RuntimeData.BaseDamage * Data.baseDamageMult * PlayCount);
    } 

    public void Modify(BombEffectData bombData)
    {
        var plusDamage = Mathf.FloorToInt(skillContext.Tower.RuntimeData.BaseDamage * Data.baseDamageMult * PlayCount);
        bombData.damage += plusDamage;
        bombData.bomb.OnDealtDamage += _ =>
        {
            Damage += plusDamage;
        };
    }

    private Action<int> OnUpdateDamageHandle;
    public void Apply(TowerWaveData towerWaveData)
    {
        RuntimeStat = Mathf.FloorToInt(skillContext.Tower.RuntimeData.BaseDamage * Data.baseDamageMult * PlayCount);
        OnUpdateDamageHandle = _ => OnBaseDamageUpdated();
        skillContext.Tower.RuntimeData.OnBaseDamageUpdated += OnUpdateDamageHandle;
        OnPlayCountUpdated += OnUpdateDamageHandle;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        RuntimeStat = 0;
        OnPlayCountUpdated -= OnUpdateDamageHandle;
    }
    public int SetPlayCount() => 1;

}