using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Quickdraw Data", menuName = "SkillData/Generic/Quickdraw")]
public class QuickdrawSkillData : SkillData
{
    public float speedIncrease = 0.25f;

    private void OnValidate()
    {
        description = $"Increases fire rate by +{speedIncrease * 100}%.";
    }

    public override SkillInstance CreateInstance()
    {
        return new QuickdrawSkillInstance(this);
    }
}

public class QuickdrawSkillInstance : SkillInstance<QuickdrawSkillData>, ITowerCardReceivedModifier, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    public QuickdrawSkillInstance(QuickdrawSkillData data) : base(data)
    {
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        skillContext.Tower.RuntimeData.TimeBetweenShots *= 1 + Data.speedIncrease * PlayCount;
    }
    
    public void Apply(TowerWaveData towerWaveData)
    {
        PlayCard();
        skillContext.Tower.RuntimeData.TimeBetweenShots /= 1 + Data.speedIncrease * PlayCount;
    }

    public int SetPlayCount() => 1;
}