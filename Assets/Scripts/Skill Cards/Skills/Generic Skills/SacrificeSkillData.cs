using UnityEngine;

[CreateAssetMenu(fileName = "Sacrifice Data", menuName = "SkillData/Generic/Sacrifice")]
public class SacrificeSkillData : SkillData
{
    public int plusDamage = 4;

    private void OnValidate()
    {
        description = $"Card gains +{plusDamage} base damage every time one of your cards self destructs.";
    }

    public override SkillInstance CreateInstance()
    {
        return new SacrificeSkillInstance(this);
    }
}

public class SacrificeSkillInstance : SkillInstance<SacrificeSkillData>, IOnCardSelfDestruct, ITowerCardReceivedModifier
{
    private int _accumulatedBonus;
    private int totalAccumulatedBonus;
    private bool appliedInitialBonus;
    public SacrificeSkillInstance(SacrificeSkillData data) : base(data)
    {
    }
    void ITowerCardReceivedModifier.Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage += totalAccumulatedBonus;
        PlayCard();
        appliedInitialBonus = true;
    }

    public void OnComplete()
    {
        skillContext.Tower.RuntimeData.BaseDamage += _accumulatedBonus;
        _accumulatedBonus = 0;
    }
    void IOnCardSelfDestruct.Apply(TowerWaveData towerWaveData)
    {
        Debug.Log("Sacrifice triggered");
        if (!appliedInitialBonus) return;
        totalAccumulatedBonus += Data.plusDamage;
        _accumulatedBonus += Data.plusDamage;
        appliedInitialBonus = true;
        PlayCard();
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage -= totalAccumulatedBonus;
        appliedInitialBonus = false;
    }

    public bool alwaysPlayOnce { get; } = true;
}