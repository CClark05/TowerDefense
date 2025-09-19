using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Blood Money Data", menuName = "SkillData/Economy/Blood Money")]
public class BloodMoneySkillData : SkillData
{
    public int MoneyPerKill = 5;

    private void OnValidate()
    {
        description = $"Gives +${MoneyPerKill} per kill.";
    }

    public override SkillInstance CreateInstance()
    {
        return new BloodMoneySkillInstance(this);
    }
}

public class BloodMoneySkillInstance : SkillInstance<BloodMoneySkillData>, IOnKill
{
    public void OnKill(HitData hitData)
    {
        PlayerInventory.Instance.AddCoins(Data.MoneyPerKill);
        PlayCard();
    }

    public BloodMoneySkillInstance(BloodMoneySkillData data) : base(data)
    {
    }
}
