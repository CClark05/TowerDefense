using UnityEngine;

[CreateAssetMenu(fileName = "Critical Momentum Data", menuName = "SkillData/Crits/CriticalMomentum")]
public class CriticalMomentumSkillData : SkillData
{
    public int plusBaseDamage = 1;

    private void OnValidate()
    {
        string hex = ColorUtility.ToHtmlStringRGB(statusEffects[0].data.color);
        description = $"Card gains +{plusBaseDamage} base damage on <color=#{hex}>Critical</color> kill.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CriticalMomentumSkillInstance(this);
    }
}

public class CriticalMomentumSkillInstance : SkillInstance<CriticalMomentumSkillData>, IOnKill, ITowerCardReceivedModifier, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    private int accumulatedBonus;
    public CriticalMomentumSkillInstance(CriticalMomentumSkillData data) : base(data)
    {
    }
    public void OnKill(HitData hitData)
    {
        if (hitData.didCrit)
        {
            PlayCard();
            accumulatedBonus += Data.plusBaseDamage;
            hitData.dataHolder.RuntimeData.BaseDamage += Data.plusBaseDamage;
            RuntimeStat = accumulatedBonus;
        }
    }

    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage += accumulatedBonus;
        PlayCard();
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage -= accumulatedBonus;
    }

    int IPlayCountPolicy<ITowerCardReceivedModifier>.SetPlayCount() => 1;
}