using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Blood Pact Data", menuName = "SkillData/Economy/Blood Pact")]
public class BloodPactSkillData : SkillData
{
    public int livesLost = 5;
    public int plusGold = 10;
    private void OnValidate()
    {
        description = $"Lose {livesLost} lives on wave start but gain +${plusGold}.";
    }

    public override SkillInstance CreateInstance()
    {
        return new BloodPactSkillInstance(this);
    }
}

public class BloodPactSkillInstance : SkillInstance<BloodPactSkillData>, IPlayerWaveStartModifier
{
    public BloodPactSkillInstance(BloodPactSkillData data) : base(data)
    {
    }

    public void WaveStart()
    {
        PlayCard();
        PlayerLife.Instance.AddLives(-Data.livesLost);
        PlayerInventory.Instance.AddCoins(Data.plusGold);
    }

    public void WaveEnd()
    {
        
    }
}