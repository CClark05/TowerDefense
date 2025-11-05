using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Quickdraw Data", menuName = "SkillData/Generic/Quickdraw")]
public class QuickdrawSkillData : SkillData
{
    public float speedIncrease = 1.1f;

    private void OnValidate()
    {
        description = $"Increases fire rate by {(speedIncrease - 1) * 100}%.";
    }

    public override SkillInstance CreateInstance()
    {
        return new QuickdrawSkillInstance(this);
    }
}

public class QuickdrawSkillInstance : SkillInstance<QuickdrawSkillData>, ITowerCardReceivedModifier
{
    public QuickdrawSkillInstance(QuickdrawSkillData data) : base(data)
    {
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedSpeed /= Data.speedIncrease;
    }

    public bool alwaysPlayOnce { get; }

    public void Apply(TowerWaveData towerWaveData)
    {
        PlayCard();
        towerWaveData.increasedSpeed *= Data.speedIncrease;
    }
}