using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Payday Data", menuName = "SkillData/Economy/Payday")]
public class PaydaySkillData : SkillData
{
    public int goldPerKill = 5;

    private void OnValidate()
    {
        description = $"Gain +${goldPerKill} for each enemy killed. Self destructs after this wave.";
    }
    public override SkillInstance CreateInstance()
    {
        return new PaydaySkillInstance(this);
    }
}
public class PaydaySkillInstance : SkillInstance<PaydaySkillData>, ITowerWaveEndModifier, IOnKill
{
    public PaydaySkillInstance(PaydaySkillData data) : base(data)
    {
    }
    
    public void Modify(TowerWaveData towerWaveData)
    {
        towerWaveData.removedCards.Add(Data);
    }

    public void OnKill(HitData hitData)
    {
        PlayerInventory.Instance.AddCoins(Data.goldPerKill);
        PlayCard();
    }
}