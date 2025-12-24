using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Far Sight Data", menuName = "SkillData/Generic/FarSight")]
public class FarSightSkillData : SkillData
{
    public float increasedRange = 1.25f;

    private void OnValidate()
    {
        description = $"Gives +{(increasedRange* 100)}% range.";
    }

    public override SkillInstance CreateInstance()
    {
        return new FarSightSkillInstance(this);
    }
}

public class FarSightSkillInstance : SkillInstance<FarSightSkillData>, ITowerCardReceivedModifier, IPlayCountPolicy<ITowerCardReceivedModifier>
{
    public FarSightSkillInstance(FarSightSkillData data) : base(data)
    {
    }
    public void Apply(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedRange *= 1f + Data.increasedRange * PlayCount;
        PlayCard();
    }

    public void Remove(TowerWaveData towerWaveData)
    {
        towerWaveData.increasedRange /= 1f + Data.increasedRange * PlayCount;
    }

    public int SetPlayCount() => 1;
}