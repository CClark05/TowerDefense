using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Payday Data", menuName = "SkillData/Economy/Payday")]
public class PaydaySkillData : SkillData
{
    public int goldPerKill = 3;

    private void OnValidate()
    {
        description = $"Gain +${goldPerKill} for each enemy killed. Self destructs after this wave.";
    }
    public override SkillInstance CreateInstance()
    {
        return new PaydaySkillInstance(this);
    }
}
public class PaydaySkillInstance : SkillInstance<PaydaySkillData>, ITowerWaveEndModifier, IOnKill, ISelfDestructs, IPlayCountPolicy<ITowerWaveEndModifier>
{
    public Action OnSelfDestruct { get; set; }
    public PaydaySkillInstance(PaydaySkillData data) : base(data)
    {
    }
    
    public void Modify(TowerWaveData towerWaveData)
    {
        OnSelfDestruct?.Invoke();
    }

    public void OnKill(HitData hitData)
    {
        PlayerInventory.Instance.AddCoins(Data.goldPerKill);
        PlayCard();
    }


    public int SetPlayCount() => 1;
}