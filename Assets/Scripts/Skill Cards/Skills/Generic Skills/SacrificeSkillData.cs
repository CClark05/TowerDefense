using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Sacrifice Data", menuName = "SkillData/Generic/Sacrifice")]
public class SacrificeSkillData : SkillData
{
    public int plusDamage = 4;

    private void OnValidate()
    {
        description = $"Gains +{plusDamage} base damage every time one of your cards self destructs. Resets if card is removed.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SacrificeSkillInstance(this);
    }
}
public class SacrificeSkillInstance : SkillInstance<SacrificeSkillData>, IOnCardSelfDestruct, IOnRemoval
{
    private int accumulatedBonus;
    public SacrificeSkillInstance(SacrificeSkillData data) : base(data)
    {
    }
    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage += Data.plusDamage;
        accumulatedBonus += Data.plusDamage;
        PlayCard();
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage -= accumulatedBonus;
        accumulatedBonus = 0;
    }
}