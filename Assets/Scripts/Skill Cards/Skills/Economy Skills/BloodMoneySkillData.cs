using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Blood Money Data", menuName = "SkillData/Economy/Blood Money")]
public class BloodMoneySkillData : SkillData
{
    [FormerlySerializedAs("MoneyPerKill")] public int PlusMoney = 1;
    public int killsNeeded = 2;
    private void OnValidate()
    {
        description = $"Gives +${PlusMoney} every {killsNeeded} kill(s).";
    }

    public override SkillInstance CreateInstance()
    {
        return new BloodMoneySkillInstance(this);
    }
}

public class BloodMoneySkillInstance : SkillInstance<BloodMoneySkillData>, IOnKill
{
    private int killCounter;
    public void OnKill(HitData hitData)
    {
        killCounter++;
        if(killCounter % Data.killsNeeded != 0) return;
        PlayerInventory.Instance.AddCoins(Data.PlusMoney);
        PlayCard();
    }

    public BloodMoneySkillInstance(BloodMoneySkillData data) : base(data)
    {
        EnemyManager.Instance.OnWaveComplete += OnWaveComplete;
    }

    private void OnWaveComplete() => killCounter = 0;

    public override void Dispose()
    {
        base.Dispose();
        EnemyManager.Instance.OnWaveComplete -= OnWaveComplete;
    }
}
