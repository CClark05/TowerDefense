using UnityEngine;

[CreateAssetMenu(fileName = "Critical Momentum Data", menuName = "SkillData/Crits/CriticalMomentum")]
public class CriticalMomentumSkillData : SkillData
{
    public int plusBaseDamage = 1;

    private void OnValidate()
    {
        description = $"Card gains +{plusBaseDamage} base damage on Critical kill.";
    }

    public override SkillInstance CreateInstance()
    {
        return new CriticalMomentumSkillInstance(this);
    }
}

public class CriticalMomentumSkillInstance : SkillInstance<CriticalMomentumSkillData>, IOnKill, ITowerCardReceivedModifier
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

    public bool alwaysPlayOnce { get; } = true;
}