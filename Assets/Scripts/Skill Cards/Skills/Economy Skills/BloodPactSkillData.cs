using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Blood Pact Data", menuName = "SkillData/Economy/Blood Pact")]
public class BloodPactSkillData : SkillData
{
    public int livesLost = 5;
    public int plusGold = 10;
    private void OnValidate()
    {
        description = $"Lose {livesLost} lives on wave start but gain +${plusGold}. Self destructs before if it would reduce your lives to 0.";
    }

    public override SkillInstance CreateInstance()
    {
        return new BloodPactSkillInstance(this);
    }
}

public class BloodPactSkillInstance : SkillInstance<BloodPactSkillData>, IPlayerWaveStartModifier, ITowerWaveStartModifier
{
    public BloodPactSkillInstance(BloodPactSkillData data) : base(data)
    {
        
    }
    public void Modify(TowerWaveData towerWaveData)
    {
        if (PlayerLife.Instance.CurrentLives <= Data.livesLost)
            towerWaveData.removedCards.Add(Data);
    }
    public void WaveStart()
    {
        if (PlayerLife.Instance.CurrentLives <= Data.livesLost) return;
        PlayCard();
        PlayerLife.Instance.AddLives(-Data.livesLost);
        PlayerInventory.Instance.AddCoins(Data.plusGold);
    }

    public void WaveEnd()
    {
        
    }

    
}