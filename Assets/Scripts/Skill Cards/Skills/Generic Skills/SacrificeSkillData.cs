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

public class SacrificeSkillInstance : SkillInstance<SacrificeSkillData>, IOnCardSelfDestruct, ITowerCardReceivedModifier, IPlayCountPolicy<IOnCardSelfDestruct>, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    private int accumulatedBonus;
    private int appliedBonus;
    public SacrificeSkillInstance(SacrificeSkillData data) : base(data)
    {
    }
    void ITowerCardReceivedModifier.Apply(TowerWaveData towerWaveData)
    {
        SyncBonus(towerWaveData);
        PlayCard();
    }

    void IOnCardSelfDestruct.Apply(TowerWaveData towerWaveData)
    {
        Debug.Log("Sacrifice triggered");
        accumulatedBonus += Data.plusDamage * PlayCount;
        SyncBonus(towerWaveData);
        PlayCard();
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedBaseDamage -= appliedBonus;
        appliedBonus = 0;
    }

    public int SetPlayCount() => 1;
    private void SyncBonus(TowerWaveData towerWaveData)
    {
        int delta = accumulatedBonus - appliedBonus;
        if (delta == 0) return;

        towerWaveData.increasedBaseDamage += delta;
        appliedBonus = accumulatedBonus;
    }
}