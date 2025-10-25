using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Dividend Data", menuName = "SkillData/Economy/Dividend")]
public class DividendSkillData : SkillData
{
    public int plusGold = 4;

    private void OnValidate()
    {
        description = $"At the end of each wave, gain +${plusGold}.";
    }

    public override SkillInstance CreateInstance()
    {
        return new DividendSkillInstance(this);
    }
}
public class DividendSkillInstance : SkillInstance<DividendSkillData>, IPlayerWaveStartModifier
{
    public DividendSkillInstance(DividendSkillData data) : base(data)
    {
    }
    public void WaveEnd()
    {
        PlayerInventory.Instance.AddCoins(Data.plusGold);
        PlayCard();
    }

    public void WaveStart()
    {
    }

    
}