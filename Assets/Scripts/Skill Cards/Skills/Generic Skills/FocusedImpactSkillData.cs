using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Focused Impact Data", menuName = "SkillData/Generic/FocusedImpact")]
public class FocusedImpactSkillData : SkillData
{
    public int PlusDamage = 2;
    
    private void OnValidate()
    {
        description = $"Adds +{PlusDamage} base damage.";
    }

    public override SkillInstance CreateInstance()
    {
        return new FocusedImpactSkillInstance(this);
    }
}

public class FocusedImpactSkillInstance : SkillInstance<FocusedImpactSkillData>, ITowerCardReceivedModifier, IOnHit, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    public FocusedImpactSkillInstance(FocusedImpactSkillData data) : base(data)
    {
        OnPlayCountUpdatedHandle += _ =>
        {
            skillContext.Tower.RuntimeData.BaseDamage -= bonus;
            bonus = Data.PlusDamage * PlayCount;
            skillContext.Tower.RuntimeData.BaseDamage += bonus;
        };
    }

    private int bonus;
    private Action<int> OnPlayCountUpdatedHandle;
    public void Apply(TowerWaveData towerWaveData)
    {
        bonus = Data.PlusDamage * PlayCount;
        towerWaveData.increasedBaseDamage += bonus;
        OnPlayCountUpdated += OnPlayCountUpdatedHandle;
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage -= bonus;
        OnPlayCountUpdated -= OnPlayCountUpdatedHandle;
    }

    public void OnHit(HitData hitData)
    {
        Damage += Data.PlusDamage;
    }

    public int SetPlayCount() => 1;
}

